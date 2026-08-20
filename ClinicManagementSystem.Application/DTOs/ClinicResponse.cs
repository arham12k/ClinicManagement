using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.DTOs
{
	public class ClinicResponse
	{
		public Guid ClinicId { get; set; }

		public string ClinicName { get; set; } = string.Empty;

		public string OwnerName { get; set; } = string.Empty;

		public string MobileNumber { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;
	}
}
