using System;
using System.Collections.Generic;
using System.Linq;
using MetaShare.Common.Core.CommonService;
using Sesi.SanjelData.Entities.BusinessEntities.VariablePay;
using Sesi.SanjelData.Services.Interfaces.BusinessEntities.VariablePay;

namespace eServiceOnline.WebAPI.Data.VariablePay
{
    public class VariablePayProcess
    {
        public static PayEntry CalculateBaseVariablePay(WorkAssignment workAssignment, PayPeriod payPeriod, PayType payType, PayPosition payPosition)
        {
            return payPosition==null?null:CalculateBaseVariablePay(workAssignment, payPeriod, payPosition, payType);
        }

        private static PayEntry CalculateBaseVariablePay(WorkAssignment workAssignment, PayPeriod payPeriod,
            PayPosition payPosition, PayType payType, double payRatio = 1)
        {
            IBaseVariablePayScheduleService baseVariablePayScheduleService =
                ServiceFactory.Instance.GetService<IBaseVariablePayScheduleService>();

            if (baseVariablePayScheduleService != null)
            {
                double baseVariablePay = 0.0;
                var baseVariablePaySchedules = baseVariablePayScheduleService.SelectAll();

                var baseVariablePaySchedule = baseVariablePaySchedules.Find(p =>
                    p.PayArea.Name == workAssignment.PayArea.Name && p.PayPosition.Id == payPosition.Id &&
                    workAssignment.JobRevenue >= p.RevenueStart && workAssignment.JobRevenue < p.RevenueEnd);
                if (baseVariablePaySchedule != null)
                {
                    baseVariablePay = baseVariablePaySchedule.BaseAmount;

                    if (Math.Abs(baseVariablePaySchedule.Percentage) > 0.00001)
                    {
                        baseVariablePay = baseVariablePay +
                                          (workAssignment.JobRevenue - baseVariablePaySchedule.RevenueStart) *
                                          baseVariablePaySchedule.Percentage;
                    }

                    return CreatePayEntry(workAssignment, payType, payPeriod, baseVariablePay * payRatio, payPosition);
                }
            }

            return null;
        }

        private static void SavePayEntry(PayEntry payEntry)
        {
            IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();

            if (payEntryService != null)
            {
                List<PayEntry> existingPayEntries = payEntryService.SelectBy(payEntry, new List<string>() {"WorkAssignmentid", "PayTypeid"});
                if (existingPayEntries != null && existingPayEntries.Count > 0)
                {
                    var existingPayEntry = existingPayEntries.First();
                    payEntry.Id = existingPayEntry.Id;
                    payEntry.UniqueId = existingPayEntry.UniqueId;
                    payEntryService.Update(payEntry);
                }
                else
                {
                    payEntryService.Insert(payEntry);
                }
            }
        }

        public static void SavePayEntry(List<PayEntry> newPayEntries, WorkAssignment workAssignment)
        {
            IPayEntryService payEntryService = ServiceFactory.Instance.GetService<IPayEntryService>();

            if (payEntryService != null)
            {
                List<PayEntry> existingPayEntries = payEntryService.SelectBy(new PayEntry(){WorkAssignment = workAssignment}, new List<string>() {"WorkAssignmentid"});
                if (existingPayEntries != null && existingPayEntries.Count > 0)
                {
                    foreach (var newPayEntry in newPayEntries)
                    {
                        var foundPayEntry = existingPayEntries.Find(p => p.PayType.Id == newPayEntry.PayType.Id);
                        if (foundPayEntry == null)
                        {
                            newPayEntry.UniqueId = Guid.NewGuid().ToString();
                            payEntryService.Insert(newPayEntry);
                        }
                        else
                        {
                            foundPayEntry = UpdatePayEntry(foundPayEntry, newPayEntry);
                            payEntryService.Update(foundPayEntry);
                        }

                    }

                    foreach (var existingPayEntry in existingPayEntries)
                    {
                        var foundPayEntry = newPayEntries.Find(p => p.PayType.Id == existingPayEntry.PayType.Id);
                        if (foundPayEntry == null)
                        {
                            payEntryService.Delete(existingPayEntry);
                        }
                    }
                    
                }
                else
                {
                    foreach (var newPayEntry in newPayEntries)
                    {
                        newPayEntry.UniqueId = Guid.NewGuid().ToString();
                        payEntryService.Insert(newPayEntry);
                    }
                }
            }
        }


        private static PayEntry CreatePayEntry(WorkAssignment workAssignment, PayType payType, PayPeriod payPeriod,
            double baseVariablePay, PayPosition payPosition)
        {
            if(payPeriod == null) throw new Exception("Pay period is null.");
            PayPeriodSummary payPeriodSummary = null;
            IPayPeriodSummaryService payPeriodSummaryService =
                ServiceFactory.Instance.GetService<IPayPeriodSummaryService>();
            List<PayPeriodSummary> payPeriodSummaryList =
                payPeriodSummaryService.SelectByPayPeriodServicePoint(payPeriod.Id,
                    workAssignment.HomeServicePoint.Id);
            if (payPeriodSummaryList.Count == 0)
            {
                throw new Exception("Default Pay Period Summary cannot be found.");
                /*
                var newPayPeriodSummary = new PayPeriodSummary()
                {
                    Name = payPeriod.Name,
                    PayPeriod = payPeriod,
                    ServicePoint = workAssignment.WorkingServicePoint,
                    Status = PayrollStatus.Open,
                    UniqueId = Guid.NewGuid().ToString()
                };
                payPeriodSummaryService.Insert(newPayPeriodSummary);
                        
                payPeriodSummaryList =
                    payPeriodSummaryService.SelectByPayPeriodServicePoint(payPeriod.Id,
                        workAssignment.WorkingServicePoint.Id);
            */
            }

            payPeriodSummary = payPeriodSummaryList.FirstOrDefault(p => p.Status == PayrollStatus.Open);
            //If all proper pay period are closed, create pay entry in next available pay period.
            if (payPeriodSummary == null)
            {
                IPayPeriodService payPeriodService = ServiceFactory.Instance.GetService<IPayPeriodService>();
                List<PayPeriod> payPeriodList = payPeriodService.SelectByStatus((int) PayrollStatus.Open);
                PayPeriod nextPayPeriod = payPeriodList.Find(p =>
                    p.StartDate < payPeriod.EndDate.AddDays(2 * 7) && p.EndDate > payPeriod.EndDate.AddDays(2 * 7));
                return CreatePayEntry(workAssignment, payType, nextPayPeriod, baseVariablePay, payPosition);
            }

            IPaySummaryService paySummaryService =  ServiceFactory.Instance.GetService<IPaySummaryService>();
            PaySummary paySummary = paySummaryService.SelectBy(
                new PaySummary() {Employee = workAssignment.Employee, PayPeriod = payPeriod}, new List<string>() {"Employeeid","PayPeriodid" }).FirstOrDefault();
            if (paySummary == null)
            {
                paySummary = new PaySummary();
                paySummary.Employee = workAssignment.Employee;
                paySummary.ServicePoint = workAssignment.HomeServicePoint;
                paySummary.PayPeriod = payPeriod;
                paySummary.UniqueId = Guid.NewGuid().ToString();
                paySummary.PayPeriodSummary = payPeriodSummary;
                paySummaryService.Insert(paySummary);
            }

            payPeriodSummary = payPeriodSummaryList.FirstOrDefault();

            PayEntry payEntry = new PayEntry();
            payEntry.MtsNumber = workAssignment.MtsNumber;
            payEntry.Employee = workAssignment.Employee;
            payEntry.TravelTime = workAssignment.TravelTime;
            payEntry.JobRevenue = workAssignment.JobRevenue;
            payEntry.WorkAssignment = workAssignment;
            payEntry.WorkDescription = workAssignment.WorkDescription;
            payEntry.TravelDistance = workAssignment.TravelDistance;
            payEntry.PaySummaryType = payType.PaySummaryType;
            payEntry.JobDate = workAssignment.JobDateTime;
            payEntry.HomeServicePoint = workAssignment.HomeServicePoint;
            payEntry.JobUniqueId = workAssignment.JobUniqueId;
            payEntry.WorkingServicePoint = workAssignment.WorkingServicePoint;
            payEntry.JobNumber = workAssignment.JobNumber;
            payEntry.WorkType = workAssignment.WorkType;
            payEntry.LoadQuantity = workAssignment.LoadQuantity;
            payEntry.JobProvince = workAssignment.JobProvince;
            payEntry.StandyHour = workAssignment.StandbyHours;
            payEntry.ClientCompany = workAssignment.ClientCompany;
            payEntry.ServiceLine = workAssignment.ServiceLine;
            payEntry.PayArea = workAssignment.PayArea;
            payEntry.IsTwoWay = workAssignment.IsTwoWay;
            payEntry.ModifiedUserName = workAssignment.ModifiedUserName;
            payEntry.PayPeriod = payPeriod;
            payEntry.PayType = payType;
            payEntry.Status = PayrollStatus.Approved;
            payEntry.Amount = Math.Round(baseVariablePay, 2, MidpointRounding.AwayFromZero) ;
            payEntry.PayPosition = payPosition;
            payEntry.PaySummaryType = payType.PaySummaryType;
            payEntry.PayPeriodSummary = payPeriodSummary;

            return payEntry;
        }

        private static PayEntry UpdatePayEntry(PayEntry existingPayEntry, PayEntry newPayEntry)
        {
            existingPayEntry.MtsNumber = newPayEntry.MtsNumber;
            existingPayEntry.Employee = newPayEntry.Employee;
            existingPayEntry.TravelTime = newPayEntry.TravelTime;
            existingPayEntry.JobRevenue = newPayEntry.JobRevenue;
            existingPayEntry.WorkAssignment = newPayEntry.WorkAssignment;
            existingPayEntry.WorkDescription = newPayEntry.WorkDescription;
            existingPayEntry.TravelDistance = newPayEntry.TravelDistance;
            existingPayEntry.PaySummaryType = newPayEntry.PaySummaryType;
            existingPayEntry.JobDate = newPayEntry.JobDate;
            existingPayEntry.HomeServicePoint = newPayEntry.HomeServicePoint;
            existingPayEntry.JobUniqueId = newPayEntry.JobUniqueId;
            existingPayEntry.WorkingServicePoint = newPayEntry.WorkingServicePoint;
            existingPayEntry.JobNumber = newPayEntry.JobNumber;
            existingPayEntry.WorkType = newPayEntry.WorkType;
            existingPayEntry.LoadQuantity = newPayEntry.LoadQuantity;
            existingPayEntry.JobProvince = newPayEntry.JobProvince;
            existingPayEntry.StandyHour = newPayEntry.StandyHour;
            existingPayEntry.ClientCompany = newPayEntry.ClientCompany;
            existingPayEntry.ServiceLine = newPayEntry.ServiceLine;
            existingPayEntry.PayArea = newPayEntry.PayArea;
            existingPayEntry.IsTwoWay = newPayEntry.IsTwoWay;
            existingPayEntry.ModifiedUserName = newPayEntry.ModifiedUserName;
            existingPayEntry.PayPeriod = newPayEntry.PayPeriod;
            existingPayEntry.PayType = newPayEntry.PayType;
            existingPayEntry.Status = newPayEntry.Status;
            existingPayEntry.Amount = Math.Round(newPayEntry.Amount, 2, MidpointRounding.AwayFromZero) ;
            existingPayEntry.PayPosition = newPayEntry.PayPosition;
            existingPayEntry.PaySummaryType = newPayEntry.PaySummaryType;
            existingPayEntry.PayPeriodSummary = newPayEntry.PayPeriodSummary;

            return existingPayEntry;

        }

        /* Ideally PayArea can be calculated from work location, however our current data quality is not sufficient yet.
            private static PayArea GetPayArea(string workAssignementWorkLocation)
            {
                throw new NotImplementedException();
            }
        */
        public static PayEntry CalculateCrewEfficiencyBonus(WorkAssignment workAssignment, PayPeriod payPeriod,
            PayType payType)
        {
            ICrewEfficiencyBonusPayScheduleService crewEfficiencyBonusPayScheduleService =
                ServiceFactory.Instance.GetService<ICrewEfficiencyBonusPayScheduleService>();
            CrewEfficiencyBonusPaySchedule crewEfficiencyBonusPaySchedule =
                crewEfficiencyBonusPayScheduleService.SelectByServiceLine(workAssignment.ServiceLine.Id).FirstOrDefault();
            if (crewEfficiencyBonusPaySchedule != null)
                return CalculateBaseVariablePay(workAssignment, payPeriod, crewEfficiencyBonusPaySchedule.PayAsPosition,
                    payType, crewEfficiencyBonusPaySchedule.PayRatio);
            return null;
        }

        public static PayEntry CalculateStandby(WorkAssignment workAssignment, PayPeriod payPeriod, PayType payType, PayPosition payPosition)
        {
            payPosition = VariablePayData.GetPayPosition(workAssignment, true);

            if (payPosition == null)
            {
                return null;
            }

            IStandbyPayScheduleService standbyPayScheduleService =
                ServiceFactory.Instance.GetService<IStandbyPayScheduleService>();
            if(standbyPayScheduleService == null) throw new Exception("StandbyPayScheduleService doesn't exist");

            StandbyPaySchedule standbyPaySchedule =
                standbyPayScheduleService.SelectByPayPosition(1, 1, payPosition.Id).FirstOrDefault();
            var payAmount = workAssignment.StandbyHours * standbyPaySchedule?.HoulyRate;
            return CreatePayEntry(workAssignment, payType, payPeriod, payAmount??0, payPosition);
        }


        public static PayEntry CalculateExtendTravelBonus(WorkAssignment workAssignment, PayPeriod payPeriod, PayType payType)
        {
            IExtendTravelPayScheduleService extendTravelPayScheduleService =
                ServiceFactory.Instance.GetService<IExtendTravelPayScheduleService>();

            if(extendTravelPayScheduleService == null) throw new Exception("ExtendTravelPayScheduleService doesn't exist");

            ExtendTravelPaySchedule extendTravelPaySchedule =
                extendTravelPayScheduleService.SelectAll(true).FirstOrDefault();
            if(extendTravelPaySchedule == null)  throw new Exception("ExtendTravelPaySchedule doesn't exist");

            if (workAssignment.TravelDistance > extendTravelPaySchedule.KmThreshold || workAssignment.TravelTime > extendTravelPaySchedule.HourThreshold)
            {
                var payAmount = extendTravelPaySchedule.BaseRate;
                return CreatePayEntry(workAssignment, payType, payPeriod, payAmount, null);
            }

            return null;
        }

        public static PayEntry CalculateRemedialTravelBonusAdjustment(WorkAssignment workAssignment, PayPeriod payPeriod, PayType payType)
        {
            IRTBAPayScheduleService rtbaPayScheduleService =
                ServiceFactory.Instance.GetService<IRTBAPayScheduleService>();

            if(rtbaPayScheduleService == null) throw new Exception("RTBAPayScheduleService doesn't exist");

            RTBAPaySchedule rtbaPaySchedule =
                rtbaPayScheduleService.SelectAll(true).FirstOrDefault();
            if(rtbaPaySchedule == null)  throw new Exception("RTBAPaySchedule doesn't exist");

            if (workAssignment.TravelDistance > rtbaPaySchedule.KmThreshold)
            {
                var payAmount = rtbaPaySchedule.BaseRate;
                return  CreatePayEntry(workAssignment, payType, payPeriod, payAmount, null);
            }

            return null;
        }

        public static PayEntry CalculateProductHaulPay(WorkAssignment workAssignment, PayPeriod payPeriod, PayType payType)
        {
            IProductHaulPayScheduleService productHaulPayScheduleService =
                ServiceFactory.Instance.GetService<IProductHaulPayScheduleService>();
            if(productHaulPayScheduleService == null) throw new Exception("ProductHaulPayScheduleService doesn't exist");

            ProductHaulPaySchedule productHaulPaySchedule =
                productHaulPayScheduleService.SelectByPayArea(workAssignment.PayArea.Id).FirstOrDefault();
            if(productHaulPaySchedule == null)  throw new Exception("ProductHaulPaySchedule doesn't exist");

            var payAmount = productHaulPaySchedule.PerHaulRate +
                            productHaulPaySchedule.PerKmRate * workAssignment.TravelDistance;

            return CreatePayEntry(workAssignment, payType, payPeriod, payAmount, null);
        }

        public static PayEntry CalculateExtendLocationPremium(WorkAssignment workAssignment, PayPeriod payPeriod, PayType payType, PayPosition payPosition)
        {
            if (payPosition == null)
            {
                return null;
            }
            IEXTLPayScheduleService extlPayScheduleService =
                ServiceFactory.Instance.GetService<IEXTLPayScheduleService>();
            if(extlPayScheduleService == null) throw new Exception("IEXTLPayScheduleService doesn't exist");
            EXTLPaySchedule extlPaySchedule =
                extlPayScheduleService.SelectByPayPosition(payPosition.Id).FirstOrDefault();
            if(extlPaySchedule == null) throw new Exception("EXTLPaySchedule doesn't exist");

            var payAmount = workAssignment.StandbyHours * extlPaySchedule.HourlyRate;

            return CreatePayEntry(workAssignment, payType, payPeriod, payAmount, payPosition);
        }
    }

}
