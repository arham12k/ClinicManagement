using ClinicManagementSystem.Application.DTOs.Doctor;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Application.Interfaces.Doctors;
using ClinicManagementSystem.Domain.Entities.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.Services.Doctors
{
	public class DoctorService : IDoctorService
	{
		private readonly IDoctorRepository _doctorRepository;
		private readonly IClinicRepository _clinicRepository;

		public DoctorService(
			IDoctorRepository doctorRepository,
			IClinicRepository clinicRepository)
		{
			_doctorRepository = doctorRepository;
			_clinicRepository = clinicRepository;
		}

		public async Task<Guid> CreateAsync(CreateDoctorRequest request)
		{
			// Check if clinic exists
			var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId);

			if (clinic == null)
				throw new Exception("Clinic not found.");

			// Check duplicate registration number
			var existingDoctor = await _doctorRepository
				.GetByMedicalRegistrationNumberAsync(request.MedicalRegistrationNumber);

			if (existingDoctor != null)
				throw new Exception("Doctor already exists with this registration number.");

			var doctor = new Doctor
			{
				DoctorId = Guid.NewGuid(),
				ClinicId = request.ClinicId,
				FullName = request.FullName,
				DateOfBirth = request.DateOfBirth,
				Gender = request.Gender,
				MobileNumber = request.MobileNumber,
				Address = request.Address,
				MedicalRegistrationNumber = request.MedicalRegistrationNumber,
				RegistrationState = request.RegistrationState,
				Specialization = request.Specialization,
				SubSpecialization = request.SubSpecialization,
				ExperienceYears = request.ExperienceYears,
				Qualification = request.Qualification,
				ConsultationFees = request.ConsultationFees,
				CreatedOn = DateTime.UtcNow,
				IsActive = true
			};

			var availability = BuildAvailability(
				doctor.DoctorId,
				request.MorningSession,
				request.EveningSession);

			return await _doctorRepository.CreateAsync(
				doctor,
				availability);
		}

		private List<DoctorAvailability> BuildAvailability(
			Guid doctorId,
			SessionRequest? morning,
			SessionRequest? evening)
		{
			var availability = new List<DoctorAvailability>();

			if (morning != null)
			{
				foreach (var day in morning.Days)
				{
					availability.Add(new DoctorAvailability
					{
						AvailabilityId = Guid.NewGuid(),
						DoctorId = doctorId,
						SessionType = "Morning",
						DayOfWeek = day,
						FromTime = morning.From,
						ToTime = morning.To
					});
				}
			}

			if (evening != null)
			{
				foreach (var day in evening.Days)
				{
					availability.Add(new DoctorAvailability
					{
						AvailabilityId = Guid.NewGuid(),
						DoctorId = doctorId,
						SessionType = "Evening",
						DayOfWeek = day,
						FromTime = evening.From,
						ToTime = evening.To
					});
				}
			}

			return availability;
		}

		public async Task<IEnumerable<DoctorResponse>> GetAllAsync()
		{
			var doctors = await _doctorRepository.GetAllAsync();

			var response = new List<DoctorResponse>();

			foreach (var doctor in doctors)
			{
				var availability = await _doctorRepository.GetAvailabilityAsync(doctor.DoctorId);

				response.Add(MapDoctorResponse(doctor, availability));
			}

			return response;
		}

		public async Task<DoctorResponse?> GetByIdAsync(Guid doctorId)
		{
			var doctor = await _doctorRepository.GetByIdAsync(doctorId);

			if (doctor == null)
				return null;

			var availability = await _doctorRepository.GetAvailabilityAsync(doctorId);

			return MapDoctorResponse(doctor, availability);
		}

		public async Task<bool> UpdateAsync(
	Guid doctorId,
	UpdateDoctorRequest request)
		{
			var doctor = await _doctorRepository.GetByIdAsync(doctorId);

			if (doctor == null)
				return false;

			doctor.FullName = request.FullName;
			doctor.DateOfBirth = request.DateOfBirth;
			doctor.Gender = request.Gender;
			doctor.MobileNumber = request.MobileNumber;
			doctor.Address = request.Address;
			doctor.RegistrationState = request.RegistrationState;
			doctor.Specialization = request.Specialization;
			doctor.SubSpecialization = request.SubSpecialization;
			doctor.ExperienceYears = request.ExperienceYears;
			doctor.Qualification = request.Qualification;
			doctor.ConsultationFees = request.ConsultationFees;
			doctor.UpdatedOn = DateTime.UtcNow;

			var availability = BuildAvailability(
				doctor.DoctorId,
				request.MorningSession,
				request.EveningSession);

			return await _doctorRepository.UpdateAsync(
				doctor,
				availability);
		}




		private DoctorResponse MapDoctorResponse(
	Doctor doctor,
	IEnumerable<DoctorAvailability> availability)
		{
			var response = new DoctorResponse
			{
				DoctorId = doctor.DoctorId,
				ClinicId = doctor.ClinicId,
				FullName = doctor.FullName,
				DateOfBirth = doctor.DateOfBirth,
				Gender = doctor.Gender,
				MobileNumber = doctor.MobileNumber,
				Address = doctor.Address,
				MedicalRegistrationNumber = doctor.MedicalRegistrationNumber,
				RegistrationState = doctor.RegistrationState,
				Specialization = doctor.Specialization,
				SubSpecialization = doctor.SubSpecialization,
				ExperienceYears = doctor.ExperienceYears,
				Qualification = doctor.Qualification,
				ConsultationFees = doctor.ConsultationFees
			};

			var groupedAvailability = availability
				.GroupBy(a => a.SessionType);

			foreach (var group in groupedAvailability)
			{
				response.Availability.Add(new SessionResponse
				{
					SessionType = group.Key,
					Days = group.Select(x => x.DayOfWeek).ToList(),
					From = group.First().FromTime,
					To = group.First().ToTime
				});
			}

			return response;
		}


	}
}
