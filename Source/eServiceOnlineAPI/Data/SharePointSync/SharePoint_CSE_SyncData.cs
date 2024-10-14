using System;
using Microsoft.SharePoint.Client;
using System.Data;

namespace eServiceOnline.WebAPI.Data.SharePointSync
{
    public class SharePoint_CSE_SyncData
    {
        static bool CSEChanged(ListItem li, DataRow dr)
        {
            bool result = false;

            if (Convert.ToInt32(dr[CSENames.CLIENT_ID] ?? 0) != Convert.ToInt32(li[CSENames.CLIENT_ID]))
                result = true;
            if (Convert.ToString(dr[CSENames.OUTPUT_FOLDER]) != Convert.ToString(li[CSENames.OUTPUT_FOLDER]))
                result = true;
            if (Convert.ToString(dr[CSENames.RIG_NAME]) != Convert.ToString(li[CSENames.RIG_NAME]))
                result = true;
            if (Convert.ToString(dr[CSENames.EMAILS]) != Convert.ToString(li[CSENames.EMAILS]))
                result = true;
            if (Convert.ToString(dr[CSENames.IS_ACTIVE]) != Convert.ToString(li[CSENames.IS_ACTIVE]))
                result = true;
            if (Convert.ToString(dr[CSENames.CLIENT_NAME]) != Convert.ToString(li[CSENames.CLIENT_NAME]))
                result = true;
            if (Convert.ToString(dr[CSENames.SERVICE_LINE]) != Convert.ToString(li[CSENames.SERVICE_LINE]))
                result = true;
            if (Convert.ToString(dr[CSENames.CS_REPRESENTATIVE]) != Convert.ToString(li[CSENames.CS_REPRESENTATIVE]))
                result = true;
            if (Convert.ToString(dr[CSENames.MERGE_FILES]) != Convert.ToString(li[CSENames.MERGE_FILES]))
                result = true;
            if (Convert.ToString(dr[CSENames.WELL_VIEW]) != Convert.ToString(li[CSENames.WELL_VIEW]))
                result = true;
            if ( ((DateTime)dr[CSENames.START_DATE]).ToString("yyyy-MM-dd") != Convert.ToString(li[CSENames.START_DATE]))
                result = true;

            return result;
        }

        static public bool UpdateSharePointCSEItem(ListItem li, DataRow dr)
        {
            bool result = CSEChanged(li, dr);
            /*
            if (result)
            {
                li[PaySummaryNames.Title] = ps.Id;

                li.Update();
            }
            */
            return result;
        }

        static public bool UpdateCSEItem(ListItem li, DataRow dr)
        {
            /*
            bool result = CSEChanged(li, dr);

            if (result)
            {
                dr[CSENames.CLIENT_ID] = Convert.ToInt32(li[CSENames.CLIENT_ID]);
                dr[CSENames.OUTPUT_FOLDER] = Convert.ToString(li[CSENames.OUTPUT_FOLDER]);
                dr[CSENames.RIG_NAME] = Convert.ToString(li[CSENames.RIG_NAME]);
                dr[CSENames.EMAILS] = Convert.ToString(li[CSENames.EMAILS]);
                dr[CSENames.IS_ACTIVE] = Convert.ToString(li[CSENames.IS_ACTIVE]);
                dr[CSENames.CLIENT_NAME] = Convert.ToString(li[CSENames.CLIENT_NAME]);
                dr[CSENames.SERVICE_LINE] = Convert.ToString(li[CSENames.SERVICE_LINE]);
                dr[CSENames.CS_REPRESENTATIVE] = Convert.ToString(li[CSENames.CS_REPRESENTATIVE]);
                dr[CSENames.MERGE_FILES] = Convert.ToString(li[CSENames.MERGE_FILES]);
                dr[CSENames.WELL_VIEW] = Convert.ToString(li[CSENames.WELL_VIEW]);
                dr[CSENames.START_DATE] = Convert.ToString(li[CSENames.START_DATE]);
            }
            */

            bool result = true;

            dr["Id"] = Convert.ToInt32(li[CSENames.SqlRow_Id]);
            dr[CSENames.CLIENT_ID] = Convert.ToInt32(li[CSENames.CLIENT_ID]);
            dr[CSENames.OUTPUT_FOLDER] = Convert.ToString(li[CSENames.OUTPUT_FOLDER]);
            dr[CSENames.RIG_NAME] = Convert.ToString(li[CSENames.RIG_NAME]);
            dr[CSENames.EMAILS] = Convert.ToString(li[CSENames.EMAILS]);
            dr[CSENames.IS_ACTIVE] = Convert.ToString(li[CSENames.IS_ACTIVE]);
            dr[CSENames.CLIENT_NAME] = Convert.ToString(li[CSENames.CLIENT_NAME]);
            dr[CSENames.SERVICE_LINE] = Convert.ToString(li[CSENames.SERVICE_LINE]);
            dr[CSENames.CS_REPRESENTATIVE] = Convert.ToString(li[CSENames.CS_REPRESENTATIVE]);
            dr[CSENames.MERGE_FILES] = Convert.ToString(li[CSENames.MERGE_FILES]);
            dr[CSENames.WELL_VIEW] = Convert.ToString(li[CSENames.WELL_VIEW]);
            dr[CSENames.START_DATE] = Convert.ToString(li[CSENames.START_DATE]);

            return result;
        }

        public class CSENames
        {
            static public string Title = "Title";
            static public string SqlRow_Id = "SqlRow_Id";
            static public string CLIENT_ID = "CLIENT_ID";
            static public string OUTPUT_FOLDER = "OUTPUT_FOLDER";
            static public string RIG_NAME = "RIG_NAME";
            static public string EMAILS = "EMAILS";
            static public string START_DATE = "START_DATE";
            static public string IS_ACTIVE = "IS_ACTIVE";
            static public string CLIENT_NAME = "CLIENT_NAME";
            static public string SERVICE_LINE = "SERVICE_LINE";
            static public string CS_REPRESENTATIVE = "CS_REPRESENTATIVE";
            static public string MERGE_FILES = "MERGE_FILES";
            static public string WELL_VIEW = "WELL_VIEW";
        }
    }
}
