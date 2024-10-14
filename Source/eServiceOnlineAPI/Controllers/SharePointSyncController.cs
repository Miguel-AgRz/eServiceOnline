﻿using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using eServiceOnline.WebAPI.Data.SharePointSync;

namespace eServiceOnline.WebAPI.Controllers
{
    //Sample: http://sanjel08/eServiceOnline.SPWebApi/SharePointSync/PushPayEntriesToSharePoint/7b5a790f-b11f-48a7-bd9a-2a285620732a
    //Sample: http://sanjel08/eServiceOnline.SPWebApi/SharePointSync/PushPayPeriodSummaryPaySummaryToSharePoint/0
    //Sample: http://sanjel08/eServiceOnline.SPWebApi/SharePointSync/SyncSharePointVariablePayToDb 
    //Sample: http://sanjel08/eServiceOnline.SPWebApi/SharePointSync/SyncSharePointPayPeriodSummaryToDb/1808     //Sql Id = 22
    //Sample: http://sanjel08/eServiceOnline.SPWebApi/SharePointSync/SyncSharePointCSExporterRecordToDb/175 
    //Sample: http://sanjel08/eServiceOnline.SPWebApi/SharePointSync/PurgePayPeriodFromSharePoint/27

    //Sample: http://localhost:52346/SharePointSync/PushPayEntriesToSharePoint/d6ed1c11-096d-4926-b30e-838d6ce0d2b9 
    //Sample: http://localhost:52346/SharePointSync/PushPayPeriodSummaryPaySummaryToSharePoint/0
    //Sample: http://localhost:52346/SharePointSync/SyncSharePointVariablePayToDb 
    //Sample: http://localhost:52346/SharePointSync/SyncSharePointPayPeriodSummaryToDb/1808     //Sql Id = 22
    //Sample: http://localhost:52346/SharePointSync/SyncSharePointVariablePayToDb 

    //Sample: http://localhost:52346/SharePointSync/JobCountIncentiveToPayEntry/2021-05-01
    //Sample: http://localhost:52346/SharePointSync/JobCountIncentiveToPayEntry/2021-06-01/16

    //Sample: http://localhost:52346/SharePointSync/PurgePayPeriodFromSharePoint/22




    [ApiController]
    public class SharePointSyncController : ControllerBase
    {

        [Route("[controller]/[action]/{jobUniqueId}")]
        public ActionResult PushPayEntriesToSharePoint(string jobUniqueId)
        {
            try
            {
                List<int> uniquePayPeriodIdsInSql = SharePoint_VP_SyncProcess.GetPayPeriodsForJob(jobUniqueId);

                // Refresh all related PaySummaries in SP
                foreach (int payPeriodId in uniquePayPeriodIdsInSql)
                {
                    SharePoint_VP_SyncProcess.PayPeriodSummaryToSharePoint(payPeriodId);

                    SharePoint_VP_SyncProcess.PaySummaryToSharePoint(payPeriodId);
                }

                SharePoint_VP_SyncProcess.PayEntryToSharePoint(jobUniqueId);
            }
            catch (Exception ex)
            {
                return new JsonResult(new {result = false, jobUniqueId, message = ex.Message});
            }

            return new JsonResult(new { result = true, jobUniqueId, message = "Succeed" });
        }

        [Route("[controller]/[action]/{payPeriodId:int}")]
        public ActionResult PurgePayPeriodFromSharePoint(int payPeriodId)
        {
            try
            {
                
                SharePoint_VP_SyncProcess.PurgePayEntryFromSharePoint(payPeriodId);
                SharePoint_VP_SyncProcess.PurgePaySummaryFromSharePoint(payPeriodId);
                SharePoint_VP_SyncProcess.PurgePayPeriodSummaryFromSharePoint(payPeriodId);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, payPeriodId, message = ex.Message });
            }

            return new JsonResult(new { result = true, payPeriodId, message = "Succeed" });
        }

        [Route("[controller]/[action]/{offsetWeeksFromCurrentDay:int}")]
        public ActionResult PushPayPeriodSummaryPaySummaryToSharePoint(int offsetWeeksFromCurrentDay)
        {
            try
            {
                int payPeriodId = SharePoint_VP_SyncProcess.GetPayPeriodId(offsetWeeksFromCurrentDay);
                if (payPeriodId > 0)
                {
                    SharePoint_VP_SyncProcess.PayPeriodSummaryToSharePoint(payPeriodId);
                    SharePoint_VP_SyncProcess.PaySummaryToSharePoint(payPeriodId);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, offsetWeeksFromCurrentDay, message = ex.Message });
            }

            return new JsonResult(new { result = true, offsetWeeksFromCurrentDay, message = "Succeed" });
        }


        //Sample: http://localhost:52346/SharePointSync/SyncSharePointVariablePayToDb 
        [Route("[controller]/[action]")]
        public ActionResult SyncSharePointVariablePayToDb()
        {
            string variablePayRecords = "AllRecordsInSharePointToSqlSync";

            try
            {
                SharePoint_VP_SyncProcess.PayEntriesToDb();
                SharePoint_VP_SyncProcess.PayPeriodSummaryToDb();
                SharePoint_VP_SyncProcess.PaySummaryToDb();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, variablePayRecords, message = ex.Message });
            }

            return new JsonResult(new { result = true, variablePayRecords, message = "Succeed" });
        }

        [Route("[controller]/[action]/{payPeriodSummarySharePointId}")]
        public ActionResult SyncSharePointPayPeriodSummaryToDb(string payPeriodSummarySharePointId)
        {
            try
            {
                SharePoint_VP_SyncProcess.PayPeriodSummaryToDb(payPeriodSummarySharePointId);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, payPeriodSummarySharePointId, message = ex.Message });
            }

            return new JsonResult(new { result = true, payPeriodSummarySharePointId, message = "Succeed" });
        }

        [Route("[controller]/[action]/{monthStartDate}")]
        public ActionResult JobCountIncentiveToPayEntry(string monthStartDate)
        {
            string variablePayRecords = "JobCountIncentiveToPayEntry";

            try
            {
                SharePoint_VP_SyncProcess.JobCountIncentiveToPayEntry(monthStartDate, "999");
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, variablePayRecords, message = ex.Message });
            }

            return new JsonResult(new { result = true, variablePayRecords, message = "Succeed" });
        }

        [Route("[controller]/[action]/{monthStartDate}/{minimalPayPeriodStatusToProceed}")]
        public ActionResult JobCountIncentiveToPayEntry(string monthStartDate, string minimalPayPeriodStatusToProceed)
        {
            string variablePayRecords = "JobCountIncentiveToPayEntry";

            try
            {
                SharePoint_VP_SyncProcess.JobCountIncentiveToPayEntry(monthStartDate, minimalPayPeriodStatusToProceed);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, variablePayRecords, message = ex.Message });
            }

            return new JsonResult(new { result = true, variablePayRecords, message = "Succeed" });
        }

        [Route("[controller]/[action]/{CSExporterRecordSharePointId}")]
        public ActionResult SyncSharePointCSExporterRecordToDb(string CSExporterRecordSharePointId)
        {
            try
            {
                SharePoint_CSE_SyncProcess.CSExporterRecordToDb(CSExporterRecordSharePointId);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, CSExporterRecordSharePointId, message = ex.Message });
            }

            return new JsonResult(new { result = true, CSExporterRecordSharePointId, message = "Succeed" });
        }

        //Sample: http://localhost:52346/SharePointSync/SyncDataBetweenSharePointAndSql/18  --TestingOnly_EmplSpToSql 
        //Sample: http://localhost:52346/SharePointSync/SyncDataBetweenSharePointAndSql/36  --TestingOnly_EmplSqlToSp 
        [Route("[controller]/[action]/{SpList_Config_Id}")]
        public ActionResult SyncDataBetweenSharePointAndSql(int SpList_Config_Id)
        {
            //SpList_Config_Id is ID of configuration record defined in [Sanjel27\DW].[SESI_DW].dbo.SpList_Config table

            try
            {
                SharePoint_SyncData.SqlStuff ss = new SharePoint_SyncData.SqlStuff(SpList_Config_Id);
                SharePoint_SyncData.SpListConfigProfile _config = ss.ConfigProfile;

                try
                {
                    if (!String.IsNullOrEmpty(_config.ErrorMessage))
                        throw new Exception("\nException Message: " + _config.ErrorMessage + "\n\n" + _config.errorParameters);

                    if (_config.SpListAction == SharePoint_SyncData.SpListConfigProfile.ProcessingAction.Delete)
                        SharePoint_SyncData.SpStuff.DeleteSpList(_config);

                    else if (_config.DataFlowDirection == SharePoint_SyncData.SpListConfigProfile.ProcessingDirection.SqlToSp)
                        SharePoint_SyncProcess.UpdateDbToSp(ss);

                    else if (_config.DataFlowDirection == SharePoint_SyncData.SpListConfigProfile.ProcessingDirection.AddFromStoredProcedure)
                        SharePoint_SyncProcess.UpdateDbToSp(ss);

                    else if (_config.DataFlowDirection == SharePoint_SyncData.SpListConfigProfile.ProcessingDirection.SpToSql)
                        SharePoint_SyncProcess.UpdateSpToDb(ss);

                    else
                        throw new Exception("\n" + System.String.Format("Error : Unknown /Action parameter!"));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                }
                finally
                {
                    if (ss.connection != null && ss.connection.State == ConnectionState.Open)
                        ss.connection.Close();
                    if (ss.da != null)
                        ss.da.Dispose();
                }

                //SharePoint_SyncData.SpListConfigProfile.LogMessageToFile(System.String.Format("End processing   : '{0}' / '{1}' / '{2}'", _config.SharePointListName, _config.SpListAction.ToString(), _config.ConvertAllFieldsToText.ToString()));

            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, SpList_Config_Id, message = Environment.NewLine + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace.ToString() });
            }

            return new JsonResult(new { result = true, SpList_Config_Id, message = "Succeed" });
        }


        /*

        [Route("[controller]/[action]/{payPeriodId:int}")]
        public ActionResult PushPaySummaryToSharePoint(int payPeriodId)
        {
            try
            {
                SharePointSyncProcess.PaySummaryToSharePoint(payPeriodId);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, payPeriodId, message = ex.Message });
            }

            return new JsonResult(new { result = true, payPeriodId, message = "Succeed" });
        }

        [Route("[controller]/[action]/{paySummarySharePointId:int}")]
        public ActionResult SyncSharePointPaySummaryToDb(int paySummarySharePointId)
        {
            try
            {
                SharePointSyncProcess.PaySummaryToDb(paySummarySharePointId);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, paySummarySharePointId, message = ex.Message });
            }

            return new JsonResult(new { result = true, paySummarySharePointId, message = "Succeed" });
        }

        [Route("[controller]/[action]/{payPeridSummarySharePointId:int}")]
        public ActionResult SyncSharePointPayEntriesToDb(string payPeridSummarySharePointId)
        {
            try
            {
                SharePointSyncProcess.PayEntriesToDb(payPeridSummarySharePointId);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, payPeridSummarySharePointId, message = ex.Message });
            }

            return new JsonResult(new { result = true, payPeridSummarySharePointId, message = "Succeed" });

        }
        */
    }
}