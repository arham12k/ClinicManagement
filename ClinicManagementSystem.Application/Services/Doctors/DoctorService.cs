using ClinicManagementSystem.Application.DTOs.Doctor;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Application.Interfaces.Doctors;
using ClinicManagementSystem.Domain.Entities.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
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
			// Check if clinic exists (only if ClinicId is provided)
			if (request.ClinicId.HasValue && request.ClinicId.Value != Guid.Empty)
			{
				var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId.Value);

				if (clinic == null)
					throw new Exception("Clinic not found.");
			}

			// Check duplicate registration number
			if (!string.IsNullOrWhiteSpace(request.MedicalRegistrationNumber))
			{
				var existingDoctor = await _doctorRepository
					.GetByMedicalRegistrationNumberAsync(request.MedicalRegistrationNumber);

				if (existingDoctor != null)
					throw new Exception("Doctor already exists with this registration number.");
			}

			var availableDaysStr = request.AvailableDays != null && request.AvailableDays.Any()
				? string.Join(",", request.AvailableDays)
				: string.Empty;

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
				Experience = request.Experience,
				Qualification = request.Qualification,
				ConsultationFees = request.ConsultationFees,
				AvailableDays = availableDaysStr,
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

			if (request.ClinicId.HasValue && request.ClinicId.Value != Guid.Empty)
			{
				doctor.ClinicId = request.ClinicId;
			}

			var availableDaysStr = request.AvailableDays != null && request.AvailableDays.Any()
				? string.Join(",", request.AvailableDays)
				: string.Empty;

			doctor.FullName = request.FullName;
			doctor.DateOfBirth = request.DateOfBirth;
			doctor.Gender = request.Gender;
			doctor.MobileNumber = request.MobileNumber;
			doctor.Address = request.Address;
			doctor.RegistrationState = request.RegistrationState;
			doctor.Specialization = request.Specialization;
			doctor.SubSpecialization = request.SubSpecialization;
			doctor.Experience = request.Experience;
			doctor.Qualification = request.Qualification;
			doctor.ConsultationFees = request.ConsultationFees;
			doctor.AvailableDays = availableDaysStr;
			doctor.UpdatedOn = DateTime.UtcNow;

			var availability = BuildAvailability(
				doctor.DoctorId,
				request.MorningSession,
				request.EveningSession);

			return await _doctorRepository.UpdateAsync(
				doctor,
				availability);
		}

		public async Task<bool> DeleteAsync(Guid doctorId)
		{
			return await _doctorRepository.DeleteAsync(doctorId);
		}

		private List<DoctorAvailability> BuildAvailability(
			Guid doctorId,
			SessionRequest? morning,
			SessionRequest? evening)
		{
			var availability = new List<DoctorAvailability>();

			if (morning != null && morning.Days != null)
			{
				var fromTime = ParseTime(morning.From);
				var toTime = ParseTime(morning.To);

				foreach (var day in morning.Days)
				{
					availability.Add(new DoctorAvailability
					{
						AvailabilityId = Guid.NewGuid(),
						DoctorId = doctorId,
						SessionType = "Morning",
						DayOfWeek = day,
						FromTime = fromTime,
						ToTime = toTime
					});
				}
			}

			if (evening != null && evening.Days != null)
			{
				var fromTime = ParseTime(evening.From);
				var toTime = ParseTime(evening.To);

				foreach (var day in evening.Days)
				{
					availability.Add(new DoctorAvailability
					{
						AvailabilityId = Guid.NewGuid(),
						DoctorId = doctorId,
						SessionType = "Evening",
						DayOfWeek = day,
						FromTime = fromTime,
						ToTime = toTime
					});
				}
			}

			return availability;
		}

		private TimeOnly ParseTime(string timeStr)
		{
			if (string.IsNullOrWhiteSpace(timeStr))
				return TimeOnly.MinValue;

			if (TimeOnly.TryParse(timeStr, out var time))
				return time;

			return TimeOnly.MinValue;
		}

		private DoctorResponse MapDoctorResponse(
			Doctor doctor,
			IEnumerable<DoctorAvailability> availabilities)
		{
			var availabilityList = availabilities.ToList();

			var availableDaysList = !string.IsNullOrWhiteSpace(doctor.AvailableDays)
				? doctor.AvailableDays.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()).ToList()
				: new List<string>();

			var morningItems = availabilityList
				.Where(a => string.Equals(a.SessionType, "Morning", StringComparison.OrdinalIgnoreCase))
				.ToList();

			var eveningItems = availabilityList
				.Where(a => string.Equals(a.SessionType, "Evening", StringComparison.OrdinalIgnoreCase))
				.ToList();

			SessionResponse? morningSession = null;
			if (morningItems.Any())
			{
				morningSession = new SessionResponse
				{
					Days = morningItems.Select(m => m.DayOfWeek).Distinct().ToList(),
					From = morningItems.First().FromTime.ToString("HH:mm"),
					To = morningItems.First().ToTime.ToString("HH:mm")
				};
			}

			SessionResponse? eveningSession = null;
			if (eveningItems.Any())
			{
				eveningSession = new SessionResponse
				{
					Days = eveningItems.Select(e => e.DayOfWeek).Distinct().ToList(),
					From = eveningItems.First().FromTime.ToString("HH:mm"),
					To = eveningItems.First().ToTime.ToString("HH:mm")
				};
			}

			return new DoctorResponse
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
				Experience = doctor.Experience,
				Qualification = doctor.Qualification,
				ConsultationFees = doctor.ConsultationFees,
				AvailableDays = availableDaysList,
				MorningSession = morningSession,
				EveningSession = eveningSession,
				Availability = availabilityList.Select(a => new DoctorAvailabilityItemResponse
				{
					AvailabilityId = a.AvailabilityId,
					SessionType = a.SessionType,
					DayOfWeek = a.DayOfWeek,
					FromTime = a.FromTime.ToString("HH:mm"),
					ToTime = a.ToTime.ToString("HH:mm")
				}).ToList()
			};
		}
	}
}
