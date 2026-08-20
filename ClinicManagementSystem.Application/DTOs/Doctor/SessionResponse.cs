using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.DTOs.Doctor
{
	public class SessionResponse
	{
		public string SessionType { get; set; } = string.Empty;

		public List<string> Days { get; set; } = new();

		public TimeOnly From { get; set; }

		public TimeOnly To { get; set; }
	}
}
