using ClinicManagementSystem.Domain.Common;

namespace ClinicManagementSystem.Domain.Entities.Doctor;

public class Doctor: AuditableEntity
{
	public Guid DoctorId { get; set; }

	public Guid ClinicId { get; set; }

	public string FullName { get; set; } = string.Empty;

	public DateTime? DateOfBirth { get; set; }

	public string Gender { get; set; } = string.Empty;

	public string MobileNumber { get; set; } = string.Empty;

	public string Address { get; set; } = string.Empty;

	public string MedicalRegistrationNumber { get; set; } = string.Empty;

	public string RegistrationState { get; set; } = string.Empty;

	public string Specialization { get; set; } = string.Empty;

	public string SubSpecialization { get; set; } = string.Empty;

	public int ExperienceYears { get; set; }

	public string Qualification { get; set; } = string.Empty;

	public decimal ConsultationFees { get; set; }
}