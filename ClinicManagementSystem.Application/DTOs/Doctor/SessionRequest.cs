using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.DTOs.Doctor
{
	public class SessionRequest
	{
		public List<string> Days { get; set; } = new();

		public TimeOnly From { get; set; }

		public TimeOnly To { get; set; }
	}
}
