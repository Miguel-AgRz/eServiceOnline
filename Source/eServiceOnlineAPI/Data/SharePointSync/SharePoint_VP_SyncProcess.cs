using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using System.Configuration;

using MetaShare.Common.Core.CommonService;
using Sesi.SanjelData.Entities.BusinessEntities.VariablePay;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.VariablePay;

using System.Data;
using System.Data.SqlClient;

namespace eServiceOnline.WebAPI.Data.SharePointSync
{
    public class SharePoint_VP_SyncProcess
    {
        static public ClientContext cx;

        static public void PayEntryToSharePoint(string jobUniqueId)
        {
            IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();
            var existingPayEntries = payEntryService.SelectBy(new PayEntry() { JobUniqueId = jobUniqueId }, new List<string>() { "JobUniqueId" });

            List<int> uniquePayEntryIdsInSql = new List<int>();
            Dictionary<int, int> payPeriodSummarySqlToSpMapping = GetPeriodSummarySqlToSpMapping();

            cx = GetClientContext(false);

            if (cx != null)
            {
                string spKeyColumnName = "SqlRow_Id"; //CheckColumnName(keyColumnName);

                Web web = cx.Web;
                cx.Load(web);

                //Prepare SharePoint VP_PayEntry list data for a JobUniqueId
                List payEntryList = web.Lists.GetByTitle("VP_PayEntry");
                cx.Load(payEntryList);
                CamlQuery payEntryQuery = new CamlQuery();
                payEntryQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='JobUniqueId'/><Value Type='Text'>" + jobUniqueId + "</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
                ListItemCollection payEntryListItems = payEntryList.GetItems(payEntryQuery);
                cx.Load(payEntryListItems);

                //Initialize SharePoint lists and its items
                cx.ExecuteQueryRetry();

                // For each SQL record Add / Update an item in SP list 
                foreach (PayEntry existingPayEntry in existingPayEntries)
                {
                    uniquePayEntryIdsInSql.Add(existingPayEntry.Id);

                    bool isNewItem = true;

                    foreach (ListItem li in payEntryListItems)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])) && Convert.ToInt32(li[spKeyColumnName]) == existingPayEntry.Id)
                        {
                            isNewItem = false;
                            SharePoint_VP_SyncData.UpdateSharePointPayEntryItem(li, existingPayEntry, payPeriodSummarySqlToSpMapping, false);
                            break;
                        }
                    }

                    if (isNewItem)
                    {
                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem newItem = payEntryList.AddItem(itemCreateInfo);

                        SharePoint_VP_SyncData.UpdateSharePointPayEntryItem(newItem, existingPayEntry, payPeriodSummarySqlToSpMapping, true);
                        cx.Load(newItem);
                    }
                }

                // Delete SP list items not having a match in SQL records
                for (int i = payEntryListItems.Count - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(payEntryListItems[i][spKeyColumnName])) && !uniquePayEntryIdsInSql.Contains(Convert.ToInt32(payEntryListItems[i][spKeyColumnName])))
                        payEntryListItems[i].DeleteObject();
                }

                cx.Load(payEntryList);
                cx.ExecuteQueryRetry();
            }
        }

        static public void PaySummaryToSharePoint(int payPeriodId)
        {
            IPaySummaryService paySummaryService = ServiceFactory.Instance.GetService<IPaySummaryService>();
            var existingPaySummaries = paySummaryService.SelectByPayPeriod(payPeriodId);

            Dictionary<int, int> payPeriodSummarySqlToSpMapping = GetPeriodSummarySqlToSpMapping();

            cx = GetClientContext(false);

            if (cx != null)
            {
                Web web = cx.Web;
                cx.Load(web);

                //Prepare SharePoint VP_PayEntry list data for a JobUniqueId
                List paySummaryList = web.Lists.GetByTitle("VP_PaySummary");
                cx.Load(paySummaryList);
                CamlQuery payPaySummaryQuery = new CamlQuery();
                payPaySummaryQuery.ViewXml = "<View><Query><Where>" +
                    "<Eq><FieldRef Name='PayPeriodId'/><Value Type='Integer'>" + payPeriodId + "</Value></Eq>" +
                    "</Where></Query><RowLimit>500</RowLimit></View>";
                ListItemCollection paySummaryListItems = paySummaryList.GetItems(payPaySummaryQuery);
                cx.Load(paySummaryListItems);

                cx.ExecuteQueryRetry();

                // For each SQL record Add / Update an item in SP list 
                foreach (PaySummary existingPaySummary in existingPaySummaries)
                {
                    bool isNewItem = true;

                    foreach (ListItem li in paySummaryListItems)
                    {
                        if (Convert.ToInt32(li["PayPeriodId"]) == existingPaySummary.PayPeriod.Id && Convert.ToInt32(li["EmployeeId"]) == existingPaySummary.Employee.Id)
                        {
                            isNewItem = false;
                            //UpdateSarePointPaySummaryItem(li, existingPaySummary, payPeriodSummarySqlToSpMapping, false);
                            break;
                        }
                    }

                    if (isNewItem)
                    {
                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem newListItem = paySummaryList.AddItem(itemCreateInfo);

                        SharePoint_VP_SyncData.UpdateSharePointPaySummaryItem(newListItem, existingPaySummary, payPeriodSummarySqlToSpMapping, true);
                        cx.Load(newListItem);
                    }
                }

                cx.Load(paySummaryList);
                cx.ExecuteQueryRetry();
            }
        }

        static public void PayPeriodSummaryToSharePoint(int payPeriodId)
        {
            IPayPeriodSummaryService payPeriodSummaryService = ServiceFactory.Instance.GetService<IPayPeriodSummaryService>();
            var existingPayPeriodSummaries = payPeriodSummaryService.SelectByPayPeriod(payPeriodId);

            cx = GetClientContext(false);

            if (cx != null)
            {
                string spKeyColumnName = "SqlRow_Id"; //CheckColumnName(keyColumnName);
                Web web = cx.Web;
                cx.Load(web);

                //Prepare SharePoint VP_PayEntry list data for a JobUniqueId
                List payPeriodSummaryList = web.Lists.GetByTitle("VP_PayPeriodSummary");
                cx.Load(payPeriodSummaryList);
                CamlQuery payPayPeriodSummaryQuery = new CamlQuery();
                payPayPeriodSummaryQuery.ViewXml = "<View><Query><Where>" +
                    "<Eq><FieldRef Name='PayPeriodid'/><Value Type='Integer'>" + payPeriodId + "</Value></Eq>" +
                    "</Where></Query><RowLimit>500</RowLimit></View>";
                ListItemCollection payPeriodSummaryListItems = payPeriodSummaryList.GetItems(payPayPeriodSummaryQuery);
                cx.Load(payPeriodSummaryListItems);

                cx.ExecuteQueryRetry();

                // For each SQL record Add / Update an item in SP list 
                foreach (PayPeriodSummary existingPayPeriodSummary in existingPayPeriodSummaries)
                {
                    bool isNewItem = true;

                    foreach (ListItem li in payPeriodSummaryListItems)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])) && Convert.ToInt32(li[spKeyColumnName]) == existingPayPeriodSummary.Id)
                        {
                            isNewItem = false;
                            //SharePointSyncData.UpdateSharePointPayPeriodSummaryItem(li, existingPayPeriodSummary, false);
                            break;
                        }
                    }

                    if (isNewItem)
                    {
                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem newListItem = payPeriodSummaryList.AddItem(itemCreateInfo);

                        SharePoint_VP_SyncData.UpdateSharePointPayPeriodSummaryItem(newListItem, existingPayPeriodSummary, true);
                        cx.Load(newListItem);
                    }
                }

                cx.Load(payPeriodSummaryList);
                cx.ExecuteQueryRetry();
            }
        }

        static public bool PayPeriodSummaryToDb()
        {
            bool result = true;
            int rowsPerPage = 2000;
            string spKeyColumnName = SharePoint_VP_SyncData.PayPeriodSummaryNames.SqlRow_Id;

            IPayPeriodSummaryService payPeriodSummaryService = ServiceFactory.Instance.GetService<IPayPeriodSummaryService>();

            using (ClientContext ccx = GetClientContext())
            {
                if (ccx != null)
                {
                    List<ListItem> allListItems = new List<ListItem>();

                    List payPeriodSummaryList = ccx.Web.Lists.GetByTitle("VP_PayPeriodSummary");
                    ccx.Load(payPeriodSummaryList);
                    ccx.ExecuteQueryRetry();

                    CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);
                    do
                    {
                        ListItemCollection payPeriodSummaryListItems = payPeriodSummaryList.GetItems(camlQuery);
                        ccx.Load(payPeriodSummaryListItems);
                        ccx.ExecuteQueryRetry();

                        allListItems.AddRange(payPeriodSummaryListItems);
                        camlQuery.ListItemCollectionPosition = payPeriodSummaryListItems.ListItemCollectionPosition;

                    } while (camlQuery.ListItemCollectionPosition != null);

                    List<int> ppsDistinctPayPeriods = allListItems.Select(s => Convert.ToInt32(s[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid])).Distinct().ToList();
                    foreach (int ppId in ppsDistinctPayPeriods)
                    {
                        List<ListItem> filteredListItems = allListItems.Where(item => int.Parse(item.FieldValues[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid].ToString()) == ppId).ToList();
                        List<PayPeriodSummary> ppsList = payPeriodSummaryService.SelectByPayPeriod(ppId);

                        foreach (ListItem li in filteredListItems)
                        {
                            PayPeriodSummary pps = null;
                            if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                pps = ppsList.FirstOrDefault(p => p.Id == Convert.ToInt32(li[spKeyColumnName]));

                            if (pps != null && pps.Id > 0)
                            {
                                if (SharePoint_VP_SyncData.UpdatePayPeriodSummaryItem(li, pps, false))
                                    payPeriodSummaryService.Update(pps);
                            }
                            else
                            {
                                pps = new PayPeriodSummary();
                                SharePoint_VP_SyncData.UpdatePayPeriodSummaryItem(li, pps, true);
                                payPeriodSummaryService.Insert(pps);
                                li[spKeyColumnName] = pps.Id;
                                li.Update();
                            }
                        }

                        ccx.ExecuteQueryRetry();

                        List<int> payPeriodSummaryIdsinSp = filteredListItems.Select(s => Convert.ToInt32(s[spKeyColumnName])).Distinct().ToList();
                        foreach (PayPeriodSummary pps in ppsList)
                        {
                            if (!payPeriodSummaryIdsinSp.Contains(pps.Id))
                                //throw new Exception(String.Format("SQL side PayPeriodSummary with Id  = {0} was not found in the SharePoint VP_PayPeriodSummary list!", pps.Id.ToString()));
                                payPeriodSummaryService.Delete(pps);
                        }
                    }
                    ccx.Load(payPeriodSummaryList);
                    ccx.ExecuteQueryRetry();
                }
            }
            return result;
        }

        static public bool PaySummaryToDb()
        {
            bool result = true;
            int rowsPerPage = 2000;
            string spKeyColumnName = SharePoint_VP_SyncData.PaySummaryNames.SqlRow_Id;

            IPaySummaryService paySummaryService = ServiceFactory.Instance.GetService<IPaySummaryService>();
            Dictionary<int, int> payPeriodSummarySpToSqlMapping = GetPeriodSummarySpToSqlMapping();

            using (ClientContext ccx = GetClientContext())
            {
                if (ccx != null)
                {
                    List<ListItem> allListItems = new List<ListItem>();

                    List paySummaryList = ccx.Web.Lists.GetByTitle("VP_PaySummary");
                    ccx.Load(paySummaryList);
                    ccx.ExecuteQueryRetry();

                    CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);

                    do
                    {
                        ListItemCollection paySummaryListItems = paySummaryList.GetItems(camlQuery);
                        ccx.Load(paySummaryListItems);
                        ccx.ExecuteQueryRetry();

                        allListItems.AddRange(paySummaryListItems);
                        camlQuery.ListItemCollectionPosition = paySummaryListItems.ListItemCollectionPosition;

                    } while (camlQuery.ListItemCollectionPosition != null);

                    List<int> psDistinctPayPeriods = allListItems.Select(s => Convert.ToInt32(s[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId])).Distinct().ToList();
                    foreach (int ppId in psDistinctPayPeriods)
                    {
                        List<ListItem> filteredListItems = allListItems.Where(item => int.Parse(item.FieldValues[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId].ToString()) == ppId).ToList();
                        List<PaySummary> psList = paySummaryService.SelectByPayPeriod(ppId);

                        foreach (ListItem li in filteredListItems)
                        {
                            PaySummary ps = null;

                            if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                ps = psList.FirstOrDefault(p => p.Id == Convert.ToInt32(li[spKeyColumnName]));

                            if (ps != null && ps.Id > 0)
                            {
                                if (SharePoint_VP_SyncData.UpdatePaySummaryItem(li, ps, false, payPeriodSummarySpToSqlMapping))
                                    paySummaryService.Update(ps);
                            }
                            else
                            {
                                ps = new PaySummary();
                                SharePoint_VP_SyncData.UpdatePaySummaryItem(li, ps, true, payPeriodSummarySpToSqlMapping);
                                paySummaryService.Insert(ps);
                                li[spKeyColumnName] = ps.Id;
                                li.Update();
                            }
                        }

                        ccx.ExecuteQueryRetry();

                        List<int> paySummaryIdsinSp = filteredListItems.Select(s => Convert.ToInt32(s[spKeyColumnName])).Distinct().ToList();
                        foreach (PaySummary ps in psList)
                        {
                            if (!paySummaryIdsinSp.Contains(ps.Id))
                                //throw new Exception(String.Format("SQL side PaySummary with Id  = {0} was not found in the SharePoint VP_PaySummary list!", ps.Id.ToString()));
                                paySummaryService.Delete(ps);
                        }
                    }
                    ccx.Load(paySummaryList);
                    ccx.ExecuteQueryRetry();
                }
            }
            return result;
        }

        static public bool PayEntriesToDb()
        {
            bool result = true;
            int rowsPerPage = 2000;
            int payEntriesCountThresholdToDeleteFromSQLWhatNotInSp = 0;
            string spKeyColumnName = SharePoint_VP_SyncData.PayEntryNames.SqlRow_Id;

            IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();
            Dictionary<int, int> payPeriodSummarySpToSqlMapping = GetPeriodSummarySpToSqlMapping();

            using (ClientContext ccx = GetClientContext())
            {
                if (ccx != null)
                {
                    List<ListItem> allListItems = new List<ListItem>();

                    List payEntryList = ccx.Web.Lists.GetByTitle("VP_PayEntry");
                    ccx.Load(payEntryList);
                    ccx.ExecuteQueryRetry();

                    CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);
                    do
                    {
                        ListItemCollection payEntryListItems = payEntryList.GetItems(camlQuery);
                        ccx.Load(payEntryListItems);
                        ccx.ExecuteQueryRetry();
                        allListItems.AddRange(payEntryListItems);
                        camlQuery.ListItemCollectionPosition = payEntryListItems.ListItemCollectionPosition;
                    } while (camlQuery.ListItemCollectionPosition != null);

                    List<int> ppDistinctPayPeriods = allListItems.Select(s => Convert.ToInt32(s[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid])).Distinct().ToList();

                    foreach (int ppId in ppDistinctPayPeriods)
                    {
                        List<ListItem> filteredListItems = allListItems.Where(item => int.Parse(item.FieldValues[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid].ToString()) == ppId).ToList();
                        List<PayEntry> peList = payEntryService.SelectByPayPeriod(ppId);

                        foreach (ListItem li in filteredListItems)
                        {
                            PayEntry pe = null;

                            //Try to find existing matching pay entry in DB
                            if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                pe = peList.FirstOrDefault(p => p.Id == Convert.ToInt32(li[spKeyColumnName]));

                            //Found in DB -> update
                            if (pe != null && pe.Id > 0)
                            {
                                //Try to update all properties in DB first
                                if (SharePoint_VP_SyncData.UpdatePayEntryItem(li, pe, false, payPeriodSummarySpToSqlMapping))
                                    payEntryService.Update(pe);

                                //Mark as deleted in DB if Deleted flag is set in SP
                                if (pe != null && SharePoint_VP_SyncData.PayEntryDeleted(li))
                                    payEntryService.Delete(pe);
                            }
                            //Not found in DB and not marked as deleted - add new to DB
                            else if (!SharePoint_VP_SyncData.PayEntryDeleted(li))
                            {
                                pe = new PayEntry();
                                SharePoint_VP_SyncData.UpdatePayEntryItem(li, pe, true, payPeriodSummarySpToSqlMapping);
                                payEntryService.Insert(pe);
                                li[spKeyColumnName] = pe.Id;
                                li.Update();

                                peList.Add(pe);
                            }
                        }
                        //ccx.Load(payEntryList);
                        ccx.ExecuteQueryRetry();

                        List<int> payEntryIdsinSp = filteredListItems.Select(s => Convert.ToInt32(s[spKeyColumnName])).Distinct().ToList();
                        if (payEntryIdsinSp.Count > payEntriesCountThresholdToDeleteFromSQLWhatNotInSp)
                        {
                            foreach (PayEntry pe in peList)
                            {
                                if (!payEntryIdsinSp.Contains(pe.Id))
                                {
                                    if (ConfigurationManager.AppSettings["applicationContext"].ToString().ToLower() == "production")
                                        throw new Exception(String.Format("SQL side PayEntry with Id  = {0} was not found in the SharePoint VP_PayEntry list!", pe.Id.ToString()));
                                    else
                                        payEntryService.Delete(pe);
                                }
                            }
                        }
                    }
                    ccx.Load(payEntryList);
                    ccx.ExecuteQueryRetry();
                }
            }

            return result;
        }

        static public void PayPeriodSummaryToDb(string payPeridSummarySharePointId)
        {
            IPayPeriodSummaryService payPeriodSummaryService = ServiceFactory.Instance.GetService<IPayPeriodSummaryService>();

            cx = GetClientContext(false);

            if (cx != null)
            {
                Web web = cx.Web;
                cx.Load(web);

                //Prepare SharePoint VP_PayEntry list data for a JobUniqueId
                List payPeriodSummaryList = web.Lists.GetByTitle("VP_PayPeriodSummary");
                cx.Load(payPeriodSummaryList);

                CamlQuery payPeriodSummaryQuery = new CamlQuery();
                payPeriodSummaryQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + payPeridSummarySharePointId + "</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
                ListItemCollection payPeriodSummaryListItems = payPeriodSummaryList.GetItems(payPeriodSummaryQuery);
                cx.Load(payPeriodSummaryListItems);

                //Initialize SharePoint lists and its items
                cx.ExecuteQueryRetry();

                string spKeyColumnName = "SqlRow_Id"; //CheckColumnName(keyColumnName);
                bool spUpdated = false;

                foreach (ListItem li in payPeriodSummaryListItems)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                    {
                        PayPeriodSummary pps = payPeriodSummaryService.SelectById(Convert.ToInt32(li[spKeyColumnName]), DateTime.Now);
                        if (SharePoint_VP_SyncData.UpdatePayPeriodSummaryItem(li, pps, false))
                            payPeriodSummaryService.Update(pps);
                    }
                    else
                    {
                        PayPeriodSummary pps = new PayPeriodSummary();
                        SharePoint_VP_SyncData.UpdatePayPeriodSummaryItem(li, pps, true);
                        payPeriodSummaryService.Insert(pps);
                        li[spKeyColumnName] = pps.Id;
                        li.Update();

                        spUpdated = true;
                    }
                }

                if (spUpdated)
                {
                    cx.Load(payPeriodSummaryList);
                    cx.ExecuteQueryRetry();
                }
            }
        }

        static public bool JobCountIncentiveToPayEntry(string monthStart, string minimalPayPeriodStatusToProceed)
        {
            bool result = true;
            DateTime monthStartDt;

            if (!DateTime.TryParse(monthStart, out monthStartDt))
                throw new Exception("Input parameter for a processing month should be in the following format : yyyy-MM-dd");

            IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();
            IPayPeriodService payPeriodService = ServiceFactory.Instance.GetService<IPayPeriodService>();

            PayType payType = ServiceFactory.Instance.GetService<IPayTypeService>().SelectById(new PayType() { Id = 14 }, true);
            PayPeriod startToCheckPayPeriod = payPeriodService.SelectByStatus((int)PayrollStatus.Open).Find(p => p.StartDate < DateTime.Now && p.EndDate > DateTime.Now);

            DateTime startPeriod = new DateTime(monthStartDt.Year, monthStartDt.Month, 1);
            DateTime endPeriod = startPeriod.AddMonths(1).AddDays(-1);
            string processingPeriod = startPeriod.ToString("MM/dd/yyyy") + " - " + endPeriod.ToString("MM/dd/yyyy");
            string uniqueId = "JCI-" + startPeriod.ToString("yyyyMMdd") + "-" + endPeriod.ToString("yyyyMMdd");

            List<int> payPeriodIdsList = new List<int>();
            int minimalMonthStatus = GetInvolvedPayPeriods(startPeriod, endPeriod, ref payPeriodIdsList);

            if (minimalMonthStatus >= Math.Min((int)PayrollStatus.Closed, int.Parse(minimalPayPeriodStatusToProceed)))
            {
                ///List<PayEntry> payEntryListExisting = payEntryService.SelectBy(new PayEntry() {JobUniqueId = uniqueId}, new List<string> { "JobUniqueId" });
                List<PayEntry> payEntryListExisting = payEntryService.SelectByPayType(payType.Id);
                payEntryListExisting = payEntryListExisting.Where(pe => (pe.JobUniqueId.Substring(0, 21) == uniqueId)).ToList();
                if (payEntryListExisting != null && payEntryListExisting.Count > 0)
                {
                    foreach (PayEntry pe in payEntryListExisting)
                        payEntryService.Delete(pe);
                }

                List<PayEntry> payEntryList = payEntryService.SelectPayEntryByPayPeriods(payPeriodIdsList.ToArray());

                payEntryList = payEntryList.Where(pe => (pe.EffectiveEndDateTime > DateTime.Now && pe.PayType.Id == 1 && pe.JobDate >= startPeriod && pe.JobDate <= endPeriod.AddDays(1))).OrderBy(pe => pe.JobDate).ToList();
                //payEntryList = payEntryList.OrderBy(pe => pe.JobDate).ToList();

                Dictionary<string, int> countJobsByEmployee = new Dictionary<string, int>();

                foreach (PayEntry pe in payEntryList)
                {
                    if (!countJobsByEmployee.ContainsKey(pe.Employee.Name))
                        countJobsByEmployee.Add(pe.Employee.Name, 1);
                    else
                        countJobsByEmployee[pe.Employee.Name]++;

                    CreateJciPayEntry(pe, countJobsByEmployee[pe.Employee.Name], processingPeriod, payType, startToCheckPayPeriod, uniqueId + "_" + pe.Employee.Name);
                }
            }


            return result;
        }

        static private string CreateJciPayEntry(PayEntry vpPayEntry, int jobsCount, string processingPeriod, PayType payType, PayPeriod startToCheckPayPeriod, string uniqueId)
        {
            string result = "";

            try
            {
                double amount = GetMonthlyActivityIncentive(jobsCount);

                if (amount > 0.0)
                {
                    PayEntry jciPayEntry = (PayEntry)vpPayEntry.DeepClone();

                    jciPayEntry.WorkAssignment = null;
                    jciPayEntry.MtsNumber = "";
                    jciPayEntry.JobRevenue = 0;
                    jciPayEntry.LoadQuantity = 0;
                    jciPayEntry.StandyHour = 0;
                    jciPayEntry.TravelTime = 0;
                    jciPayEntry.JobUniqueId = uniqueId;
                    jciPayEntry.TravelDistance = Convert.ToDouble(jobsCount);
                    jciPayEntry.Amount = amount;
                    jciPayEntry.ModifiedUserName = "JCI_AutoProcessing";
                    jciPayEntry.Status = PayrollStatus.Approved;
                    jciPayEntry.WorkDescription = "Jobs Count = " + Convert.ToString(jobsCount) + " for " + processingPeriod;
                    jciPayEntry.PayType = payType;
                    jciPayEntry.PaySummaryType = payType.PaySummaryType;

                    PayPeriodSummary pps = GetFirstOpenPayPeriodSummary(startToCheckPayPeriod, jciPayEntry.HomeServicePoint.Id);

                    jciPayEntry.PayPeriod = pps.PayPeriod;
                    jciPayEntry.PayPeriodSummary = pps;

                    ServiceFactory.Instance.GetService<IPayEntryService>().Insert(jciPayEntry);

                    CheckPaySummary(jciPayEntry);
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        static public bool PurgePayEntryFromSharePoint(int payPeriodId)
        {
            return PurgeItemsFromSharePointList("VP_PayEntry", "PayPeriodid", payPeriodId);
        }

        static public bool PurgePayPeriodSummaryFromSharePoint(int payPeriodId)
        {
            return PurgeItemsFromSharePointList("VP_PayPeriodSummary", "PayPeriodid", payPeriodId);
        }

        static public bool PurgePaySummaryFromSharePoint(int payPeriodId)
        {
            return PurgeItemsFromSharePointList("VP_PaySummary", "PayPeriodId", payPeriodId);
        }

        static public bool PurgeItemsFromSharePointList(string listName, string payPeriodIdPropertyName, int payPeriodIdValueToPurge)
        {
            bool result = true;

            cx = GetClientContext(false);

            if (cx != null)
            {
                using (cx)
                {
                    List list = cx.Web.Lists.GetByTitle(listName);
                    cx.Load(list);
                    cx.ExecuteQueryRetry();

                    CamlQuery query = new CamlQuery();
                    query.ViewXml = $@"
                        <View>
                            <ViewFields><FieldRef Name='ID'/><FieldRef Name='" + payPeriodIdPropertyName + $@"' /></ViewFields>
                            <RowLimit>5000</RowLimit>
                        </View>";

                    List<ListItem> allListItems = new List<ListItem>();

                    do
                    {
                        ListItemCollection items = list.GetItems(query);
                        cx.Load(items);
                        cx.ExecuteQueryRetry();
                        allListItems.AddRange(items);
                        query.ListItemCollectionPosition = items.ListItemCollectionPosition;
                    }
                    while (query.ListItemCollectionPosition != null);

                    var filteredItems = allListItems.Where(item => int.Parse(item.FieldValues[payPeriodIdPropertyName].ToString()) == payPeriodIdValueToPurge).ToList();

                    int j = 0;

                    for (int i = filteredItems.Count - 1; i >= 0; i--)
                    {
                        filteredItems[i].DeleteObject();
                        j++;
                        if (j >= 100)
                        {
                            cx.Load(list);
                            cx.ExecuteQueryRetry();
                            j = 0; 
                        }
                    }

                    cx.Load(list);
                    cx.ExecuteQueryRetry();
                }
            }

            return result;
        }


        static private int GetInvolvedPayPeriods(DateTime startDate, DateTime endtDate, ref List<int> periodIds)
        {
            List<PayPeriod> payPeriodsOfInterestList = ServiceFactory.Instance.GetService<IPayPeriodService>().SelectAll();
            int minimalStatus = (int)PayrollStatus.Closed;

            foreach (PayPeriod pp in payPeriodsOfInterestList)
            {
                if (pp.StartDate < endtDate && pp.EndDate.AddDays(1) >= startDate)
                {
                    periodIds.Add(pp.Id);
                    minimalStatus = (int)pp.Status < minimalStatus ? (int)pp.Status : minimalStatus;
                }
            }

            return minimalStatus;
        }

        static private double GetMonthlyActivityIncentive(int jobsCount)
        {
            //CrewEfficiencyBonusPaySchedule crewEfficiencyBonusPaySchedule = crewEfficiencyBonusPayScheduleService.SelectAll(true).FirstOrDefault();

            return jobsCount < 11 ? 0 : (jobsCount < 16 ? 100 : 150);
        }

        static private PayPeriodSummary GetFirstOpenPayPeriodSummary(PayPeriod pp, int homeServicePointId)
        {
            PayPeriodSummary payPeriodSummary = null;

            IPayPeriodSummaryService payPeriodSummaryService = ServiceFactory.Instance.GetService<IPayPeriodSummaryService>();
            List<PayPeriodSummary> payPeriodSummaryList = payPeriodSummaryService.SelectByPayPeriodServicePoint(pp.Id, homeServicePointId);

            if (payPeriodSummaryList.Count == 0)
                throw new Exception("Default Pay Period Summary is not created.");

            payPeriodSummary = payPeriodSummaryList.FirstOrDefault(p => p.Status == PayrollStatus.Open);

            //If all proper pay period are closed, create pay entry in next available pay period.
            if (payPeriodSummary == null)
            {
                IPayPeriodService payPeriodService = ServiceFactory.Instance.GetService<IPayPeriodService>();
                List<PayPeriod> payPeriodList = payPeriodService.SelectByStatus((int)PayrollStatus.Open);

                PayPeriod nextPayPeriod = payPeriodList.Find(p => p.StartDate < pp.EndDate.AddDays(2 * 7) && p.EndDate > pp.EndDate.AddDays(2 * 7));
                return GetFirstOpenPayPeriodSummary(nextPayPeriod, homeServicePointId);
            }

            payPeriodSummary = payPeriodSummaryList.FirstOrDefault();

            return payPeriodSummary;
        }

        static private void CheckPaySummary(PayEntry pe)
        {
            IPaySummaryService paySummaryService = ServiceFactory.Instance.GetService<IPaySummaryService>();
            PaySummary paySummary = paySummaryService.SelectBy(new PaySummary() { Employee = pe.Employee, PayPeriod = pe.PayPeriod }, new List<string>() { "Employeeid", "PayPeriodid" }).FirstOrDefault();
            if (paySummary == null)
            {
                paySummary = new PaySummary();
                paySummary.Employee = pe.Employee;
                paySummary.ServicePoint = pe.HomeServicePoint;
                paySummary.PayPeriod = pe.PayPeriod;
                paySummary.UniqueId = Guid.NewGuid().ToString();
                paySummary.PayPeriodSummary = pe.PayPeriodSummary;
                paySummary.ModifiedUserName = "JCI_AutoProcessing";
                paySummary.Status = PayrollStatus.Open;

                paySummaryService.Insert(paySummary);
            }
        }

        static private SqlDataAdapter GetDataFromSQL()
        {
            string connString = SharePointUtils.GetConnectionString(@"sanjel27\DW", "SESI_Staging");

            string pViewName = (ConfigurationManager.AppSettings["applicationContext"].ToLower() == "production" ? @"dbo.V_VP_JobCountIncentivePastMonth" : @"dbo.V_VP_JobCountIncentivePastMonth_Support");
            string pSqlQuery = "select Id, JobCountRank, MonthlyActivityIncentive, ProcessingPeriod from " + pViewName;

            SqlConnection connection = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(pSqlQuery, connection);
            connection.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            return da;
        }

        static Dictionary<int, int> GetPeriodSummarySqlToSpMapping()
        {
            Dictionary<int, int> payPeriodSummarySqlToSpMapping = new Dictionary<int, int>();

            cx = GetClientContext(false);

            if (cx != null)
            {
                Web web = cx.Web;
                cx.Load(web);

                //Prepare SharePoint VP_PayPeriodSummary list data for looking up its SharePoint ID by SQL Id (to be assigned to PayPeriodSummarySharePointId field in VP_PayEntry)

                List payPeriodSummaryList = web.Lists.GetByTitle("VP_PayPeriodSummary");
                cx.Load(payPeriodSummaryList);

                CamlQuery payPeriodSummaryQuery = new CamlQuery();
                payPeriodSummaryQuery.ViewXml = "<View><ViewFields><FieldRef Name='SqlRow_Id'/><FieldRef Name='ID'/></ViewFields></View>";
                ListItemCollection payPeriodSummaryListItems = payPeriodSummaryList.GetItems(payPeriodSummaryQuery);
                cx.Load(payPeriodSummaryListItems);

                //Initialize SharePoint lists and its items
                cx.ExecuteQueryRetry();

                foreach (ListItem li in payPeriodSummaryListItems)
                {
                    if (!String.IsNullOrEmpty(Convert.ToString(li["SqlRow_Id"])) && !payPeriodSummarySqlToSpMapping.ContainsKey(Convert.ToInt32(li["SqlRow_Id"])))
                        payPeriodSummarySqlToSpMapping.Add(Convert.ToInt32(li["SqlRow_Id"]), Convert.ToInt32(li["ID"]));
                }
            }

            return payPeriodSummarySqlToSpMapping;
        }

        static Dictionary<int, int> GetPeriodSummarySpToSqlMapping()
        {
            Dictionary<int, int> payPeriodSummarySpToSqlMapping = new Dictionary<int, int>();

            cx = GetClientContext(false);

            if (cx != null)
            {
                Web web = cx.Web;
                cx.Load(web);

                //Prepare SharePoint VP_PayPeriodSummary list data for looking up its SharePoint ID by SQL Id (to be assigned to PayPeriodSummarySharePointId field in VP_PayEntry)

                List payPeriodSummaryList = web.Lists.GetByTitle("VP_PayPeriodSummary");
                cx.Load(payPeriodSummaryList);

                CamlQuery payPeriodSummaryQuery = new CamlQuery();
                payPeriodSummaryQuery.ViewXml = "<View><ViewFields><FieldRef Name='SqlRow_Id'/><FieldRef Name='ID'/></ViewFields></View>";
                ListItemCollection payPeriodSummaryListItems = payPeriodSummaryList.GetItems(payPeriodSummaryQuery);
                cx.Load(payPeriodSummaryListItems);

                //Initialize SharePoint lists and its items
                cx.ExecuteQueryRetry();

                foreach (ListItem li in payPeriodSummaryListItems)
                {
                    //if (!String.IsNullOrEmpty(Convert.ToString(li["SqlRow_Id"])))
                    payPeriodSummarySpToSqlMapping.Add(Convert.ToInt32(li["ID"]), Convert.ToInt32(li["SqlRow_Id"]));
                }
            }

            return payPeriodSummarySpToSqlMapping;
        }

        static public List<int> GetPayPeriodsForJob(string jobUniqueId)
        {
            IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();
            var existingPayEntries = payEntryService.SelectBy(new PayEntry() { JobUniqueId = jobUniqueId }, new List<string>() { "JobUniqueId" });

            List<int> uniquePayPeriodIdsInSql = new List<int>();

            foreach (PayEntry existingPayEntry in existingPayEntries)
            {
                if (!uniquePayPeriodIdsInSql.Contains(existingPayEntry.PayPeriod.Id))
                    uniquePayPeriodIdsInSql.Add(existingPayEntry.PayPeriod.Id);
            }

            return uniquePayPeriodIdsInSql;
        }

        static public int GetPayPeriodId(int offsetWeeksFromCurrentDay)
        {
            IPayPeriodService payPeriodService = ServiceFactory.Instance.GetService<IPayPeriodService>();
            List<PayPeriod> payPeriodList = payPeriodService.SelectByStatus((int)PayrollStatus.Open);
            DateTime lookupDt = DateTime.Now.AddDays(offsetWeeksFromCurrentDay * 7);

            PayPeriod payPeriod = payPeriodList.Find(p => p.StartDate <= lookupDt && p.EndDate > lookupDt);

            return payPeriod?.Id ?? -1;
        }

        static public ClientContext GetClientContext(bool enforceRefresh)
        {
            if (cx == null || SharePointUtils.TokenNeedsRefreshing() || enforceRefresh)
            {
                string appContext = ConfigurationManager.AppSettings["applicationContext"];
                string siteUrl = @"https://1961531albertaltd.sharepoint.com/sites/" + (appContext.ToLower() == "production" ? "HumanResources" : "AppTesting") + @"/";

                string token = SharePointUtils.GetToken(siteUrl);
                var ctx = new ClientContext(new Uri(siteUrl));
                ctx.ExecutingWebRequest += (sender, e) =>
                {
                    e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + token;
                };

                cx = ctx;
            }

            return cx;
        }

        static public ClientContext GetClientContext()
        {
            string appContext = ConfigurationManager.AppSettings["applicationContext"];
            string siteUrl = @"https://1961531albertaltd.sharepoint.com/sites/" + (appContext.ToLower() == "production" ? "HumanResources" : "AppTesting") + @"/";

            string token = SharePointUtils.GetToken(siteUrl);
            var ctx = new ClientContext(new Uri(siteUrl));
            ctx.ExecutingWebRequest += (sender, e) =>
            {
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + token;
            };

            return ctx;
        }


        /* ...ToDb(string payPeridSummarySharePointId)
        static public void PayEntriesToDb(string payPeridSummarySharePointId)
        {
            IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();

            if (cx == null)
                SharePointSyncProcess.InitializeClientContext().Wait();

            if (cx != null)
            {
                Web web = cx.Web;
                cx.Load(web);

                //Prepare SharePoint VP_PayEntry list data for a JobUniqueId
                CamlQuery payEntryQuery = new CamlQuery();
                payEntryQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='PayPeriodSummarySharePointId'/><Value Type='Text'>" + payPeridSummarySharePointId.ToString() + "</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
                List payEntryList = web.Lists.GetByTitle("VP_PayEntry");
                cx.Load(payEntryList);
                ListItemCollection payEntryListItems = payEntryList.GetItems(payEntryQuery);
                cx.Load(payEntryListItems);

                string spKeyColumnName = "SqlRow_Id"; //CheckColumnName(keyColumnName);

                foreach (ListItem li in payEntryListItems)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                    {
                        PayEntry pe = payEntryService.SelectBy(new PayEntry() { Id = Convert.ToInt32(li["SqlRow_Id"]) }, new List<string>() { "Id" }).First();
                        if (SharePointSyncData.UpdatePayEntryItem(li, pe, false))
                            payEntryService.Update(pe);
                    }
                    else
                    {
                        PayEntry pe = new PayEntry();
                        SharePointSyncData.UpdatePayEntryItem(li, pe, true);
                        payEntryService.Insert(pe);
                        li[spKeyColumnName] = pe.Id;
                        li.Update();
                    }
                }

                cx.Load(payEntryList);
                cx.ExecuteQueryRetry();
            }
        }

        static public void PaySummaryToDb(int paySummarySharePointId)
        {
            IPaySummaryService paySummaryService = ServiceFactory.Instance.GetService<IPaySummaryService>();

            if (cx == null)
                SharePointSyncProcess.InitializeClientContext().Wait();

            if (cx != null)
            {
                Web web = cx.Web;
                cx.Load(web);

                //Prepare SharePoint VP_PayEntry list data for a JobUniqueId
                CamlQuery paySummaryQuery = new CamlQuery();
                paySummaryQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + paySummarySharePointId.ToString() + "</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
                List paySummaryList = web.Lists.GetByTitle("VP_PaySummary");
                cx.Load(paySummaryList);
                ListItemCollection paySummaryListItems = paySummaryList.GetItems(paySummaryQuery);
                cx.Load(paySummaryListItems);

                string spKeyColumnName = "SqlRow_Id"; //CheckColumnName(keyColumnName);

                foreach (ListItem li in paySummaryListItems)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                    {
                        PaySummary ps = paySummaryService.SelectById(Convert.ToInt32(li[spKeyColumnName]), DateTime.Now);
                        if (SharePointSyncData.UpdatePaySummaryItem(li, ps, false))
                            paySummaryService.Update(ps);
                    }
                    else
                    {
                        PaySummary ps = new PaySummary();
                        SharePointSyncData.UpdatePaySummaryItem(li, ps, true);
                        paySummaryService.Insert(ps);
                        li[spKeyColumnName] = ps.Id;
                        li.Update();
                    }
                }

                cx.Load(paySummaryList);
                cx.ExecuteQueryRetry();
            }

        }


        static public bool PayEntriesToDbOld()
        {
            bool result = true;
            int rowsPerPage = 2000;
            int payEntriesCountThresholdToDeleteFromSQLWhatNotInSp = 20;
            string spKeyColumnName = SharePoint_VP_SyncData.PayEntryNames.SqlRow_Id;

            IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();
            Dictionary<int, int> payPeriodSummarySpToSqlMapping = GetPeriodSummarySpToSqlMapping();
            Dictionary<int, List<int>> payEntryIdsByPayPeriod = new Dictionary<int, List<int>>();

            List<int> payPeriodIds = GetPayPeriodsListFromSp("VP_PayEntry");

            //Run SP to DB sync re-initializing SP connection for each PayPeriod
            foreach (int ppId in payPeriodIds)
            {
                List<PayEntry> peList = payEntryService.SelectByPayPeriod(ppId);

                using (ClientContext ccx = GetClientContext())
                {
                    if (ccx != null)
                    {
                        //Web web = ccx.Web;
                        //ccx.Load(web);

                        List payEntryList = ccx.Web.Lists.GetByTitle("VP_PayEntry");
                        ccx.Load(payEntryList);
                        ccx.ExecuteQueryRetry();

                        CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);

                        do //while (true)
                        {
                            ListItemCollection payEntryListItems = payEntryList.GetItems(camlQuery);
                            ccx.Load(payEntryListItems);
                            ccx.ExecuteQueryRetry();

                            var pps = payEntryListItems.Select(s => Convert.ToInt32(s[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid])).Distinct();

                            foreach (ListItem li in payEntryListItems)
                            {
                                if (Convert.ToInt32(li[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid]) == ppId)
                                {
                                    //Find existing in DB pay entry
                                    PayEntry pe = null;
                                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                    {
                                        //pe = payEntryService.SelectById(Convert.ToInt32(li[spKeyColumnName]), DateTime.Now);
                                        pe = peList.FirstOrDefault(p => p.Id == Convert.ToInt32(li[spKeyColumnName]));
                                    }

                                    if (pe != null && pe.Id > 0)
                                    {
                                        //Try to update all properties in DB first
                                        if (SharePoint_VP_SyncData.UpdatePayEntryItem(li, pe, false, payPeriodSummarySpToSqlMapping))
                                            payEntryService.Update(pe);

                                        //Mark as deleted in DB if Deleted flag is set in SP
                                        if (pe != null && SharePoint_VP_SyncData.PayEntryDeleted(li))
                                            payEntryService.Delete(pe);
                                    }
                                    //Was not not found in DB and not marked as deleted
                                    else if (!SharePoint_VP_SyncData.PayEntryDeleted(li))
                                    {
                                        //Create new pay entry in DB
                                        pe = new PayEntry();
                                        SharePoint_VP_SyncData.UpdatePayEntryItem(li, pe, true, payPeriodSummarySpToSqlMapping);
                                        payEntryService.Insert(pe);
                                        li[spKeyColumnName] = pe.Id;
                                        li.Update();

                                        peList.Add(pe);
                                    }

                                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                    {
                                        if (!payEntryIdsByPayPeriod.ContainsKey(Convert.ToInt32(li[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid])))
                                            payEntryIdsByPayPeriod.Add(Convert.ToInt32(li[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid]), new List<int>());

                                        payEntryIdsByPayPeriod[Convert.ToInt32(li[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid])].Add(Convert.ToInt32(li[spKeyColumnName]));
                                    }
                                }
                            }

                            ccx.ExecuteQueryRetry();

                            camlQuery.ListItemCollectionPosition = payEntryListItems.ListItemCollectionPosition;

                        } while (camlQuery.ListItemCollectionPosition != null);

                        ccx.Load(payEntryList);
                        ccx.ExecuteQueryRetry();
                    }
                }

                if (payEntryIdsByPayPeriod[ppId].Count > payEntriesCountThresholdToDeleteFromSQLWhatNotInSp)
                {
                    foreach (PayEntry pe in peList)
                    {
                        if (!payEntryIdsByPayPeriod[ppId].Contains(pe.Id))
                            payEntryService.Delete(pe);
                    }
                }



                //cx = GetClientContext(true);
                //cx.RequestTimeout = -1;

                //if (cx != null)
                //{
                //    using (cx)
                //    {


                //        //List list = cx.Web.Lists.GetByTitle("VP_PayEntry");
                //        //cx.Load(list);
                //        //cx.ExecuteQueryRetry();

                //        ////CamlQuery query = new CamlQuery();
                //        ////query.ViewXml = $@"
                //        ////<View>
                //        ////    <ViewFields><FieldRef Name='ID'/><FieldRef Name='" + SharePoint_VP_SyncData.PayEntryNames.PayPeriodid + $@"' /></ViewFields>
                //        ////    <RowLimit>5000</RowLimit>
                //        ////</View>";
                //        //CamlQuery query = CamlQuery.CreateAllItemsQuery(rowsPerPage);

                //        //List<ListItem> allListItems = new List<ListItem>();

                //        //do
                //        //{
                //        //    ListItemCollection items = list.GetItems(query);
                //        //    cx.Load(items);
                //        //    cx.ExecuteQueryRetry();
                //        //    allListItems.AddRange(items);
                //        //    query.ListItemCollectionPosition = items.ListItemCollectionPosition;
                //        //}
                //        //while (query.ListItemCollectionPosition != null);

                //        ////var filteredItems = allListItems.Where(item => int.Parse(item.FieldValues[payPeriodIdPropertyName].ToString()) == payPeriodIdValueToPurge).ToList();







                //        Web web = cx.Web;
                //        cx.Load(web);
                //        List payEntryList = web.Lists.GetByTitle("VP_PayEntry");

                //        ListItemCollectionPosition itemPosition = null;

                //        while (true)
                //        {

                //            CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);
                //            camlQuery.ListItemCollectionPosition = itemPosition;

                //            ListItemCollection payEntryListItems = payEntryList.GetItems(camlQuery);
                //            cx.Load(payEntryListItems);
                //            cx.ExecuteQueryRetry();

                //            foreach (ListItem li in payEntryListItems)
                //            {
                //                if (Convert.ToInt32(li[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid]) == ppId)
                //                {
                //                    //Find existing in DB pay entry
                //                    PayEntry pe = null;
                //                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                //                        pe = payEntryService.SelectById(Convert.ToInt32(li[spKeyColumnName]), DateTime.Now);

                //                    if (pe != null)
                //                    {
                //                        //Try to update all properties in DB first
                //                        if (SharePoint_VP_SyncData.UpdatePayEntryItem(li, pe, false, payPeriodSummarySpToSqlMapping))
                //                            payEntryService.Update(pe);

                //                        //Mark as deleted in DB if Deleted flag is set in SP
                //                        if (pe != null && SharePoint_VP_SyncData.PayEntryDeleted(li))
                //                            payEntryService.Delete(pe);
                //                    }
                //                    //Was not not found in DB and not marked as deleted
                //                    else if (!SharePoint_VP_SyncData.PayEntryDeleted(li))
                //                    {
                //                        //Create new pay entry in DB
                //                        pe = new PayEntry();
                //                        SharePoint_VP_SyncData.UpdatePayEntryItem(li, pe, true, payPeriodSummarySpToSqlMapping);
                //                        payEntryService.Insert(pe);
                //                        li[spKeyColumnName] = pe.Id;
                //                        li.Update();

                //                    }

                //                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                //                    {
                //                        if (!payEntryIdsByPayPeriod.ContainsKey(Convert.ToInt32(li[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid])))
                //                            payEntryIdsByPayPeriod.Add(Convert.ToInt32(li[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid]), new List<int>());

                //                        payEntryIdsByPayPeriod[Convert.ToInt32(li[SharePoint_VP_SyncData.PayEntryNames.PayPeriodid])].Add(Convert.ToInt32(li[spKeyColumnName]));
                //                    }
                //                }
                //            }

                //            cx.ExecuteQueryRetry();

                //            itemPosition = payEntryListItems.ListItemCollectionPosition;

                //            if (itemPosition == null)
                //            {
                //                break; // TODO: might not be correct. Was : Exit While
                //            }
                //        }

                //        cx.Load(payEntryList);
                //        cx.ExecuteQueryRetry();

                //    }
                //}
            }

            //foreach (int payPeriodId in payEntryIdsByPayPeriod.Keys.Distinct<int>())
            //{
            //    if (payEntryIdsByPayPeriod[payPeriodId].Count > payEntriesCountThresholdToDeleteFromSQLWhatNotInSp)
            //    {
            //        List<PayEntry> peList = payEntryService.SelectByPayPeriod(payPeriodId);

            //        foreach (PayEntry pe in peList)
            //        {
            //            //if (!payEntryIdsByPayPeriod[payPeriodId].Contains(pe.Id) && (pe.WorkAssignment?.Id ?? 0) == 0)
            //            if (!payEntryIdsByPayPeriod[payPeriodId].Contains(pe.Id))
            //                payEntryService.Delete(pe);
            //        }
            //    }
            //}

            return result;
        }

        static List<int> GetPayPeriodsListFromSp(string listName)
        {

            //int rowsPerPage = 1000;
            List<int> payPeriodIds = new List<int>();

            string fieldName = "";

            switch (listName)
            {
                case "VP_PayEntry":
                    fieldName = SharePoint_VP_SyncData.PayEntryNames.PayPeriodid;
                    break;
                case "VP_PayPeriodSummary":
                    fieldName = SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid;
                    break;
                case "VP_PaySummary":
                    fieldName = SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId;
                    break;
                default:
                    break;
            }

            cx = GetClientContext(true);
            cx.RequestTimeout = -1;

            List list = cx.Web.Lists.GetByTitle(listName);
            cx.Load(list);
            cx.ExecuteQueryRetry();

            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"
                        <View>
                            <ViewFields><FieldRef Name='ID'/><FieldRef Name='" + fieldName + $@"' /></ViewFields>
                            <RowLimit>5000</RowLimit>
                        </View>";

            List<ListItem> allListItems = new List<ListItem>();

            do
            {
                ListItemCollection items = list.GetItems(query);
                cx.Load(items);
                cx.ExecuteQueryRetry();
                allListItems.AddRange(items);
                query.ListItemCollectionPosition = items.ListItemCollectionPosition;
            }
            while (query.ListItemCollectionPosition != null);

            //var filteredItems = allListItems.Where(item => int.Parse(item.FieldValues[payPeriodIdPropertyName].ToString()) == payPeriodIdValueToPurge).ToList();

            foreach (ListItem li in allListItems)
            {
                int pppId = Convert.ToInt32(li[fieldName]);
                if (!payPeriodIds.Contains(pppId))
                    payPeriodIds.Add(pppId);
            }
            return payPeriodIds;

        }
        static public bool PaySummaryToDbOld()
        {
            bool result = true;
            int rowsPerPage = 2000;
            string spKeyColumnName = SharePoint_VP_SyncData.PaySummaryNames.SqlRow_Id;

            IPaySummaryService paySummaryService = ServiceFactory.Instance.GetService<IPaySummaryService>();
            Dictionary<int, int> payPeriodSummarySpToSqlMapping = GetPeriodSummarySpToSqlMapping();
            Dictionary<int, List<int>> paySummaryIdsByPayPeriod = new Dictionary<int, List<int>>();

            List<int> payPeriodIds = GetPayPeriodsListFromSp("VP_PaySummary");

            //Run SP to DB sync re-initializing SP connection for each PayPeriod
            foreach (int ppId in payPeriodIds)
            {
                List<PaySummary> psList = paySummaryService.SelectByPayPeriod(ppId);

                using (ClientContext ccx = GetClientContext())
                {
                    if (ccx != null)
                    {
                        List paySummaryList = ccx.Web.Lists.GetByTitle("VP_PaySummary");
                        ccx.Load(paySummaryList);
                        ccx.ExecuteQueryRetry();

                        CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);

                        do //while (true)
                        {
                            ListItemCollection paySummaryListItems = paySummaryList.GetItems(camlQuery);
                            ccx.Load(paySummaryListItems);
                            ccx.ExecuteQueryRetry();

                            foreach (ListItem li in paySummaryListItems)
                            {
                                if (Convert.ToInt32(li[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId]) == ppId)
                                {
                                    PaySummary ps = null;

                                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                        ps = psList.FirstOrDefault(p => p.Id == Convert.ToInt32(li[spKeyColumnName]));

                                    if (ps != null && ps.Id > 0)
                                    {
                                        if (SharePoint_VP_SyncData.UpdatePaySummaryItem(li, ps, false, payPeriodSummarySpToSqlMapping))
                                            paySummaryService.Update(ps);
                                    }
                                    else
                                    {
                                        ps = new PaySummary();
                                        SharePoint_VP_SyncData.UpdatePaySummaryItem(li, ps, true, payPeriodSummarySpToSqlMapping);
                                        paySummaryService.Insert(ps);
                                        li[spKeyColumnName] = ps.Id;
                                        li.Update();
                                    }

                                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                    {
                                        if (!paySummaryIdsByPayPeriod.ContainsKey(Convert.ToInt32(li[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId])))
                                            paySummaryIdsByPayPeriod.Add(Convert.ToInt32(li[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId]), new List<int>());

                                        paySummaryIdsByPayPeriod[Convert.ToInt32(li[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId])].Add(Convert.ToInt32(li[spKeyColumnName]));
                                    }
                                }
                            }

                            ccx.ExecuteQueryRetry();

                            camlQuery.ListItemCollectionPosition = paySummaryListItems.ListItemCollectionPosition;

                        } while (camlQuery.ListItemCollectionPosition != null);

                        ccx.Load(paySummaryList);
                        ccx.ExecuteQueryRetry();
                    }
                }

                foreach (PaySummary ps in psList)
                {
                    if (!paySummaryIdsByPayPeriod[ppId].Contains(ps.Id))
                        paySummaryService.Delete(ps);
                }
            }


            //cx = GetClientContext(false);

            //if (cx != null)
            //{
            //    using (cx)
            //    {

            //        Web web = cx.Web;
            //        cx.Load(web);
            //        List paySummaryList = web.Lists.GetByTitle("VP_PaySummary");

            //        ListItemCollectionPosition itemPosition = null;

            //        Dictionary<int, List<int>> paySummaryIdsByPayPeriod = new Dictionary<int, List<int>>();

            //        while (true)
            //        {
            //            CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);
            //            camlQuery.ListItemCollectionPosition = itemPosition;

            //            ListItemCollection paySummaryListItems = paySummaryList.GetItems(camlQuery);
            //            cx.Load(paySummaryListItems);
            //            cx.ExecuteQueryRetry();

            //            foreach (ListItem li in paySummaryListItems)
            //            {
            //                PaySummary ps;
            //                if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])) && (ps = paySummaryService.SelectById(Convert.ToInt32(li[spKeyColumnName]), DateTime.Now)) != null)
            //                {
            //                    if (SharePoint_VP_SyncData.UpdatePaySummaryItem(li, ps, false, payPeriodSummarySpToSqlMapping))
            //                        paySummaryService.Update(ps);
            //                }
            //                else
            //                {
            //                    ps = new PaySummary();
            //                    SharePoint_VP_SyncData.UpdatePaySummaryItem(li, ps, true, payPeriodSummarySpToSqlMapping);
            //                    paySummaryService.Insert(ps);
            //                    li[spKeyColumnName] = ps.Id;
            //                    li.Update();
            //                }

            //                if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
            //                {
            //                    if (paySummaryIdsByPayPeriod.ContainsKey(Convert.ToInt32(li[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId])))
            //                    {
            //                        paySummaryIdsByPayPeriod[Convert.ToInt32(li[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId])].Add(Convert.ToInt32(li[spKeyColumnName]));
            //                    }
            //                    else
            //                    {
            //                        paySummaryIdsByPayPeriod.Add(Convert.ToInt32(li[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId]), new List<int>());
            //                        paySummaryIdsByPayPeriod[Convert.ToInt32(li[SharePoint_VP_SyncData.PaySummaryNames.PayPeriodId])].Add(Convert.ToInt32(li[spKeyColumnName]));
            //                    }
            //                }
            //            }

            //            cx.ExecuteQueryRetry();

            //            itemPosition = paySummaryListItems.ListItemCollectionPosition;

            //            if (itemPosition == null)
            //            {
            //                break; // TODO: might not be correct. Was : Exit While
            //            }
            //        }

            //        cx.Load(paySummaryList);
            //        cx.ExecuteQueryRetry();

            //        foreach (int payPeriodId in paySummaryIdsByPayPeriod.Keys.Distinct<int>())
            //        {
            //            List<PaySummary> psList = paySummaryService.SelectByPayPeriod(payPeriodId);

            //            foreach (PaySummary ps in psList)
            //            {
            //                if (!paySummaryIdsByPayPeriod[payPeriodId].Contains(ps.Id))
            //                    paySummaryService.Delete(ps);
            //            }
            //        }
            //    }
            //}
            return result;
        }

        static public bool PayPeriodSummaryToDbOld()
        {
            bool result = true;
            int rowsPerPage = 2000;
            string spKeyColumnName = SharePoint_VP_SyncData.PayPeriodSummaryNames.SqlRow_Id;

            IPayPeriodSummaryService payPeriodSummaryService = ServiceFactory.Instance.GetService<IPayPeriodSummaryService>();
            Dictionary<int, List<int>> payPeriodSummaryIdsByPayPeriod = new Dictionary<int, List<int>>();

            List<int> payPeriodIds = GetPayPeriodsListFromSp("VP_PayPeriodSummary");

            //Run SP to DB sync re-initializing SP connection for each PayPeriod
            foreach (int ppId in payPeriodIds)
            {
                List<PayPeriodSummary> ppsList = payPeriodSummaryService.SelectByPayPeriod(ppId);

                using (ClientContext ccx = GetClientContext())
                {
                    if (ccx != null)
                    {
                        List payPeriodSummaryList = ccx.Web.Lists.GetByTitle("VP_PayPeriodSummary");
                        ccx.Load(payPeriodSummaryList);
                        ccx.ExecuteQueryRetry();

                        CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);

                        do //while (true)
                        {
                            ListItemCollection payPeriodSummaryListItems = payPeriodSummaryList.GetItems(camlQuery);
                            ccx.Load(payPeriodSummaryListItems);
                            ccx.ExecuteQueryRetry();

                            foreach (ListItem li in payPeriodSummaryListItems)
                            {
                                if (Convert.ToInt32(li[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid]) == ppId)
                                {
                                    PayPeriodSummary pps = null;
                                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                        pps = ppsList.FirstOrDefault(p => p.Id == Convert.ToInt32(li[spKeyColumnName]));

                                    if (pps != null && pps.Id > 0)
                                    {
                                        if (SharePoint_VP_SyncData.UpdatePayPeriodSummaryItem(li, pps, false))
                                            payPeriodSummaryService.Update(pps);
                                    }
                                    else
                                    {
                                        pps = new PayPeriodSummary();
                                        SharePoint_VP_SyncData.UpdatePayPeriodSummaryItem(li, pps, true);
                                        payPeriodSummaryService.Insert(pps);
                                        li[spKeyColumnName] = pps.Id;
                                        li.Update();
                                    }

                                    if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
                                    {
                                        if (!payPeriodSummaryIdsByPayPeriod.ContainsKey(Convert.ToInt32(li[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid])))
                                            payPeriodSummaryIdsByPayPeriod.Add(Convert.ToInt32(li[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid]), new List<int>());

                                        payPeriodSummaryIdsByPayPeriod[Convert.ToInt32(li[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid])].Add(Convert.ToInt32(li[spKeyColumnName]));
                                    }
                                }
                            }

                            ccx.ExecuteQueryRetry();

                            camlQuery.ListItemCollectionPosition = payPeriodSummaryListItems.ListItemCollectionPosition;

                        } while (camlQuery.ListItemCollectionPosition != null);

                        ccx.Load(payPeriodSummaryList);
                        ccx.ExecuteQueryRetry();
                    }
                }

                foreach (PayPeriodSummary pps in ppsList)
                {
                    if (!payPeriodSummaryIdsByPayPeriod[ppId].Contains(pps.Id))
                        payPeriodSummaryService.Delete(pps);
                }
            }

            //cx = GetClientContext(false);

            //if (cx != null)
            //{
            //    using (cx)
            //    {

            //        Web web = cx.Web;
            //        cx.Load(web);
            //        List payPeriodSummaryList = web.Lists.GetByTitle("VP_PayPeriodSummary");

            //        ListItemCollectionPosition itemPosition = null;

            //        Dictionary<int, List<int>> payPeriodSummaryIdsByPayPeriod = new Dictionary<int, List<int>>();

            //        while (true)
            //        {
            //            CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery(rowsPerPage);
            //            camlQuery.ListItemCollectionPosition = itemPosition;

            //            ListItemCollection payPeriodSummaryListItems = payPeriodSummaryList.GetItems(camlQuery);
            //            cx.Load(payPeriodSummaryListItems);
            //            //cx.Load(list.Fields);
            //            cx.ExecuteQueryRetry();

            //            foreach (ListItem li in payPeriodSummaryListItems)
            //            {
            //                PayPeriodSummary pps;
            //                if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])) && (pps = payPeriodSummaryService.SelectById(new PayPeriodSummary() { Id = Convert.ToInt32(li[spKeyColumnName]) })) != null)
            //                {
            //                    if (SharePoint_VP_SyncData.UpdatePayPeriodSummaryItem(li, pps, false))
            //                        payPeriodSummaryService.Update(pps);
            //                }
            //                else
            //                {
            //                    pps = new PayPeriodSummary();
            //                    SharePoint_VP_SyncData.UpdatePayPeriodSummaryItem(li, pps, true);
            //                    payPeriodSummaryService.Insert(pps);
            //                    li[spKeyColumnName] = pps.Id;
            //                    li.Update();
            //                }

            //                if (!string.IsNullOrEmpty(Convert.ToString(li[spKeyColumnName])))
            //                {
            //                    if (payPeriodSummaryIdsByPayPeriod.ContainsKey(Convert.ToInt32(li[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid])))
            //                    {
            //                        payPeriodSummaryIdsByPayPeriod[Convert.ToInt32(li[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid])].Add(Convert.ToInt32(li[spKeyColumnName]));
            //                    }
            //                    else
            //                    {
            //                        payPeriodSummaryIdsByPayPeriod.Add(Convert.ToInt32(li[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid]), new List<int>());
            //                        payPeriodSummaryIdsByPayPeriod[Convert.ToInt32(li[SharePoint_VP_SyncData.PayPeriodSummaryNames.PayPeriodid])].Add(Convert.ToInt32(li[spKeyColumnName]));
            //                    }
            //                }
            //            }

            //            cx.ExecuteQueryRetry();

            //            itemPosition = payPeriodSummaryListItems.ListItemCollectionPosition;

            //            if (itemPosition == null)
            //            {
            //                break; // TODO: might not be correct. Was : Exit While
            //            }
            //        }

            //        cx.Load(payPeriodSummaryList);
            //        cx.ExecuteQueryRetry();

            //        foreach (int payPeriodId in payPeriodSummaryIdsByPayPeriod.Keys.Distinct<int>())
            //        {
            //            List<PayPeriodSummary> ppsList = payPeriodSummaryService.SelectByPayPeriod(payPeriodId);

            //            foreach (PayPeriodSummary pps in ppsList)
            //            {
            //                if (!payPeriodSummaryIdsByPayPeriod[payPeriodId].Contains(pps.Id))
            //                    payPeriodSummaryService.Delete(pps);
            //            }
            //        }
            //    }
            //}

            return result;
        }

    */


    }
}
