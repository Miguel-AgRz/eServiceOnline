using System;
using System.Collections.Generic;
using System.Linq;
using MetaShare.Common.Core.CommonService;
using MetaShare.Common.Foundation.Logging;
using Sesi.SanjelData.Entities.BusinessEntities.VariablePay;
using Sesi.SanjelData.Entities.Common.BusinessEntities.HumanResources;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Operation;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Organization;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.VariablePay;

namespace eServiceOnline.WebAPI.Data.VariablePay
{
    public class VariablePayData
    {
        public static object CalculateVariablePay(string jobUniqueId)
        {
            try
            {
                IWorkAssignmentService workAssignmentService =
                    ServiceFactory.Instance.GetService<IWorkAssignmentService>();
                IPayTypeService payTypeService = ServiceFactory.Instance.GetService<IPayTypeService>();
                var workAssignments = workAssignmentService.SelectBy(new WorkAssignment() {JobUniqueId = jobUniqueId},
                    new List<string>() {"JobUniqueId"});
                IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();
                var existingPayEntries = payEntryService.SelectBy(new PayEntry() {JobUniqueId = jobUniqueId},
                    new List<string>() {"JobUniqueId"});

                //Clean up last version pay entry if the work assignment not exist in latest update
                foreach (var existingPayEntry in existingPayEntries)
                {
                    var foundWorkAssignment = workAssignments.FindAll(p => p.Id == existingPayEntry.WorkAssignment.Id);
                    if (foundWorkAssignment.Count == 0)
                    {
                        payEntryService.Delete(existingPayEntry);
                    }
                }

                PayEntry variablePay = null;
                foreach (var workAssignment in workAssignments)
                {
                    try
                    {
                        if (workAssignment.JobFlag != JobFlag.Complete) continue;
                        if (workAssignment.IsMultipleWellProject) continue;

                        //Lost Code Review: Viktor 2022-07-18, fix in release _6_8_4_6 branch
                        //Ignore Infrastructure
                        //if (workAssignment.ServiceLine.Id == 14) continue;
                        //End Lost Code Review
                        var payPosition = GetPayPosition(workAssignment, false);

                        List<PayEntry> newPayEntries = new List<PayEntry>();

                        //Only calculate completed job pay
                        IPayPeriodService payPeriodService = ServiceFactory.Instance.GetService<IPayPeriodService>();
                        List<PayPeriod> payPeriodList = payPeriodService.SelectByStatus((int) PayrollStatus.Open);
                        PayPeriod payPeriod = payPeriodList.Find(p =>
                            p.StartDate < workAssignment.JobDateTime && p.EndDate > workAssignment.JobDateTime);

                        if (workAssignment.WorkType.Id == 1) //Primary Cementing
                        {
                            //Base Variable Pay
                            var payType = payTypeService.SelectById(new PayType() {Id = 1}, true);
                            variablePay =
                                VariablePayProcess.CalculateBaseVariablePay(workAssignment, payPeriod, payType,
                                    payPosition);
                            if (variablePay != null) newPayEntries.Add(variablePay);
                            //Crew efficiency Bonus
                            if (workAssignment.IsCrewEfficiency)
                            {
                                payType = payTypeService.SelectById(new PayType() {Id = 8}, true);
                                variablePay =
                                    VariablePayProcess.CalculateCrewEfficiencyBonus(workAssignment, payPeriod, payType);
                                if (variablePay != null) newPayEntries.Add(variablePay);
                            }

                            //Standby Pay
                            if (workAssignment.IsEligibleStandby && Math.Abs(workAssignment.StandbyHours) > 0.0)
                            {
                                payType = payTypeService.SelectById(new PayType() {Id = 9}, true);
                                variablePay = VariablePayProcess.CalculateStandby(workAssignment, payPeriod, payType,
                                    payPosition);
                                if (variablePay != null) newPayEntries.Add(variablePay);
                            }

                            //Extend Travel Bonus
                            if (!workAssignment.IsTwoWay)
                            {
                                payType = payTypeService.SelectById(new PayType() {Id = 2}, true);
                                variablePay =
                                    VariablePayProcess.CalculateExtendTravelBonus(workAssignment, payPeriod, payType);
                                if (variablePay != null) newPayEntries.Add(variablePay);
                            }
                        }
                        else if (workAssignment.WorkType.Id == 2) //Remedial Cementing
                        {
                            var payType = payTypeService.SelectById(new PayType() {Id = 1}, true);
                            variablePay =
                                VariablePayProcess.CalculateBaseVariablePay(workAssignment, payPeriod, payType,
                                    payPosition);
                            if (variablePay != null) newPayEntries.Add(variablePay);

                            //Extend Travel Bonus
                            if (!workAssignment.IsTwoWay)
                            {
                                payType = payTypeService.SelectById(new PayType() {Id = 2}, true);
                                variablePay = VariablePayProcess.CalculateRemedialTravelBonusAdjustment(workAssignment,
                                    payPeriod,
                                    payType);
                                if (variablePay != null) newPayEntries.Add(variablePay);
                            }

                            if (workAssignment.IsEligibleStandby && Math.Abs(workAssignment.StandbyHours) > 0.0)
                            {
                                if (payPeriod.Id <= 50)
                                {
                                    //Extend Location Premium
                                    payType = payTypeService.SelectById(new PayType() { Id = 13 }, true);
                                    variablePay = VariablePayProcess.CalculateExtendLocationPremium(workAssignment,
                                        payPeriod, payType, payPosition);
                                    if (variablePay != null) newPayEntries.Add(variablePay);
                                }
                                else
                                {
                                    payType = payTypeService.SelectById(new PayType() { Id = 9 }, true);
                                    variablePay = VariablePayProcess.CalculateStandby(workAssignment, payPeriod, payType,
                                        payPosition);
                                    if (variablePay != null) newPayEntries.Add(variablePay);
                                }
                            }

                            //Crew efficiency Bonus
                            if (workAssignment.IsCrewEfficiency)
                            {
                                payType = payTypeService.SelectById(new PayType() {Id = 8}, true);
                                variablePay =
                                    VariablePayProcess.CalculateCrewEfficiencyBonus(workAssignment, payPeriod, payType);
                                if (variablePay != null) newPayEntries.Add(variablePay);
                            }

                        }

                        //Disable product haul pay calculation for now.
                        /* 
                        else if (workAssignment.WorkType.Id == 3) //Product Haul
                        {
                            var payType = payTypeService.SelectById(new PayType() {Id = 4}, true);
                            variablePay = VariablePayProcess.CalculateProductHaulPay(workAssignment, payPeriod, payType);
                            if(variablePay != null) newPayEntries.Add(variablePay);
                        }
                        */
                        if (newPayEntries.Count > 0)
                            VariablePayProcess.SavePayEntry(newPayEntries, workAssignment);
                    }
                    catch (Exception ex)
                    {
                        SendErrorNotification(workAssignment, ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                return new {result = false, jobUniqueId, message = ex.Message};
            }
            return new {result = true,  jobUniqueId, message = "Succeed"};
        }

        private static void SendErrorNotification(WorkAssignment workAssignment, string exMessage)
        {
            string subject = "OVPP error: Job #" + workAssignment.JobNumber.ToString() + ", Employee:" +
                             workAssignment.Employee.Name;
            if (workAssignment.Employee.Name != null)
            {
                string body = "Job #" + workAssignment.JobNumber.ToString() + ", Employee:" + workAssignment.Employee.Name + "\n" + exMessage;
                string to = workAssignment.ModifiedUserName + "@sanjel.com";
//                string to = "awang@sanjel.com";
                string cc = MailUtility.DefaultMailto;

                //var mailMessage = MailUtility.CreateMailMessage(subject, body, null, "OVPP Automation", to, cc, null);
                //MailUtility.SendMail(mailMessage);

                EMail.EMailUtils.SendGraphEmailNoAttachmentsWait(to, subject, body, cc);
            }
        }

        public static PayPosition GetPayPosition(WorkAssignment workAssignment, bool ignoreJobRole)
        {
            IPayPositionMappingService payPositionMappingService =
                ServiceFactory.Instance.GetService<IPayPositionMappingService>();

            if(payPositionMappingService == null) throw new Exception("PayPositionMappingService doesn't exist");

            var payPositionList = 
                payPositionMappingService.SelectByHrPositionJobRole(workAssignment.HrPosition.Id,
                    ignoreJobRole==true?0:workAssignment.JobRole.Id);
            if (payPositionList != null && payPositionList.Count > 0)
            {
                var payPositionMapping = payPositionList.Find(p => p.ServiceLine.Id == workAssignment.ServiceLine.Id);
                if (payPositionMapping != null)
                {
                    return payPositionMapping.PayPosition;
                }
            }
            string errMessage = 
                "Job Role, HR Position and Service Line don't match. " + "Base variable pay/standby pay entries could not be calculated automatically. You may either correct it in eService and re-approve again or enter the pay entry in OVPP App.";
            SendErrorNotification(workAssignment, errMessage);
            return null;
        }
    }
}
