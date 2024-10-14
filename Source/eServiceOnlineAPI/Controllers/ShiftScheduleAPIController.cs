using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using eServiceOnline.Gateway;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ResourceSchedule;

namespace eServiceOnline.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]

	public class ShiftScheduleAPIController : ControllerBase
	{
		//Sample: http://localhost:5000/ShiftScheduleAPI/ExtendShiftScheduleEndDate?employeeId=20655
		public async Task<ActionResult> ExtendShiftScheduleEndDate(int employeeId, int rotationId, DateTime startDateTime, DateTime endDateTime)
		{
			int rotationIndex = eServiceOnlineGateway.Instance.GetRotationTemplateById(rotationId).RotationIndex;
			int result = eServiceOnlineGateway.Instance.UpdateWorkRotationSchedule(employeeId, rotationIndex, startDateTime, endDateTime);
			return new JsonResult(new { Succeed = result });
		}


		//Sample: http://localhost:5000/ShiftScheduleAPI/GetShiftScheduleEndDate?employeeId=20655
		public ActionResult GetShiftScheduleEndDate(int employeeId)
		{
			DateTime today = DateTime.Now;
			List<WorkerSchedule> workerSchedules = eServiceOnlineGateway.Instance.GetWorkerSchedulesByQuery(p => p.Worker.Id == employeeId && p.EndTime > today && (p.Type == WorkerScheduleType.OffShift || p.Type == WorkerScheduleType.OnShift));
			WorkerSchedule workerSchedule = (workerSchedules == null || workerSchedules.Count == 0) ? null : workerSchedules.OrderByDescending(p => p.EndTime).First();
			if (workerSchedule == null)
				return new JsonResult("No shift schedules found.");
			else
				return new JsonResult(new { EmployeeId = employeeId, ScheduleEndTime = workerSchedule.EndTime, ScheduleType = workerSchedule.Type.GetDescription(), RotationId = workerSchedule.Rotation.Id });
		}

	}

	public static class EnumExtensions
	{
		public static string GetDescription(this Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());
			if (field == null)
				return value.ToString();

			DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
			return attribute != null ? attribute.Description : value.ToString();
		}
	}

}
