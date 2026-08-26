using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.Application.DTOs.Doctor
{
	public class DoctorResponse
	{
		public Guid DoctorId { get; set; }

		public Guid? ClinicId { get; set; }

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

		public decimal ConsultationFees { get; set; }

		public List<string> AvailableDays { get; set; } = new();

		public SessionResponse? MorningSession { get; set; }

		public SessionResponse? EveningSession { get; set; }

		public List<DoctorAvailabilityItemResponse> Availability { get; set; } = new();
	}

	public class DoctorAvailabilityItemResponse
	{
		public Guid AvailabilityId { get; set; }

		public string SessionType { get; set; } = string.Empty;

		public string DayOfWeek { get; set; } = string.Empty;

		public string FromTime { get; set; } = string.Empty;

		public string ToTime { get; set; } = string.Empty;
	}
}
