using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.DTOs
{
	public class CreateClinicRequest
	{
		public string ClinicName { get; set; } = string.Empty;

		public string OwnerName { get; set; } = string.Empty;

		public string MobileNumber { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string Address { get; set; } = string.Empty;

		public string City { get; set; } = string.Empty;

		public string State { get; set; } = string.Empty;

		public string Pincode { get; set; } = string.Empty;
	}
}
