using ClinicManagementSystem.Application.DTOs.Doctor;
using ClinicManagementSystem.Application.Interfaces.Doctors;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers.Doctors
{
	[ApiController]
	[Route("api/[controller]")]
	public class DoctorsController : ControllerBase
	{
		private readonly IDoctorService _doctorService;

		public DoctorsController(IDoctorService doctorService)
		{
			_doctorService = doctorService;
		}

		private const string ClinicIdHeader = "X-Clinic-Id";

		[HttpPost]
		public async Task<IActionResult> Register(
			[FromHeader(Name = ClinicIdHeader)] Guid clinicId,
			CreateDoctorRequest request)
		{
			if (clinicId == Guid.Empty)
				return BadRequest(new { success = false, message = $"{ClinicIdHeader} header is required." });

			var doctorId = await _doctorService.CreateAsync(clinicId, request);

			return Ok(new
			{
				success = true,
				doctorId,
				message = "Doctor registered successfully."
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetAll([FromHeader(Name = ClinicIdHeader)] Guid clinicId)
		{
			if (clinicId == Guid.Empty)
				return BadRequest(new { success = false, message = $"{ClinicIdHeader} header is required." });

			var doctors = await _doctorService.GetAllAsync(clinicId);

			return Ok(doctors);
		}

		[HttpGet("{doctorId:guid}")]
		public async Task<IActionResult> GetById(
			Guid doctorId,
			[FromHeader(Name = ClinicIdHeader)] Guid clinicId)
		{
			if (clinicId == Guid.Empty)
				return BadRequest(new { success = false, message = $"{ClinicIdHeader} header is required." });

			var doctor = await _doctorService.GetByIdAsync(doctorId, clinicId);

			if (doctor == null)
				return NotFound(new { success = false, message = "Doctor not found." });

			return Ok(doctor);
		}

		[HttpPut("{doctorId:guid}")]
		public async Task<IActionResult> Update(
			Guid doctorId,
			[FromHeader(Name = ClinicIdHeader)] Guid clinicId,
			UpdateDoctorRequest request)
		{
			if (clinicId == Guid.Empty)
				return BadRequest(new { success = false, message = $"{ClinicIdHeader} header is required." });

			var updated = await _doctorService.UpdateAsync(
				doctorId,
				clinicId,
				request);

			if (!updated)
				return NotFound(new { success = false, message = "Doctor not found." });

			return Ok(new
			{
				success = true,
				message = "Doctor updated successfully."
			});
		}

		[HttpDelete("{doctorId:guid}")]
		public async Task<IActionResult> Delete(
			Guid doctorId,
			[FromHeader(Name = ClinicIdHeader)] Guid clinicId)
		{
			if (clinicId == Guid.Empty)
				return BadRequest(new { success = false, message = $"{ClinicIdHeader} header is required." });

			var deleted = await _doctorService.DeleteAsync(doctorId, clinicId);

			return deleted
				? Ok(new { success = true, message = "Doctor deleted successfully." })
				: NotFound(new { success = false, message = "Doctor not found." });
		}
	}
}
