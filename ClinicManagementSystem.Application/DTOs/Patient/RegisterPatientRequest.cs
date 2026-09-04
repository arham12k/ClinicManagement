using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Application.DTOs.Patient;

public class RegisterPatientRequest
{
	[Required]
	public string FullName { get; set; } = string.Empty;
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	[Range(1, int.MaxValue)]
	public int Age { get; set; }
	[Required]
	public string Gender { get; set; } = string.Empty;
	[Required]
	public string VisitPriority { get; set; } = string.Empty;
	[Required]
	public string MobileNumber { get; set; } = string.Empty;
	public string? Address { get; set; }
	public string? BloodPressure { get; set; }
	public string? SugarLevel { get; set; }
	public string? Weight { get; set; }
	public string? Height { get; set; }
	public string? Temperature { get; set; }
	public string? PulseRate { get; set; }
	public string? SpO2 { get; set; }
	public string? RespiratoryRate { get; set; }
	public string? Allergies { get; set; }
	public string? ExistingDiseases { get; set; }
	public string? CurrentMedications { get; set; }
	public string? PastSurgeries { get; set; }
	public string? AncProfile { get; set; }
	public DateOnly? LmpDate { get; set; }
	public string? GestationalAge { get; set; }
	public DateOnly? ExpectedDeliveryDate { get; set; }
	public string? Trimester { get; set; }
}