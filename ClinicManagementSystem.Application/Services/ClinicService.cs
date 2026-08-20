using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.Services;

public class ClinicService : IClinicService
{
	private readonly IClinicRepository _repository;

	public ClinicService(IClinicRepository repository)
	{
		_repository = repository;
	}

	public async Task<Guid> CreateAsync(CreateClinicRequest request)
	{
		var existingClinic = await _repository.GetByEmailAsync(request.Email);

		if (existingClinic != null)
			throw new Exception("Clinic already exists with this email.");

		var clinic = new Clinic
		{
			ClinicId = Guid.NewGuid(),
			ClinicName = request.ClinicName,
			OwnerName = request.OwnerName,
			MobileNumber = request.MobileNumber,
			Email = request.Email,
			Address = request.Address,
			City = request.City,
			State = request.State,
			Pincode = request.Pincode,
			CreatedOn = DateTime.UtcNow,
			IsActive = true
		};

		return await _repository.CreateAsync(clinic);
	}

	public Task<bool> DeleteAsync(Guid clinicId)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<ClinicResponse>> GetAllAsync()
	{
		throw new NotImplementedException();
	}

	public Task<ClinicResponse?> GetByIdAsync(Guid clinicId)
	{
		throw new NotImplementedException();
	}

	public Task<bool> UpdateAsync(Guid clinicId, CreateClinicRequest request)
	{
		throw new NotImplementedException();
	}
}