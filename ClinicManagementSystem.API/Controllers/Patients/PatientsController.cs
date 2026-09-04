using ClinicManagementSystem.Application.DTOs.Patient;
using ClinicManagementSystem.Application.Interfaces.Patients;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers.Patients;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
	private readonly IPatientService _patientService;

	public PatientsController(IPatientService patientService)
	{
		_patientService = patientService;
	}

	private const string ClinicIdHeader = "X-Clinic-Id";
	private const string DoctorIdHeader = "X-Doctor-Id";

	[HttpPost]
	public async Task<IActionResult> Register(
		[FromHeader(Name = ClinicIdHeader)] Guid clinicId,
		[FromHeader(Name = DoctorIdHeader)] Guid? doctorId,
		RegisterPatientRequest request)
	{
		if (clinicId == Guid.Empty)
			return BadRequest(new { success = false, message = $"{ClinicIdHeader} header is required." });

		var result = await _patientService.RegisterAsync(clinicId, doctorId, request);

		return CreatedAtAction(nameof(GetById), new { patientId = result.PatientId }, new
		{
			success = true,
			result.PatientId,
			result.PatientToken,
			message = "Patient registered successfully."
		});
	}

	[HttpGet]
	public async Task<IActionResult> GetAll(
		[FromHeader(Name = ClinicIdHeader)] Guid clinicId,
		[FromHeader(Name = DoctorIdHeader)] Guid? doctorId)
	{
		if (clinicId == Guid.Empty)
			return BadRequest(new { success = false, message = $"{ClinicIdHeader} header is required." });

		return Ok(await _patientService.GetAllAsync(clinicId, doctorId));
	}

	[HttpGet("{patientId:guid}")]
	public async Task<IActionResult> GetById(Guid patientId)
	{
		var patient = await _patientService.GetByIdAsync(patientId);
		return patient == null
			? NotFound(new { success = false, message = "Patient not found." })
			: Ok(patient);
	}

	[HttpPut("{patientId:guid}")]
	public async Task<IActionResult> Update(Guid patientId, UpdatePatientRequest request)
	{
		var updated = await _patientService.UpdateAsync(patientId, request);
		return updated
			? Ok(new { success = true, message = "Patient updated successfully." })
			: NotFound(new { success = false, message = "Patient not found." });
	}

	[HttpDelete("{patientId:guid}")]
	public async Task<IActionResult> Delete(Guid patientId)
	{
		var deleted = await _patientService.DeleteAsync(patientId);
		return deleted
			? Ok(new { success = true, message = "Patient deleted successfully." })
			: NotFound(new { success = false, message = "Patient not found." });
	}

	[HttpGet("{patientId:guid}/visits")]
	public async Task<IActionResult> GetVisits(Guid patientId)
	{
		var visits = await _patientService.GetVisitsAsync(patientId);
		return visits == null
			? NotFound(new { success = false, message = "Patient not found." })
			: Ok(visits);
	}

	[HttpPost("{patientId:guid}/visits")]
	public async Task<IActionResult> AddVisit(
		Guid patientId,
		[FromHeader(Name = DoctorIdHeader)] Guid? doctorId,
		CreatePatientVisitRequest request)
	{
		var visitId = await _patientService.CreateVisitAsync(patientId, doctorId, request);
		return visitId == null
			? NotFound(new { success = false, message = "Patient not found." })
			: CreatedAtAction(nameof(GetVisits), new { patientId }, new { success = true, PatientVisitId = visitId, message = "Visit added successfully." });
	}
}