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

	public async Task<bool> DeleteAsync(Guid clinicId)
	{
		return await _repository.DeleteAsync(clinicId);
	}

	public async Task<IEnumerable<ClinicResponse>> GetAllAsync()
	{
		var clinics = await _repository.GetAllAsync();

		return clinics.Select(MapClinicResponse);
	}

	public async Task<ClinicResponse?> GetByIdAsync(Guid clinicId)
	{
		var clinic = await _repository.GetByIdAsync(clinicId);

		return clinic == null ? null : MapClinicResponse(clinic);
	}

	public async Task<bool> UpdateAsync(Guid clinicId, CreateClinicRequest request)
	{
		var clinic = await _repository.GetByIdAsync(clinicId);

		if (clinic == null)
			return false;

		var existingClinic = await _repository.GetByEmailAsync(request.Email);

		if (existingClinic != null && existingClinic.ClinicId != clinicId)
			throw new Exception("Clinic already exists with this email.");

		clinic.ClinicName = request.ClinicName;
		clinic.OwnerName = request.OwnerName;
		clinic.MobileNumber = request.MobileNumber;
		clinic.Email = request.Email;
		clinic.Address = request.Address;
		clinic.City = request.City;
		clinic.State = request.State;
		clinic.Pincode = request.Pincode;
		clinic.UpdatedOn = DateTime.UtcNow;

		return await _repository.UpdateAsync(clinic);
	}

	private static ClinicResponse MapClinicResponse(Clinic clinic) => new()
	{
		ClinicId = clinic.ClinicId,
		ClinicName = clinic.ClinicName,
		OwnerName = clinic.OwnerName,
		MobileNumber = clinic.MobileNumber,
		Email = clinic.Email,
		Address = clinic.Address,
		City = clinic.City,
		State = clinic.State,
		Pincode = clinic.Pincode
	};
}