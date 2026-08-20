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

	[HttpPost]
	public async Task<IActionResult> Register(RegisterPatientRequest request)
	{
		var result = await _patientService.RegisterAsync(request);

		return CreatedAtAction(nameof(GetById), new { patientId = result.PatientId }, new
		{
			result.PatientId,
			result.PatientToken,
			message = "Patient registered successfully."
		});
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() => Ok(await _patientService.GetAllAsync());

	[HttpGet("{patientId:guid}")]
	public async Task<IActionResult> GetById(Guid patientId)
	{
		var patient = await _patientService.GetByIdAsync(patientId);
		return patient == null ? NotFound() : Ok(patient);
	}

	[HttpPut("{patientId:guid}")]
	public async Task<IActionResult> Update(Guid patientId, UpdatePatientRequest request)
	{
		return await _patientService.UpdateAsync(patientId, request) ? NoContent() : NotFound();
	}

	[HttpDelete("{patientId:guid}")]
	public async Task<IActionResult> Delete(Guid patientId)
	{
		return await _patientService.DeleteAsync(patientId) ? NoContent() : NotFound();
	}

	[HttpGet("{patientId:guid}/visits")]
	public async Task<IActionResult> GetVisits(Guid patientId)
	{
		var visits = await _patientService.GetVisitsAsync(patientId);
		return visits == null ? NotFound() : Ok(visits);
	}

	[HttpPost("{patientId:guid}/visits")]
	public async Task<IActionResult> AddVisit(Guid patientId, CreatePatientVisitRequest request)
	{
		var visitId = await _patientService.CreateVisitAsync(patientId, request);
		return visitId == null ? NotFound() : CreatedAtAction(nameof(GetVisits), new { patientId }, new { PatientVisitId = visitId });
	}
}