using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Data;
using System.Data.SqlClient;

namespace eServiceOnline.WebAPI.Data.SharePointSync
{
    public class SharePoint_CSE_SyncProcess
    {
        static ClientContext cx_CSE;

        static public void CSExporterRecordToDb(string pCSExporterRecordSharePointId)
        {
            cx_CSE = GetClientContext();

            if (cx_CSE != null)
            {
                Web web = cx_CSE.Web;
                cx_CSE.Load(web);

                List CSEList = web.Lists.GetByTitle("CS_ExtractToClient");
                cx_CSE.Load(CSEList);

                CamlQuery CSEQuery = new CamlQuery();
                CSEQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + pCSExporterRecordSharePointId + "</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
                ListItemCollection CSEListItems = CSEList.GetItems(CSEQuery);
                cx_CSE.Load(CSEListItems);

                //Initialize SharePoint lists and its items
                cx_CSE.ExecuteQuery();

                string spKeyColumnName = "SqlRow_Id"; //CheckColumnName(keyColumnName);

                if (CSEListItems.Count > 0)
                {
                    ListItem li = CSEListItems[0];
                    string strId = !string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])) ? Convert.ToString(li[spKeyColumnName]) : "-1";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = GetDataFromSQL(strId);
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        dt.Rows.Add();
                    }

                    if (SharePoint_CSE_SyncData.UpdateCSEItem(li, dt.Rows[0]))
                    {
                        SqlCommandBuilder builder = new SqlCommandBuilder(da);
                        da.UpdateCommand = builder.GetUpdateCommand();
                        da.DeleteCommand = builder.GetDeleteCommand();
                        da.InsertCommand = builder.GetInsertCommand();
                        da.Update(dt);
                    }

                    da.Dispose();
                }
            }
        }

        static private SqlDataAdapter GetDataFromSQL(string strId)
        {
            string connString = SharePointUtils.GetConnectionString(@"Sanjel25\App", "eservice6");

            string pSqlQuery = "select * from dbo.JOB_TO_CLIENT_CONFIG where Id = " + strId;

            SqlConnection connection = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(pSqlQuery, connection);
            connection.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            return da;
        }

        static public ClientContext GetClientContext()
        {
            if (cx_CSE == null || SharePointUtils.TokenNeedsRefreshing())
            {
                string appContext = ConfigurationManager.AppSettings["applicationContext"];
                string spListUrl = @"https://1961531albertaltd.sharepoint.com/sites/" + (appContext.ToLower() == "production" ? "informationtechnology" : "informationtechnology") + @"/";

                string token = SharePointUtils.GetToken(spListUrl);
                var ctx = new ClientContext(new Uri(spListUrl));
                ctx.ExecutingWebRequest += (sender, e) =>
                {
                    e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + token;
                };

                cx_CSE = ctx;
            }

            return cx_CSE;
        }
    }
}
