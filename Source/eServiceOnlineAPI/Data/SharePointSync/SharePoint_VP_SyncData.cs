using System;
using System.Collections.Generic;

using Microsoft.SharePoint.Client;

using MetaShare.Common.Core.CommonService;
using Sesi.SanjelData.Entities.BusinessEntities.VariablePay;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Entities.Common.Entities.General;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.VariablePay;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Services.Interfaces.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Services.Interfaces.Common.Entities.General;
using System.Configuration;

namespace eServiceOnline.WebAPI.Data.SharePointSync
{
    public class SharePoint_VP_SyncData
    {
        static bool PaySummaryChanged(ListItem li, PaySummary ps)
        {
            bool result = false;

            if ((ps.PayPeriod?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.PayPeriodId]))
                result = true;
            //if ((ps.PayPeriodSummary?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.PayPeriodSummaryId]))
            //    result = true;
            if ((ps.ServicePoint?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.ServicePointId]))
                result = true;
            if ((ps.ServiceLine?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.ServiceLineId]))
                result = true;
            if ((ps.Employee?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.EmployeeId]))
                result = true;
            if ((ps.Employee?.Name ?? "") != Convert.ToString(li[PaySummaryNames.EmployeeName]))
                result = true;
            if (ps.Status != (PayrollStatus)Convert.ToInt32(li[PaySummaryNames.Status]))
                result = true;

            if (ps.SumOfOVPP != Convert.ToDouble(li[PaySummaryNames.SumOfOVPP]))
                result = true;
            if (ps.MealAllowance != Convert.ToDouble(li[PaySummaryNames.MealAllowance]))
                result = true;
            if (ps.StandbyAmount != Convert.ToDouble(li[PaySummaryNames.StandbyAmount]))
                result = true;
            if (ps.BaseVariablePayAmount != Convert.ToDouble(li[PaySummaryNames.BaseVariablePayAmount]))
                result = true;
            if (ps.TotalAmount != Convert.ToDouble(li[PaySummaryNames.TotalAmount]))
                result = true;

            return result;
        }

        static public bool UpdateSharePointPaySummaryItem(ListItem li, PaySummary ps, Dictionary<int, int> payPeriodSummarySqlToSpMapping, bool isNew)
        {
            bool result = isNew || PaySummaryChanged(li, ps) || Convert.ToInt32(li[PaySummaryNames.PeriodSummarySharepointId]) != (payPeriodSummarySqlToSpMapping.ContainsKey(ps.PayPeriodSummary.Id) ? payPeriodSummarySqlToSpMapping[ps.PayPeriodSummary.Id] : 0);

            if (result)
            {
                li[PaySummaryNames.Title] = ps.Id;
                li[PaySummaryNames.SqlRow_Id] = ps.Id;
                //li[PaySummaryNames.RecordUniqueId] = pe.UniqueId;
                li[PaySummaryNames.PayPeriodId] = ps.PayPeriod.Id;
                li[PaySummaryNames.PayPeriodName] = ps.PayPeriod.Name;
                li[PaySummaryNames.ServicePointId] = ps.ServicePoint.Id;
                li[PaySummaryNames.ServicePointName] = ps.ServicePoint.Name;
                li[PaySummaryNames.ServiceLineId] = ps.ServiceLine.Id;
                li[PaySummaryNames.ServiceLineName] = ps.ServiceLine.Name;
                li[PaySummaryNames.EmployeeId] = ps.Employee.Id;
                li[PaySummaryNames.EmployeeName] = ps.Employee.Name;
                li[PaySummaryNames.Status] = ps.Status;
                li[PaySummaryNames.SumOfOVPP] = ps.SumOfOVPP;
                li[PaySummaryNames.MealAllowance] = ps.MealAllowance;
                li[PaySummaryNames.StandbyAmount] = ps.StandbyAmount;
                li[PaySummaryNames.BaseVariablePayAmount] = ps.BaseVariablePayAmount;
                li[PaySummaryNames.TotalAmount] = ps.TotalAmount;
                li[PaySummaryNames.PayPeriodSummaryId] = ps.PayPeriodSummary.Id;
                li[PaySummaryNames.PayPeriodSummaryName] = ps.PayPeriodSummary.Name;
                li[PaySummaryNames.PeriodSummarySharepointId] = payPeriodSummarySqlToSpMapping.ContainsKey(ps.PayPeriodSummary.Id) ? payPeriodSummarySqlToSpMapping[ps.PayPeriodSummary.Id] : 0;

                li.Update();
            }

            return result;
        }

        static public bool UpdatePaySummaryItem(ListItem li, PaySummary ps, bool isNew, Dictionary<int, int> payPeriodSummarySpToSqlMapping)
        {
            bool result = isNew || PaySummaryChanged(li, ps) || (ps.PayPeriodSummary?.Id ?? 0) != (Convert.ToInt32(li[PaySummaryNames.PeriodSummarySharepointId]) == 0 ? 0 : payPeriodSummarySpToSqlMapping[Convert.ToInt32(li[PaySummaryNames.PeriodSummarySharepointId])]);

            if (result)
            {
                //ps.Id = Convert.ToInt32(li[PaySummaryNames.SqlRow_Id]);
                //ps.Name = Convert.ToString(li[PaySummaryNames.PayPeriodName]);
                //pe.UniqueId = Convert.ToString(li[PaySummaryNames.RecordUniqueId]);

                if (isNew || (ps.PayPeriod?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.PayPeriodId]))
                    ps.PayPeriod = ServiceFactory.Instance.GetService<IPayPeriodService>().SelectById(new PayPeriod() { Id = Convert.ToInt32(li[PaySummaryNames.PayPeriodId]) });
                if (isNew || (ps.ServicePoint?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.ServicePointId]))
                    ps.ServicePoint = ServiceFactory.Instance.GetService<IServicePointService>().SelectById(new ServicePoint() { Id = Convert.ToInt32(li[PaySummaryNames.ServicePointId]) });
                if (isNew || (ps.ServiceLine?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.ServiceLineId]))
                    ps.ServiceLine = ServiceFactory.Instance.GetService<IServiceLineService>().SelectById(new ServiceLine() { Id = Convert.ToInt32(li[PaySummaryNames.ServiceLineId]) });
                if (isNew || (ps.PayPeriodSummary?.Id ?? 0) != payPeriodSummarySpToSqlMapping[Convert.ToInt32(li[PaySummaryNames.PeriodSummarySharepointId])])
                    ps.PayPeriodSummary = ServiceFactory.Instance.GetService<IPayPeriodSummaryService>().SelectById(new PayPeriodSummary() { Id = (Convert.ToInt32(li[PaySummaryNames.PeriodSummarySharepointId]) == 0 ? 0 : payPeriodSummarySpToSqlMapping[Convert.ToInt32(li[PaySummaryNames.PeriodSummarySharepointId])]) });
                if (isNew || (ps.Employee?.Id ?? 0) != Convert.ToInt32(li[PaySummaryNames.EmployeeId]) || (ps.Employee?.Name ?? "") != Convert.ToString(li[PaySummaryNames.EmployeeName]))
                {
                    //ps.Employee = ServiceFactory.Instance.GetService<IEmployeeService>().SelectById(new Employee() { Id = Convert.ToInt32(li[PaySummaryNames.EmployeeId]) });
                    ps.Employee = GetLatestKnownEmployeeRecordById( Convert.ToInt32(li[PaySummaryNames.EmployeeId]));
                    ps.Employee.Name = Convert.ToString(li[PaySummaryNames.EmployeeName]);
                }

                ps.Status = (PayrollStatus)Convert.ToInt32(li[PaySummaryNames.Status]);
                ps.SumOfOVPP = Convert.ToDouble(li[PaySummaryNames.SumOfOVPP]);
                ps.MealAllowance = Convert.ToDouble(li[PaySummaryNames.MealAllowance]);
                ps.StandbyAmount = Convert.ToDouble(li[PaySummaryNames.StandbyAmount]);
                ps.BaseVariablePayAmount = Convert.ToDouble(li[PaySummaryNames.BaseVariablePayAmount]);
                ps.TotalAmount = Convert.ToDouble(li[PaySummaryNames.TotalAmount]);

                ps.ModifiedUserId = 0;
                ps.ModifiedUserName = ((FieldUserValue)li["Editor"]).LookupValue;
            }

            return result;
        }

        static Employee GetLatestKnownEmployeeRecordById(int pId)
        {
            Employee result = ServiceFactory.Instance.GetService<IEmployeeService>().SelectById(new Employee() { Id = pId }); ;

            if (result == null)
            {
                List < Employee > list = ServiceFactory.Instance.GetService<IEmployeeService>().SelectAllVersions(pId);

                foreach (Employee em in list)
                {
                    if (result == null)
                        result = em;
                    else
                    {   
                        if (em.Version > result.Version)
                            result = em;
                    }
                }
            }

            return result;
        }

        public class PaySummaryNames
        {
            static public string Title = "Title";
            static public string SqlRow_Id = "SqlRow_Id";
            static public string RecordUniqueId = "RecordUniqueId";
            static public string PayPeriodId = "PayPeriodId";
            static public string PayPeriodName = "PayPeriodName";
            static public string ServicePointId = "ServicePointId";
            static public string ServicePointName = "ServicePointName";
            static public string ServiceLineId = "ServiceLineId";
            static public string ServiceLineName = "ServiceLineName";
            static public string EmployeeId = "EmployeeId";
            static public string EmployeeName = "EmployeeName";
            static public string Status = "Status";
            static public string SumOfOVPP = "SumOfOVPP";
            static public string MealAllowance = "MealAllowance";
            static public string StandbyAmount = "StandbyAmount";
            static public string BaseVariablePayAmount = "BaseVariablePayAmount";
            static public string TotalAmount = "TotalAmount";
            static public string PayPeriodSummaryId = "PayPeriodSummaryId";
            static public string PayPeriodSummaryName = "PayPeriodSummaryName";
            static public string PeriodSummarySharepointId = "PeriodSummarySharepointId";
        }



        static bool PayPeriodSummaryChanged(ListItem li, PayPeriodSummary pps)
        {
            bool result = false;

            if ((pps.PayPeriod?.Id ?? 0) != Convert.ToInt32(li[PayPeriodSummaryNames.PayPeriodid]))
                result = true;
            if ((pps.ServicePoint?.Id ?? 0) != Convert.ToInt32(li[PayPeriodSummaryNames.ServicePointid]))
                result = true;
            if ((pps.ServiceLine?.Id ?? 0) != Convert.ToInt32(li[PayPeriodSummaryNames.ServiceLineid]))
                result = true;
            if (pps.Status != (PayrollStatus)Convert.ToInt32(li[PayPeriodSummaryNames.Status]))
                result = true;
            if (pps.Amount != Convert.ToDouble(li[PayPeriodSummaryNames.Amount]))
                result = true;
            if (pps.Name != Convert.ToString(li[PayPeriodSummaryNames.PayPeriodSummaryName]))
                result = true;

            return result;
        }

        static public bool UpdateSharePointPayPeriodSummaryItem(ListItem li, PayPeriodSummary pps, bool isNew)
        {
            bool result = isNew || PayPeriodSummaryChanged(li, pps);

            if (result)
            {
                li[PayPeriodSummaryNames.Title] = pps.Id;
                li[PayPeriodSummaryNames.SqlRow_Id] = pps.Id;

                li[PayPeriodSummaryNames.PayPeriodid] = pps.PayPeriod.Id;
                li[PayPeriodSummaryNames.PayPeriodName] = pps.PayPeriod.Name;
                li[PayPeriodSummaryNames.ServicePointid] = pps.ServicePoint.Id;
                li[PayPeriodSummaryNames.ServicePointName] = pps.ServicePoint.Name;
                li[PayPeriodSummaryNames.ServiceLineid] = pps.ServiceLine.Id;
                li[PayPeriodSummaryNames.ServiceLineName] = pps.ServiceLine.Name;
                li[PayPeriodSummaryNames.Amount] = pps.Amount;
                li[PayPeriodSummaryNames.Status] = pps.Status;
                li[PayPeriodSummaryNames.PayPeriodSummaryName] = pps.Name;
                //li[PayPeriodSummaryNames.IsCutOff] = pps.IsCutOff ? "Yes" : "No";

                li.Update();
            }

            return result;
        }

        static public bool UpdatePayPeriodSummaryItem(ListItem li, PayPeriodSummary pps, bool isNew)
        {
            bool result = isNew || PayPeriodSummaryChanged(li, pps);

            if (result)
            {
                //pps.Id = Convert.ToInt32((li["SqlRow_Id"]);
                //pe.UniqueId = Convert.ToString(li["RecordUniqueId"]);

                if (isNew || (pps.PayPeriod?.Id ?? 0) != Convert.ToInt32(li[PayPeriodSummaryNames.PayPeriodid]))
                    pps.PayPeriod = ServiceFactory.Instance.GetService<IPayPeriodService>().SelectById(new PayPeriod() { Id = Convert.ToInt32(li[PayPeriodSummaryNames.PayPeriodid]) });

                if (isNew || (pps.ServicePoint?.Id ?? 0) != Convert.ToInt32(li[PayPeriodSummaryNames.ServicePointid]))
                    pps.ServicePoint = ServiceFactory.Instance.GetService<IServicePointService>().SelectById(new ServicePoint() { Id = Convert.ToInt32(li[PayPeriodSummaryNames.ServicePointid]) });

                if (isNew || (pps.ServiceLine?.Id ?? 0) != Convert.ToInt32(li[PayPeriodSummaryNames.ServiceLineid]))
                    pps.ServiceLine = ServiceFactory.Instance.GetService<IServiceLineService>().SelectById(new ServiceLine() { Id = Convert.ToInt32(li[PayPeriodSummaryNames.ServiceLineid]) });

                pps.Status = (PayrollStatus)Convert.ToInt32(li[PayPeriodSummaryNames.Status]);
                pps.Amount = Convert.ToDouble(li[PayPeriodSummaryNames.Amount]);
                pps.Name = Convert.ToString(li[PayPeriodSummaryNames.PayPeriodSummaryName]);
                //pps.IsCutOff = Convert.ToString(li["PayPeriodSummaryNames.IsTwoWay"]) == "Yes" ? true : false;

                pps.ModifiedUserId = 0;
                pps.ModifiedUserName = ((FieldUserValue)li["Editor"]).LookupValue;
            }

            return result;
        }

        public class PayPeriodSummaryNames
        {
            static public string Title = "Title";
            static public string SqlRow_Id = "SqlRow_Id";
            static public string PayPeriodid = "PayPeriodid";
            static public string PayPeriodName = "PayPeriodName";
            static public string ServicePointid = "ServicePointid";
            static public string ServicePointName = "ServicePointName";
            static public string ServiceLineid = "ServiceLineid";
            static public string ServiceLineName = "ServiceLineName";
            static public string Amount = "Amount";
            static public string Status = "Status";
            static public string PayPeriodSummaryName = "PayPeriodSummaryName";
            static public string IsCutOff = "IsCutOff";
        }



        static bool PayEntryChanged(ListItem li, PayEntry pe)
        {
            bool result = false;

            if ((pe.Employee?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.Employeeid]))
                result = true;
            if ((pe.Employee?.Name ?? "") != Convert.ToString(li[PayEntryNames.EmployeeName]))
                result = true;
            if ((pe.PayPeriod?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayPeriodid]))
                result = true;
            if ((pe.PaySummaryType?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PaySummaryTypeId]))
                result = true;
            //if ((pe.PayPeriodSummary?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayPeriodSummaryId]))
            //    result = true;
            if ((pe.HomeServicePoint?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.HomeServicePointid]))
                result = true;
            if ((pe.WorkingServicePoint?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.WorkingServicePointid]))
                result = true;
            if ((pe.WorkType?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.WorkTypeid]))
                result = true;
            if ((pe.JobProvince?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.JobProvinceid]))
                result = true;
            if ((pe.ClientCompany?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.ClientCompanyid]))
                result = true;
            if ((pe.ServiceLine?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.ServiceLineid]))
                result = true;
            if ((pe.PayArea?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayAreaid]))
                result = true;
            if ((pe.PayType?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayTypeid]))
                result = true;
            if ((pe.PayPosition?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayPositionid]))
                result = true;
            if ((pe.WorkAssignment?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.WorkAssignmentid]))
                result = true;
            if (pe.UniqueId != Convert.ToString(li[PayEntryNames.RecordUniqueId]))
                result = true;
            if (pe.JobNumber != Convert.ToString(li[PayEntryNames.JobNumber]))
                result = true;
            if (pe.JobUniqueId != Convert.ToString(li[PayEntryNames.JobUniqueId]))
                result = true;
            if (pe.JobRevenue != Convert.ToDouble(li[PayEntryNames.JobRevenue]))
                result = true;

            //TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            //DateTime mdtDatetime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(li[PayEntryNames.JobDate]), tz);
            //if (pe.JobDate != System.TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(li[PayEntryNames.JobDate])))
            //    result = true;
            if (pe.LoadQuantity != Convert.ToDouble(li[PayEntryNames.LoadQuantity]))
                result = true;
            if (pe.WorkDescription != Convert.ToString(li[PayEntryNames.WorkDescription]))
                result = true;
            if (pe.MtsNumber != (String.IsNullOrEmpty(Convert.ToString(li[PayEntryNames.MtsNumber])) ? Convert.ToString(li[PayEntryNames.Sp_OVPPNum]) : Convert.ToString(li[PayEntryNames.MtsNumber])))
                result = true;
            if (pe.Status != (PayrollStatus)Convert.ToInt32(li[PayEntryNames.Status]))
                result = true;
            if (pe.Amount != Convert.ToDouble(li[PayEntryNames.Amount]))
                result = true;
            if (pe.IsTwoWay != (Convert.ToString(li[PayEntryNames.IsTwoWay]) == "True" ? true : false))
                result = true;
            if (pe.StandyHour != Convert.ToDouble(li[PayEntryNames.StandyHour]))
                result = true;
            if (pe.TravelDistance != Convert.ToDouble(li[PayEntryNames.TravelDistance]))
                result = true;
            if (pe.TravelTime != Convert.ToDouble(li[PayEntryNames.TravelTime]))
                result = true;
            if (pe.CrewCount != Convert.ToInt32(li[PayEntryNames.CrewCount]))
                result = true;

            return result;
        }

        static public bool UpdateSharePointPayEntryItem(ListItem li, PayEntry pe, Dictionary<int, int> payPeriodSummarySqlToSpMapping, bool isNew)
        {
            bool result = isNew || PayEntryChanged(li, pe) || Convert.ToInt32(li[PayEntryNames.PayPeriodSummarySharePointId]) != (payPeriodSummarySqlToSpMapping.ContainsKey(pe.PayPeriodSummary.Id) ? payPeriodSummarySqlToSpMapping[pe.PayPeriodSummary.Id] : 0);

            if (result)
            {
                li[PayEntryNames.Title] = pe.Id;
                li[PayEntryNames.SqlRow_Id] = pe.Id;
                li[PayEntryNames.RecordUniqueId] = pe.UniqueId;
                li[PayEntryNames.PayPeriodid] = pe.PayPeriod.Id;
                li[PayEntryNames.PayPeriodName] = pe.PayPeriod.Name;
                li[PayEntryNames.PayPeriodSummaryId] = pe.PayPeriodSummary.Id;
                li[PayEntryNames.PayPeriodSummaryName] = pe.PayPeriodSummary.Name;
                li[PayEntryNames.HomeServicePointid] = pe.HomeServicePoint.Id;
                li[PayEntryNames.HomeServicePointName] = pe.HomeServicePoint.Name;
                li[PayEntryNames.WorkingServicePointid] = pe.WorkingServicePoint.Id;
                li[PayEntryNames.WorkingServicePointName] = pe.WorkingServicePoint.Name;
                li[PayEntryNames.ClientCompanyid] = pe.ClientCompany.Id;
                li[PayEntryNames.ClientCompanyName] = pe.ClientCompany.Name;
                li[PayEntryNames.Employeeid] = pe.Employee.Id;
                li[PayEntryNames.EmployeeName] = pe.Employee.Name;
                li[PayEntryNames.JobNumber] = pe.JobNumber;
                li[PayEntryNames.ServiceLineid] = pe.ServiceLine.Id;
                li[PayEntryNames.ServiceLineName] = pe.ServiceLine.Name;
                li[PayEntryNames.JobRevenue] = pe.JobRevenue;
                li[PayEntryNames.PayPositionid] = pe.PaySummaryType.Name == "OVPP" ? pe.PayPosition?.Id : pe.PayPosition.Id;
                li[PayEntryNames.PayPositionName] = pe.PaySummaryType.Name == "OVPP" ? pe.PayPosition?.Name : pe.PayPosition.Name;
                li[PayEntryNames.WorkAssignmentid] = pe.WorkAssignment.Id;
                li[PayEntryNames.WorkAssignmentName] = pe.WorkAssignment.Name;
                li[PayEntryNames.WorkTypeid] = pe.WorkType.Id;
                li[PayEntryNames.WorkTypeName] = pe.WorkType.Name;
                li[PayEntryNames.PayTypeid] = pe.PayType.Id;
                li[PayEntryNames.PayTypeName] = pe.PayType.Name;
                li[PayEntryNames.PayAreaid] = pe.PayArea.Id;
                li[PayEntryNames.PayAreaName] = pe.PayArea.Name;
                li[PayEntryNames.LoadQuantity] = pe.LoadQuantity;
                li[PayEntryNames.WorkDescription] = pe.WorkDescription;
                li[PayEntryNames.MtsNumber] = pe.MtsNumber;
                li[PayEntryNames.JobUniqueId] = pe.JobUniqueId;
                li[PayEntryNames.JobDate] = pe.JobDate.Date;
                li[PayEntryNames.Status] = pe.Status;
                li[PayEntryNames.Amount] = pe.Amount;
                li[PayEntryNames.IsTwoWay] = pe.IsTwoWay;
                li[PayEntryNames.JobProvinceid] = pe.JobProvince.Id;
                li[PayEntryNames.JobProvinceName] = pe.JobProvince.Name;
                li[PayEntryNames.PaySummaryTypeId] = pe.PaySummaryType.Id;
                li[PayEntryNames.PaySummaryTypeName] = pe.PaySummaryType.Name;
                li[PayEntryNames.StandyHour] = pe.StandyHour;
                li[PayEntryNames.TravelDistance] = pe.TravelDistance;
                li[PayEntryNames.TravelTime] = pe.TravelTime;
                li[PayEntryNames.ApprovedBy] = "";
                li[PayEntryNames.PayPeriodSummarySharePointId] = payPeriodSummarySqlToSpMapping.ContainsKey(pe.PayPeriodSummary.Id) ? payPeriodSummarySqlToSpMapping[pe.PayPeriodSummary.Id] : 0;
                li[PayEntryNames.Sp_IsDeleted] = 0;
                li[PayEntryNames.CrewCount] = pe.CrewCount;

                li.Update();
            }

            return result;
        }

        static public bool PayEntryDeleted(ListItem li)
        {
            return Convert.ToInt32(li[PayEntryNames.Sp_IsDeleted]) == 1 ? true : false;
        }

        static public bool UpdatePayEntryItem(ListItem li, PayEntry pe, bool isNew, Dictionary<int, int> payPeriodSummarySpToSqlMapping)
        {
            bool result = 
                isNew 
                || PayEntryChanged(li, pe) 
                || (pe.PayPeriodSummary?.Id ?? 0) != (Convert.ToInt32(li["PayPeriodSummarySharePointId"]) == 0 ? 0 : payPeriodSummarySpToSqlMapping[Convert.ToInt32(li["PayPeriodSummarySharePointId"])])
                || ((pe.WorkAssignment?.Id ?? 0) == 0 && pe.JobDate != System.TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(li[PayEntryNames.JobDate])))
                ;

            if (result)
            {
                //pe.Id = Convert.ToInt32(li[PayEntryNames.Title]);
                //pe.Id = Convert.ToInt32(li[PayEntryNames.SqlRow_Id]);
                if (isNew || (pe.Employee?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.Employeeid]) || (pe.Employee?.Name ?? "") != Convert.ToString(li[PayEntryNames.EmployeeName]))
                {
                    //pe.Employee = ServiceFactory.Instance.GetService<IEmployeeService>().SelectById(new Employee() { Id = Convert.ToInt32(li[PayEntryNames.Employeeid]) });
                    pe.Employee = GetLatestKnownEmployeeRecordById(Convert.ToInt32(li[PayEntryNames.Employeeid]));
                    if (pe.Employee != null)
                    {
                        pe.Employee.Name = Convert.ToString(li[PayEntryNames.EmployeeName]);
                    }
                    else if (ConfigurationManager.AppSettings["applicationContext"] != "production")
                    {
                        pe.Employee = new Employee { Id = Convert.ToInt32(li[PayEntryNames.Employeeid]), Name = Convert.ToString(li[PayEntryNames.EmployeeName])};
                    }
                }
                if (isNew || (pe.PayPeriod?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayPeriodid]))
                    pe.PayPeriod = ServiceFactory.Instance.GetService<IPayPeriodService>().SelectById(new PayPeriod() { Id = Convert.ToInt32(li[PayEntryNames.PayPeriodid]) });
                if (isNew || (pe.PaySummaryType?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PaySummaryTypeId]))
                    pe.PaySummaryType = ServiceFactory.Instance.GetService<IPaySummaryTypeService>().SelectById(new PaySummaryType() { Id = Convert.ToInt32(li[PayEntryNames.PaySummaryTypeId]) });

                if (isNew || (pe.PayPeriodSummary?.Id ?? 0) != (Convert.ToInt32(li[PayEntryNames.PayPeriodSummarySharePointId]) == 0 ? 0 : payPeriodSummarySpToSqlMapping[Convert.ToInt32(li[PayEntryNames.PayPeriodSummarySharePointId])]))
                    pe.PayPeriodSummary = ServiceFactory.Instance.GetService<IPayPeriodSummaryService>().SelectById(new PayPeriodSummary() 
                        { Id = Convert.ToInt32(li[PayEntryNames.PayPeriodSummarySharePointId]) == 0 
                                ? 0 
                                : Convert.ToInt32(payPeriodSummarySpToSqlMapping[Convert.ToInt32(li[PayEntryNames.PayPeriodSummarySharePointId])]) });
                
                
                if (isNew || (pe.HomeServicePoint?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.HomeServicePointid]))
                    pe.HomeServicePoint = ServiceFactory.Instance.GetService<IServicePointService>().SelectById(new ServicePoint() { Id = Convert.ToInt32(li[PayEntryNames.HomeServicePointid]) });
                if (isNew || (pe.WorkingServicePoint?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.WorkingServicePointid]))
                    pe.WorkingServicePoint = ServiceFactory.Instance.GetService<IServicePointService>().SelectById(new ServicePoint() { Id = Convert.ToInt32(li[PayEntryNames.WorkingServicePointid]) });
                if (isNew || (pe.WorkType?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.WorkTypeid]))
                    pe.WorkType = ServiceFactory.Instance.GetService<IWorkTypeService>().SelectById(new WorkType() { Id = Convert.ToInt32(li[PayEntryNames.WorkTypeid]) });
                if (isNew || (pe.JobProvince?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.JobProvinceid]))
                    pe.JobProvince = ServiceFactory.Instance.GetService<IProvinceOrStateService>().SelectById(new ProvinceOrState() { Id = Convert.ToInt32(li[PayEntryNames.JobProvinceid]) });
                if (isNew || (pe.ClientCompany?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.ClientCompanyid]))
                    pe.ClientCompany = ServiceFactory.Instance.GetService<IClientCompanyService>().SelectById(new ClientCompany() { Id = Convert.ToInt32(li[PayEntryNames.ClientCompanyid]) });
                if (isNew || (pe.ServiceLine?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.ServiceLineid]))
                    pe.ServiceLine = ServiceFactory.Instance.GetService<IServiceLineService>().SelectById(new ServiceLine() { Id = Convert.ToInt32(li[PayEntryNames.ServiceLineid]) });
                if (isNew || (pe.PayArea?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayAreaid]))
                    pe.PayArea = ServiceFactory.Instance.GetService<IPayAreaService>().SelectById(new PayArea() { Id = Convert.ToInt32(li[PayEntryNames.PayAreaid]) });
                if (isNew || (pe.PayType?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayTypeid]))
                    pe.PayType = ServiceFactory.Instance.GetService<IPayTypeService>().SelectById(new PayType() { Id = Convert.ToInt32(li[PayEntryNames.PayTypeid]) });
                if (isNew || (pe.PayPosition?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.PayPositionid]))
                    pe.PayPosition = ServiceFactory.Instance.GetService<IPayPositionService>().SelectById(new PayPosition() { Id = Convert.ToInt32(li[PayEntryNames.PayPositionid]) });
                if (isNew || (pe.WorkAssignment?.Id ?? 0) != Convert.ToInt32(li[PayEntryNames.WorkAssignmentid]))
                    pe.WorkAssignment = ServiceFactory.Instance.GetService<IWorkAssignmentService>().SelectById(new WorkAssignment() { Id = Convert.ToInt32(li[PayEntryNames.WorkAssignmentid]) });

                pe.UniqueId = Convert.ToString(li[PayEntryNames.RecordUniqueId]);
                pe.JobNumber = Convert.ToString(li[PayEntryNames.JobNumber]);
                pe.JobUniqueId = Convert.ToString(li[PayEntryNames.JobUniqueId]);
                //if (isNew || ((pe.WorkAssignment?.Id ?? 0) == 0 && pe.JobDate != System.TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(li[PayEntryNames.JobDate]))))
                if (isNew || ((pe.WorkAssignment?.Id ?? 0) == 0 && pe.JobDate.Date != System.TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(li[PayEntryNames.JobDate])).Date ))
                        pe.JobDate = System.TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(li[PayEntryNames.JobDate]));
                pe.JobRevenue = Convert.ToDouble(li[PayEntryNames.JobRevenue]);
                pe.LoadQuantity = Convert.ToDouble(li[PayEntryNames.LoadQuantity]);
                pe.WorkDescription = Convert.ToString(li[PayEntryNames.WorkDescription]);
                pe.MtsNumber = String.IsNullOrEmpty(Convert.ToString(li[PayEntryNames.MtsNumber])) ? Convert.ToString(li[PayEntryNames.Sp_OVPPNum]) : Convert.ToString(li[PayEntryNames.MtsNumber]);
                pe.Status = (PayrollStatus)Convert.ToInt32(li[PayEntryNames.Status]);
                pe.Amount = Convert.ToDouble(li[PayEntryNames.Amount]);
                pe.IsTwoWay = Convert.ToString(li[PayEntryNames.IsTwoWay]) == "True" ? true : false;
                pe.StandyHour = Convert.ToDouble(li[PayEntryNames.StandyHour]);
                pe.TravelDistance = Convert.ToDouble(li[PayEntryNames.TravelDistance]);
                pe.TravelTime = Convert.ToDouble(li[PayEntryNames.TravelTime]);
                pe.CrewCount = Convert.ToInt32(li[PayEntryNames.CrewCount]);
                //pe.Status = PayrollStatus.Approved;
                // = li[PayEntryNames.ApprovedBy] = ;
                // = li[PayEntryNames.PayPeriodSummarySharePointId] = payPeriodSummarySqlToSpMapping.ContainsKey(pe.PayPeriodSummary.Id) ? payPeriodSummarySqlToSpMapping[pe.PayPeriodSummary.Id] : 0;

                pe.ModifiedUserId = 0;
                pe.ModifiedUserName = ((FieldUserValue)li["Editor"]).LookupValue;
            }

            return result;
        }

        public class PayEntryNames
        {
            static public string Title = "Title";
            static public string SqlRow_Id = "SqlRow_Id";
            static public string RecordUniqueId = "RecordUniqueId";
            static public string PayPeriodid = "PayPeriodid";
            static public string PayPeriodName = "PayPeriodName";
            static public string HomeServicePointid = "HomeServicePointid";
            static public string HomeServicePointName = "HomeServicePointName";
            static public string WorkingServicePointid = "WorkingServicePointid";
            static public string WorkingServicePointName = "WorkingServicePointName";
            static public string ClientCompanyid = "ClientCompanyid";
            static public string ClientCompanyName = "ClientCompanyName";
            static public string Employeeid = "Employeeid";
            static public string EmployeeName = "EmployeeName";
            static public string JobNumber = "JobNumber";
            static public string ServiceLineid = "ServiceLineid";
            static public string ServiceLineName = "ServiceLineName";
            static public string JobRevenue = "JobRevenue";
            static public string PayPositionid = "PayPositionid";
            static public string PayPositionName = "PayPositionName";
            static public string WorkAssignmentid = "WorkAssignmentid";
            static public string WorkAssignmentName = "WorkAssignmentName";
            static public string WorkTypeid = "WorkTypeid";
            static public string WorkTypeName = "WorkTypeName";
            static public string PayTypeid = "PayTypeid";
            static public string PayTypeName = "PayTypeName";
            static public string PayAreaid = "PayAreaid";
            static public string PayAreaName = "PayAreaName";
            static public string LoadQuantity = "LoadQuantity";
            static public string WorkDescription = "WorkDescription";
            static public string MtsNumber = "MtsNumber";
            static public string JobUniqueId = "JobUniqueId";
            static public string JobDate = "JobDate";
            static public string Status = "Status";
            static public string Amount = "Amount";
            static public string IsTwoWay = "IsTwoWay";
            static public string JobProvinceid = "JobProvinceid";
            static public string JobProvinceName = "JobProvinceName";
            static public string PaySummaryTypeId = "PaySummaryTypeId";
            static public string PaySummaryTypeName = "PaySummaryTypeName";
            static public string StandyHour = "StandyHour";
            static public string TravelDistance = "TravelDistance";
            static public string TravelTime = "TravelTime";
            static public string PayPeriodSummaryId = "PayPeriodSummaryId";
            static public string PayPeriodSummaryName = "PayPeriodSummaryName";
            static public string ApprovedBy = "ApprovedBy";
            static public string PayPeriodSummarySharePointId = "PayPeriodSummarySharePointId";
            static public string Sp_OVPPNum = "_x0067_wo1";
            static public string Sp_IsDeleted = "IsDeleted";
            static public string CrewCount = "CrewCount";
        }

    }
}
