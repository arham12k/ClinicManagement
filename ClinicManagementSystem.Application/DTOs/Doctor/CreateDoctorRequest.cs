using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Application.DTOs.Doctor
{
	public class CreateDoctorRequest
	{
		public string FullName { get; set; } = string.Empty;

		public DateOnly? DateOfBirth { get; set; }

		public string Gender { get; set; } = string.Empty;

		public string MobileNumber { get; set; } = string.Empty;

		public string Address { get; set; } = string.Empty;

		public string MedicalRegistrationNumber { get; set; } = string.Empty;

		public string RegistrationState { get; set; } = string.Empty;

		public string Specialization { get; set; } = string.Empty;

		public string SubSpecialization { get; set; } = string.Empty;

		public string Experience { get; set; } = string.Empty;

		public string Qualification { get; set; } = string.Empty;

		[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
		public decimal ConsultationFees { get; set; }

		public List<string> AvailableDays { get; set; } = new();

		public SessionRequest? MorningSession { get; set; }

		public SessionRequest? EveningSession { get; set; }
	}
}
