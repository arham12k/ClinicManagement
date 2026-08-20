using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Domain.Entities.Doctor
{
	public class DoctorAvailability
	{
		public Guid AvailabilityId { get; set; }

		public Guid DoctorId { get; set; }

		public string SessionType { get; set; } = string.Empty;

		public string DayOfWeek { get; set; } = string.Empty;

		public TimeSpan FromTime { get; set; }

		public TimeSpan ToTime { get; set; }
	}
}
