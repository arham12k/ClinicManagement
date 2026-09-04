namespace ClinicManagementSystem.Application.DTOs.Patient;

public class PatientVisitResponse
{
	public Guid PatientVisitId { get; set; }
	public Guid? DoctorId { get; set; }
	public string VisitPriority { get; set; } = string.Empty;
	public string BloodPressure { get; set; } = string.Empty;
	public string SugarLevel { get; set; } = string.Empty;
	public string Weight { get; set; } = string.Empty;
	public string Height { get; set; } = string.Empty;
	public string Temperature { get; set; } = string.Empty;
	public string PulseRate { get; set; } = string.Empty;
	public string SpO2 { get; set; } = string.Empty;
	public string RespiratoryRate { get; set; } = string.Empty;
	public DateTime CreatedOn { get; set; }
}