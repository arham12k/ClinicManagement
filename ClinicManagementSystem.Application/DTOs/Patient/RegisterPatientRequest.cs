using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Application.DTOs.Patient;

public class RegisterPatientRequest
{
	public Guid ClinicId { get; set; }
	public string FullName { get; set; } = string.Empty;
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	public int Age { get; set; }
	public string Gender { get; set; } = string.Empty;
	public string VisitPriority { get; set; } = string.Empty;
	public string MobileNumber { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string BloodPressure { get; set; } = string.Empty;
	public string SugarLevel { get; set; } = string.Empty;
	public string Weight { get; set; } = string.Empty;
	public string Height { get; set; } = string.Empty;
	public string Temperature { get; set; } = string.Empty;
	public string PulseRate { get; set; } = string.Empty;
	public string SpO2 { get; set; } = string.Empty;
	public string RespiratoryRate { get; set; } = string.Empty;
	public string Allergies { get; set; } = string.Empty;
	public string ExistingDiseases { get; set; } = string.Empty;
	public string CurrentMedications { get; set; } = string.Empty;
	public string PastSurgeries { get; set; } = string.Empty;
	public string AncProfile { get; set; } = string.Empty;
	public DateOnly? LmpDate { get; set; }
	public string GestationalAge { get; set; } = string.Empty;
	public DateOnly? ExpectedDeliveryDate { get; set; }
	public string Trimester { get; set; } = string.Empty;
}