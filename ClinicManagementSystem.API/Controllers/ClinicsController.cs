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
			nameof(Register),
			new { clinicId },
			new
			{
				clinicId,
				message = "Clinic registered successfully."
			});
	}
}