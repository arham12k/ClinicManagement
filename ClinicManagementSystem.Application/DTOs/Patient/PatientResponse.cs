namespace ClinicManagementSystem.Application.DTOs.Patient;

public class PatientResponse
{
	public Guid PatientId { get; set; }
	public Guid ClinicId { get; set; }
	public string PatientToken { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public int Age { get; set; }
	public string Gender { get; set; } = string.Empty;
	public string MobileNumber { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string Allergies { get; set; } = string.Empty;
	public string ExistingDiseases { get; set; } = string.Empty;
	public string CurrentMedications { get; set; } = string.Empty;
	public string PastSurgeries { get; set; } = string.Empty;
	public string AncProfile { get; set; } = string.Empty;
	public DateOnly? LmpDate { get; set; }
	public string GestationalAge { get; set; } = string.Empty;
	public DateOnly? ExpectedDeliveryDate { get; set; }
	public string Trimester { get; set; } = string.Empty;
	public List<PatientVisitResponse> Visits { get; set; } = [];
}