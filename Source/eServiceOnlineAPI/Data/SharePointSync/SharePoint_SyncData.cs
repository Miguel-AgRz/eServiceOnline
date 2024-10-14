using System;
using Microsoft.SharePoint.Client;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eServiceOnline.WebAPI.Data.SharePointSync
{
    public class SharePoint_SyncData
    {

        public class SqlStuff
        {
            public SqlConnection connection { get; set; }
            public SqlDataAdapter da { get; set; }

            static string idPrefix = "SqlRow_";

            public SpListConfigProfile ConfigProfile { get; set; }

            public SqlStuff(int spList_Config_Id)
            {
                ConfigProfile = new SpListConfigProfile(spList_Config_Id);
            }

            public DataTable GetDataFromSQL(SpListConfigProfile pConfig, bool keepConnectionOpen)
            {
                DataTable dataTable = new DataTable();

                //string connString = "Persist Security Info=False;Integrated Security=true;Initial Catalog=" + pConfig.SqlDbName + ";server=" + pConfig.SqlServerName;
                //string connString = "Persist Security Info=False;Integrated Security=true;Initial Catalog=SESI_DW;server=Sanjel\DW";
                string connString = SharePointUtils.GetConnectionString(pConfig.SqlServerName, pConfig.SqlDbName);

                connection = new SqlConnection(connString);

                SqlCommand cmd;
                if (pConfig.SpListAction == SpListConfigProfile.ProcessingAction.AddFromStoredProcedure)
                {
                    string[] sp = pConfig.SqlQuery.Split(' ');

                    string sp_name = sp[0].ToString();
                    int sp_param = int.Parse(sp[1].ToString().Substring(sp[1].ToString().Length - 1));

                    cmd = new SqlCommand(sp_name, connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(@"@MarksRecordsAsProcessed", sp_param);
                }
                else
                {
                    cmd = new SqlCommand(pConfig.SqlQuery, connection);
                }

                cmd.CommandTimeout = 600;

                connection.Open();

                da = new SqlDataAdapter(cmd);
                da.Fill(dataTable);

                if (!keepConnectionOpen)
                {
                    connection.Close();
                    da.Dispose();
                }

                return dataTable;
            }

            static public string CheckColumnName(string columnName)
            {
                return (columnName == "Id" || columnName == "Title") ? idPrefix + columnName : columnName;
            }

            public static DataRow GetRowByKey(DataTable dt, string columnName, string columnValue)
            {
                System.TypeCode tp = Type.GetTypeCode(dt.Columns[columnName].DataType);
                DataRow dr = null;

                switch (tp)
                {
                    case TypeCode.Boolean:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<bool>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Byte:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<byte>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Char:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<char>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.DateTime:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<DateTime>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.DBNull:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<DBNull>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Decimal:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<decimal>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Double:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<double>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Empty:
                        dr = null; // dt.AsEnumerable().Where(myRow => myRow.Field< null > (columnName).ToString() == li[keyColNm].ToString()).FirstOrDefault();
                        break;
                    case TypeCode.Int16:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<short>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Int32:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<int>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Int64:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<long>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Object:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<object>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.SByte:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<sbyte>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.Single:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<Single>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.String:
                        dr = dt.AsEnumerable().Where(myRow => (myRow.Field<string>(columnName) ?? "").ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.UInt16:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<UInt16>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.UInt32:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<UInt32>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    case TypeCode.UInt64:
                        dr = dt.AsEnumerable().Where(myRow => myRow.Field<UInt64>(columnName).ToString() == columnValue).FirstOrDefault();
                        break;
                    default:
                        break;
                }
                return dr;
            }

        }

        public class SpStuff
        {
            static public bool ProcessListExitenceAndStructure(DataTable dt, SpListConfigProfile pConfig)
            {
                bool result = false;

                if (!CheckListExists(pConfig))
                {
                    try
                    {
                        using (ClientContext context = GetClientContext(pConfig))
                        {
                            Web web = context.Web;

                            ListCreationInformation creationInfo = new ListCreationInformation();
                            creationInfo.Title = pConfig.SharePointListName;
                            creationInfo.TemplateType = (int)ListTemplateType.CustomGrid;
                            List list = web.Lists.Add(creationInfo);
                            list.Description = pConfig.SharePointListName;

                            list.Update();
                            context.ExecuteQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                if (CheckListExists(pConfig))
                {
                    using (var clientContext = GetClientContext(pConfig))
                    {
                        try
                        {
                            List list = clientContext.Web.Lists.GetByTitle(pConfig.SharePointListName);
                            clientContext.Load(list.Fields);
                            clientContext.ExecuteQuery();

                            System.Collections.Generic.List<string> fields = new System.Collections.Generic.List<string>();

                            foreach (Field field in list.Fields)
                            {
                                fields.Add(field.InternalName);
                            }

                            foreach (DataColumn dc in dt.Columns)
                            {
                                string nm = SqlStuff.CheckColumnName(dc.ColumnName);

                                if (!fields.Contains(nm))
                                {
                                    var tp = GetColumnDataType(dc, pConfig.ConvertAllFieldsToText);

                                    Field oField;

                                    if (!string.IsNullOrEmpty(nm) && !string.IsNullOrEmpty(tp))
                                    {
                                        oField = list.Fields.AddFieldAsXml("<Field DisplayName='" + nm + "' Name='" + nm + "' Type='" + tp + "' />", true, AddFieldOptions.DefaultValue);

                                        switch ("Field" + tp)
                                        {
                                            case "FieldNumber":
                                                FieldNumber fieldNumber = clientContext.CastTo<FieldNumber>(oField);
                                                //fieldNumber.MaximumValue = 10000000;
                                                //fieldNumber.MinimumValue = 0;
                                                fieldNumber.Update();
                                                break;
                                            case "FieldCurrency":
                                                FieldCurrency fieldCurrency = clientContext.CastTo<FieldCurrency>(oField);
                                                fieldCurrency.Update();
                                                break;
                                            case "FieldDateTime":
                                                FieldDateTime fieldDateTime = clientContext.CastTo<FieldDateTime>(oField);
                                                fieldDateTime.Update();
                                                break;
                                            case "FieldText":
                                                FieldText fieldText = clientContext.CastTo<FieldText>(oField);
                                                fieldText.Update();
                                                break;
                                            case "FieldBoolean":
                                                break;
                                            case "FieldCalculated":
                                            case "FieldChoice":
                                            case "FieldComputed":
                                            case "FieldGeolocation":
                                            case "FieldGuid":
                                            case "FieldLink":
                                            case "FieldLookup":
                                            case "FieldMultiChoice":
                                            case "FieldMultiLineText":
                                            case "FieldRatingScale":
                                            case "FieldUrl":
                                            case "FieldUser":
                                            default:
                                                oField.Update();
                                                break;
                                        }
                                    }
                                }
                            }
                            list.Update();
                            clientContext.ExecuteQuery();

                            result = true;
                        }
                        catch
                        {
                            result = false;
                        }
                    }
                }

                return result;
            }

            static bool CheckListExists(SpListConfigProfile ss)
            {
                bool result = true;
                using (var clientContext = GetClientContext(ss))
                {
                    try
                    {
                        List list = clientContext.Web.Lists.GetByTitle(ss.SharePointListName);
                        clientContext.Load(list.Fields);
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        //throw ex;
                    }
                }
                return result;
            }

            static public bool DeleteSpList(SpListConfigProfile ss)
            {
                bool result = true;
                using (var clientContext = SpStuff.GetClientContext(ss))
                {
                    try
                    {
                        List productList = clientContext.Web.Lists.GetByTitle(ss.SharePointListName);
                        productList.DeleteObject();
                        clientContext.ExecuteQuery();
                    }
                    catch { result = false; }
                }

                return result;
            }

            static public void AddSpListField(string fieldName, string fieldType, SpListConfigProfile pConfig)
            {
                using (var clientContext = GetClientContext(pConfig))
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web);
                    List list = web.Lists.GetByTitle(pConfig.SharePointListName);

                    Field oField;
                    oField = list.Fields.AddFieldAsXml("<Field DisplayName='" + fieldName + "' Type='" + fieldType + "' />", true, AddFieldOptions.DefaultValue);
                    oField.Update();
                    list.Update();
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                }
            }

            public static ClientContext GetClientContext(SpListConfigProfile ss)
            {
                ClientContext ctx = null;

                try
                {
                    string token = SharePointUtils.GetToken(ss.SharePointUrl);
                    ctx = new ClientContext(new Uri(ss.SharePointUrl));
                    ctx.ExecutingWebRequest += (sender, e) =>
                    {
                        e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + token;
                    };
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return ctx;
            }

            static public string GetColumnDataType(DataColumn dc, bool convertFieldsToText)
            {
                var tp = "";

                switch (Type.GetTypeCode(dc.DataType))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        tp = convertFieldsToText ? "Text" : "Number";
                        break;
                    case TypeCode.DateTime:
                        tp = convertFieldsToText ? "Text" : "DateTime";
                        break;
                    case TypeCode.Boolean:
                        tp = convertFieldsToText ? "Text" : "Boolean";
                        break;
                    case TypeCode.String:
                        tp = "Text";
                        break;
                    case TypeCode.Empty:
                        break;
                    default:    // TypeCode.DBNull, TypeCode.Char and TypeCode.Object
                        break;
                }

                return tp;
            }

        }

        public class SpListConfigProfile
        {
            public string errorParameters = "\n" +
                    "********************************************************************************************** \n" +
                    "* SharePointLists.exe parameters should indicate :\n" +
                    "*     /SpListName:[ SharePointListName ] \n" +
                    "*     /Action:[ Create | Delete | SqlToSp | SpToSql | AddFromStoredProcedure ] \n" +
                    "*     /ConvertSpToText:[ true | false | 1 | 0 ] \n" +
                    "* SharePointListName - a name of a SharePoint list that has a configuration record in the \n" +
                    "*                      'SESI_DW.dbo.SpList_Config' table.\n" +
                    "* Action - a name of processing mode (Create / Delete / SqlToSp / SpToSql) \n" +
                    "*     Create  - deletes existing list (if any) and creates a new one \n" +
                    "*               with fields that match SQl query output\n" +
                    "*     Delete  - deletes existing list (if any) \n" +
                    "*     SqlToSp - creates a new list if it does not exist, creates/adds to the list all missing \n" +
                    "*               fields from SQL query output, updates data in the list \n" +
                    "*               (updates existing records, create new ones, deletes orphaned) \n" +
                    "*     SpToSql - updates an SQL table data from a list for matching fields \n" +
                    "*               (updates existing records, create new ones, deletes orphaned) \n" +
                    "*     AddFromStoredProcedure - creates a new list if it does not exist, creates/adds to the list all missing \n" +
                    "*               fields from SQL output, just dumps all records to the list \n" +
                    "*               (only create new ones - no updating existing records, no deleting orphaned, no duplicate check). \n" +
                    "*               Expected data source is a stored procedure that returns records to be added  \n" +
                    "*               and might also set internally a processed flag for all records returned. \n" +
                    "*               A stored procedure should take only one parameter : @MarksRecordsAsProcessed = [0 | 1] . \n" +
                    "* ConvertSpToText - flag to indicate if SharePoint list properties should be all created as Text \n" +
                    "*                   ( Default : true, has effect only for Create and SqlToSp actions ) \n" +
                    "* \n" +
                    "* I.e. : SharePointLists.exe /SpListName:TestList /Action:SqlToSp /ConvertSpToText:false \n" +
                    "* \n" +
                    "********************************************************************************************** \n";

            public string SharePointUrl { get; set; }
            public string SharePointListName { get; set; }
            public string SqlServerName { get; set; }
            public string SqlDbName { get; set; }
            public string SqlQuery { get; set; }
            public string SqlKeyColumnName { get; set; }
            public bool ConvertAllFieldsToText { get; set; }
            public string CheckSpOrphanedWithinColumnName { get; set; }
            public ProcessingDirection DataFlowDirection { get; set; }
            public ProcessingAction SpListAction { get; set; }
            public string ErrorMessage { get; set; }

            public enum ProcessingDirection
            {
                SqlToSp,
                SpToSql,
                AddFromStoredProcedure,
                Unknown
            }

            public enum ProcessingAction
            {
                Create,
                Delete,
                Update,
                AddFromStoredProcedure,
                Unknown
            }

            private SpListConfigProfile() { }

            public SpListConfigProfile(int pSpList_Config_Id)
            {
                string connString = SharePointUtils.GetConnectionString(@"sanjel27\DW", "SESI_DW");
                string confQuery = "select top 1 * from SESI_DW.dbo.SpList_Config where Id = " + pSpList_Config_Id.ToString();

                SqlConnection connection = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(confQuery, connection);
                connection.Open();

                SqlDataAdapter configDA = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                configDA.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow dr = dataTable.Rows[0];

                    string _spListName = dr["SpListName"].ToString();
                    string _spUrl = dr["SpUrl"].ToString();
                    string _sqlKeyColumnName = dr["KeyColumnName"].ToString();
                    string _sqlServerName = dr["SqlServerName"].ToString();
                    string _sqlDbName = dr["SqlDbName"].ToString();
                    string _sqlQuery = dr["SqlQuery"].ToString();
                    string _checkSpOrphanedWithinColumnName = dr["CheckSpOrphanedWithinColumnName"].ToString();
                    string _convertAllFieldsToText = dr["ConvertAllSpFieldsToText"].ToString();
                    string _direction = dr["Direction"].ToString();

                    SharePointUrl = _spUrl;
                    SharePointListName = _spListName;
                    SqlServerName = _sqlServerName;
                    SqlDbName = _sqlDbName;
                    SqlQuery = _sqlQuery;
                    SqlKeyColumnName = _sqlKeyColumnName;
                    ConvertAllFieldsToText = (_convertAllFieldsToText.ToLower().Trim() == "yes" || _convertAllFieldsToText.ToLower().Trim() == "true" || _convertAllFieldsToText.ToLower().Trim() == "1") ? true : false;
                    CheckSpOrphanedWithinColumnName = _checkSpOrphanedWithinColumnName;
                    DataFlowDirection = GetSpListDirection(_direction);
                    SpListAction = GetSpListAction(_direction);
                }

                if (connection.State == ConnectionState.Open)
                    connection.Close();
                configDA.Dispose();
            }

            ProcessingAction GetSpListAction(string pSyncMode)
            {
                ProcessingAction action = ProcessingAction.Unknown;

                switch (pSyncMode.ToLower())
                {
                    case "createsp":
                    case "create":
                    case "c":
                        action = ProcessingAction.Create;
                        break;
                    case "deletesp":
                    case "delete":
                    case "del":
                    case "d":
                        action = ProcessingAction.Delete;
                        break;
                    case "sp":
                    case "tosp":
                    case "sqltosp":
                    case "sql":
                    case "tosql":
                    case "sptosql":
                    case "fromsp":
                    case "updatesp":
                    case "updatesql":
                        action = ProcessingAction.Update;
                        break;
                    case "addfromstoredprocedure":
                        action = ProcessingAction.AddFromStoredProcedure;
                        break;
                    default:
                        break;
                }
                return action;
            }

            ProcessingDirection GetSpListDirection(string pSyncMode)
            {
                ProcessingDirection direction = ProcessingDirection.Unknown;

                switch (pSyncMode.ToLower())
                {
                    case "createsp":
                    case "create":
                    case "c":

                    case "deletesp":
                    case "delete":
                    case "del":
                    case "d":

                    case "sp":
                    case "tosp":
                    case "sqltosp":
                    case "updatesp":
                        direction = ProcessingDirection.SqlToSp;
                        break;

                    case "sql":
                    case "tosql":
                    case "sptosql":
                    case "fromsp":
                    case "updatesql":
                        direction = ProcessingDirection.SpToSql;
                        break;
                    case "addfromstoredprocedure":
                        direction = ProcessingDirection.AddFromStoredProcedure;
                        break;

                    default:
                        break;
                }
                return direction;
            }
        }
    }
}
