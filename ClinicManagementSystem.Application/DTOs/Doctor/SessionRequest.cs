using System.Collections.Generic;

namespace ClinicManagementSystem.Application.DTOs.Doctor
{
	public class SessionRequest
	{
		public List<string> Days { get; set; } = new();

		public string From { get; set; } = string.Empty;

		public string To { get; set; } = string.Empty;
	}
}
