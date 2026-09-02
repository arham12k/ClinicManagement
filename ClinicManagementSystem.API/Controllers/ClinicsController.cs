using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicsController : ControllerBase
{
	private readonly IClinicService _clinicService;

	public ClinicsController(IClinicService clinicService)
	{
		_clinicService = clinicService;
	}

	[HttpPost]
	public async Task<IActionResult> Register(CreateClinicRequest request)
	{
		var clinicId = await _clinicService.CreateAsync(request);

		return CreatedAtAction(
			nameof(GetById),
			new { clinicId },
			new
			{
				success = true,
				clinicId,
				message = "Clinic registered successfully."
			});
	}

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		return Ok(await _clinicService.GetAllAsync());
	}

	[HttpGet("{clinicId:guid}")]
	public async Task<IActionResult> GetById(Guid clinicId)
	{
		var clinic = await _clinicService.GetByIdAsync(clinicId);

		return clinic == null
			? NotFound(new { success = false, message = "Clinic not found." })
			: Ok(clinic);
	}

	[HttpPut("{clinicId:guid}")]
	public async Task<IActionResult> Update(Guid clinicId, CreateClinicRequest request)
	{
		var updated = await _clinicService.UpdateAsync(clinicId, request);

		return updated
			? Ok(new { success = true, message = "Clinic updated successfully." })
			: NotFound(new { success = false, message = "Clinic not found." });
	}

	[HttpDelete("{clinicId:guid}")]
	public async Task<IActionResult> Delete(Guid clinicId)
	{
		var deleted = await _clinicService.DeleteAsync(clinicId);

		return deleted
			? Ok(new { success = true, message = "Clinic deleted successfully." })
			: NotFound(new { success = false, message = "Clinic not found." });
	}
}