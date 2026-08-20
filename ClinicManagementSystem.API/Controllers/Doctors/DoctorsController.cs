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

		[HttpPost]
		public async Task<IActionResult> Register(CreateDoctorRequest request)
		{
			var doctorId = await _doctorService.CreateAsync(request);

			return Ok(new
			{
				doctorId,
				message = "Doctor registered successfully."
			});
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var doctors = await _doctorService.GetAllAsync();

			return Ok(doctors);
		}

		[HttpGet("{doctorId:guid}")]
		public async Task<IActionResult> GetById(Guid doctorId)
		{
			var doctor = await _doctorService.GetByIdAsync(doctorId);

			if (doctor == null)
				return NotFound();

			return Ok(doctor);
		}

		[HttpPut("{doctorId:guid}")]
		public async Task<IActionResult> Update(
			Guid doctorId,
			UpdateDoctorRequest request)
		{
			var updated = await _doctorService.UpdateAsync(
				doctorId,
				request);

			if (!updated)
				return NotFound();

			return Ok(new
			{
				Message = "Doctor updated successfully."
			});
		}

		[HttpDelete("{doctorId:guid}")]
		public async Task<IActionResult> Delete(Guid doctorId)
		{
			var deleted = await _doctorService.DeleteAsync(doctorId);

			return deleted ? NoContent() : NotFound();
		}
	}
}
