using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Data;
using System.Data.SqlClient;

namespace eServiceOnline.WebAPI.Data.SharePointSync
{
    public class SharePoint_SyncProcess
    {
        static public bool UpdateDbToSp(SharePoint_SyncData.SqlStuff ss) //SpListConfigProfile pConfig) //, DataTable dt)
        {
            bool result = true;

            int rowsPerPage = 100;

            if (ss.ConfigProfile.SpListAction == SharePoint_SyncData.SpListConfigProfile.ProcessingAction.Create)
                SharePoint_SyncData.SpStuff.DeleteSpList(ss.ConfigProfile);

            DataTable dt = ss.GetDataFromSQL(ss.ConfigProfile, false);

            if (!SharePoint_SyncData.SpStuff.ProcessListExitenceAndStructure(dt, ss.ConfigProfile))
            {
                string errorMsg = "\n" + System.String.Format("Error : Cannot confirm list existence : '{0}'", ss.ConfigProfile.SharePointListName);
                throw new Exception(errorMsg);
            }

            using (var clientContext = SharePoint_SyncData.SpStuff.GetClientContext(ss.ConfigProfile))
            {
                string spKeyColumnName = SharePoint_SyncData.SqlStuff.CheckColumnName(ss.ConfigProfile.SqlKeyColumnName);

                Web web = clientContext.Web;
                clientContext.Load(web);
                List list = web.Lists.GetByTitle(ss.ConfigProfile.SharePointListName);

                ListItemCollectionPosition itemPosition = null;

                System.Collections.Generic.List<string> spIds = new System.Collections.Generic.List<string>();

                if (ss.ConfigProfile.SpListAction != SharePoint_SyncData.SpListConfigProfile.ProcessingAction.AddFromStoredProcedure)
                {
                    while (true)
                    {
                        CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);
                        camlQuery.ListItemCollectionPosition = itemPosition;

                        ListItemCollection listItems = list.GetItems(camlQuery);
                        clientContext.Load(listItems);
                        clientContext.Load(list.Fields);
                        clientContext.ExecuteQuery();

                        for (int j = listItems.Count - 1; j >= 0; j--)
                        {
                            ListItem li = listItems[j];

                            spIds.Add((li[spKeyColumnName] ?? "").ToString());

                            UpdateDeleteExistingSpRecords(ss, dt, list, li);
                        }

                        clientContext.ExecuteQuery();

                        itemPosition = listItems.ListItemCollectionPosition;

                        if (itemPosition == null)
                        {
                            break; // TODO: might not be correct. Was : Exit While
                        }
                    }
                }

                //Add new rows from Sql to Sp
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (ss.ConfigProfile.SpListAction == SharePoint_SyncData.SpListConfigProfile.ProcessingAction.AddFromStoredProcedure || (dr[ss.ConfigProfile.SqlKeyColumnName] != DBNull.Value && !spIds.Contains(dr[ss.ConfigProfile.SqlKeyColumnName].ToString())))
                    {
                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem newItem = list.AddItem(itemCreateInfo);
                        foreach (DataColumn dc in dt.Columns)
                        {
                            string spColumnName = SharePoint_SyncData.SqlStuff.CheckColumnName(dc.ColumnName);

                            if (spColumnName.Equals(spKeyColumnName))
                                newItem["Title"] = dr[dc.ColumnName];

                            newItem[spColumnName] = dr[dc.ColumnName];
                        }
                        newItem.Update();
                        clientContext.Load(newItem);

                        i++;

                        if (i >= rowsPerPage)
                        {
                            clientContext.Load(list);
                            clientContext.ExecuteQuery();
                            i = 0;
                        }
                    }
                }

                clientContext.Load(list);
                clientContext.ExecuteQuery();
            }
            return result;
        }

        static private void UpdateDeleteExistingSpRecords(SharePoint_SyncData.SqlStuff ss, DataTable dt, List list, ListItem li)
        {
            string spKeyColumnName = SharePoint_SyncData.SqlStuff.CheckColumnName(ss.ConfigProfile.SqlKeyColumnName);

            if (li[spKeyColumnName] != null)
            {
                DataRow dr = SharePoint_SyncData.SqlStuff.GetRowByKey(dt, ss.ConfigProfile.SqlKeyColumnName, li[spKeyColumnName].ToString());

                if (dr != null)
                {
                    bool isUpdated = false;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        string spColumnName = SharePoint_SyncData.SqlStuff.CheckColumnName(dc.ColumnName);

                        if (spColumnName.Equals(spKeyColumnName) && !(li["Title"] ?? "").Equals((dr[dc.ColumnName] ?? "").ToString()))
                        {
                            isUpdated = true;
                            li["Title"] = dr[dc.ColumnName];
                        }

                        FieldType ttp = new FieldType();
                        foreach (Field field in list.Fields)
                        {
                            if (field.InternalName == spColumnName)
                            {
                                ttp = field.FieldTypeKind;
                                continue;
                            }
                        }

                        bool fieldsDiffer = false;
                        switch (ttp)
                        {
                            case FieldType.Number:
                            case FieldType.Currency:
                                fieldsDiffer = !(li[spColumnName] ?? 0.0).Equals(Double.Parse(dr[dc.ColumnName] is DBNull ? "0.0" : dr[dc.ColumnName].ToString()));
                                break;
                            case FieldType.Boolean:
                                fieldsDiffer = !(li[spColumnName] ?? false).Equals((bool)(dr[dc.ColumnName] is DBNull ? false : dr[dc.ColumnName]));
                                break;
                            case FieldType.DateTime:
                                fieldsDiffer = !(li[spColumnName] ?? new DateTime(1900, 01, 01)).Equals((DateTime)(dr[dc.ColumnName] is DBNull ? new DateTime(1900, 01, 01) : dr[dc.ColumnName]));
                                break;
                            default:
                                fieldsDiffer = !(li[spColumnName] ?? "").Equals((dr[dc.ColumnName] ?? "").ToString());
                                break;
                        }

                        if (fieldsDiffer)
                        {
                            isUpdated = true;
                            li[spColumnName] = dr[dc.ColumnName];
                        }

                    }

                    if (isUpdated)
                        li.Update();
                }
                else
                {
                    if (String.IsNullOrEmpty(ss.ConfigProfile.CheckSpOrphanedWithinColumnName))
                    {
                        li.DeleteObject();
                    }
                    else
                    {
                        //Delete list item only if SQl data containg any record where sql[CheckSpOrphanedWithinColumnName].Value = sp[CheckSpOrphanedWithinColumnName].Value
                        string spListColumnName = SharePoint_SyncData.SqlStuff.CheckColumnName(ss.ConfigProfile.CheckSpOrphanedWithinColumnName);
                        if (SharePoint_SyncData.SqlStuff.GetRowByKey(dt, ss.ConfigProfile.CheckSpOrphanedWithinColumnName, li[spListColumnName].ToString()) != null)
                            li.DeleteObject();
                    }
                }
            }
        }

        static public bool UpdateSpToDb(SharePoint_SyncData.SqlStuff ss)
        {
            bool result = true;

            try
            {
                DataTable dt = ss.GetDataFromSQL(ss.ConfigProfile, true);

                using (var clientContext = SharePoint_SyncData.SpStuff.GetClientContext(ss.ConfigProfile))
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web);

                    List list = web.Lists.GetByTitle(ss.ConfigProfile.SharePointListName);
                    clientContext.Load(list);

                    CamlQuery query = CamlQuery.CreateAllItemsQuery();
                    ListItemCollection listItems = list.GetItems(query);
                    clientContext.Load(listItems);
                    clientContext.ExecuteQuery();

                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName == ss.ConfigProfile.SqlKeyColumnName)
                            dc.Unique = true;
                    }

                    string spKeyColumnName = SharePoint_SyncData.SqlStuff.CheckColumnName(ss.ConfigProfile.SqlKeyColumnName);

                    foreach (ListItem li in listItems)
                    {
                        bool isNewItem = true;

                        if (!string.IsNullOrEmpty(li[spKeyColumnName].ToString()))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (li[spKeyColumnName].ToString() == dr[ss.ConfigProfile.SqlKeyColumnName].ToString())
                                {
                                    isNewItem = false;
                                    foreach (DataColumn dc in dt.Columns)
                                    {
                                        string spColumnName = SharePoint_SyncData.SqlStuff.CheckColumnName(dc.ColumnName);
                                        if (li[spColumnName] != dr[dc.ColumnName])
                                        {
                                            if (li[spColumnName] == null)
                                                dr[dc.ColumnName] = DBNull.Value;
                                            else
                                                dr[dc.ColumnName] = li[spColumnName];
                                        }
                                    }
                                    break;
                                }
                            }

                            if (isNewItem)
                            {
                                DataRow dr = dt.NewRow();
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    string spColumnName = SharePoint_SyncData.SqlStuff.CheckColumnName(dc.ColumnName);
                                    dr[dc.ColumnName] = li[spColumnName];
                                }
                                dt.Rows.Add(dr);
                            }
                        }
                    }

                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = dt.Rows[i];
                        bool notFound = true;
                        foreach (ListItem li in listItems)
                        {
                            if (li[spKeyColumnName].ToString() == dr[ss.ConfigProfile.SqlKeyColumnName].ToString())
                                notFound = false;
                        }
                        if (notFound)
                            dr.Delete();
                    }
                }
                SqlCommandBuilder builder = new SqlCommandBuilder(ss.da);
                ss.da.UpdateCommand = builder.GetUpdateCommand();
                ss.da.DeleteCommand = builder.GetDeleteCommand();
                ss.da.InsertCommand = builder.GetInsertCommand();
                ss.da.Update(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
