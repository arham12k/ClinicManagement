using ClinicManagementSystem.Application.DTOs.Patient;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Application.Interfaces.Patients;
using ClinicManagementSystem.Domain.Entities.Patient;

namespace ClinicManagementSystem.Application.Services.Patients;

public class PatientService : IPatientService
{
	private readonly IPatientRepository _patientRepository;
	private readonly IClinicRepository _clinicRepository;

	public PatientService(IPatientRepository patientRepository, IClinicRepository clinicRepository)
	{
		_patientRepository = patientRepository;
		_clinicRepository = clinicRepository;
	}

	public async Task<(Guid PatientId, string PatientToken)> RegisterAsync(RegisterPatientRequest request)
	{
		var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId);

		if (clinic == null)
			throw new Exception("Clinic not found.");

		var patient = new Patient
		{
			PatientId = Guid.NewGuid(),
			ClinicId = request.ClinicId,
			PatientToken = $"PT-{Guid.NewGuid():N}".ToUpperInvariant(),
			FullName = request.FullName,
			Age = request.Age,
			Gender = request.Gender,
			MobileNumber = request.MobileNumber,
			Address = request.Address,
			Allergies = request.Allergies,
			ExistingDiseases = request.ExistingDiseases,
			CurrentMedications = request.CurrentMedications,
			PastSurgeries = request.PastSurgeries,
			AncProfile = request.AncProfile,
			LmpDate = request.LmpDate,
			GestationalAge = request.GestationalAge,
			ExpectedDeliveryDate = request.ExpectedDeliveryDate,
			Trimester = request.Trimester,
			IsActive = true,
			CreatedOn = DateTime.UtcNow
		};

		var firstVisit = CreateVisit(patient.PatientId, request);
		await _patientRepository.CreateAsync(patient, firstVisit);

		return (patient.PatientId, patient.PatientToken);
	}

	public async Task<IEnumerable<PatientResponse>> GetAllAsync()
	{
		var patients = await _patientRepository.GetAllAsync();
		return patients.Select(MapPatient);
	}

	public async Task<PatientResponse?> GetByIdAsync(Guid patientId)
	{
		var patient = await _patientRepository.GetByIdAsync(patientId);

		if (patient == null)
			return null;

		var response = MapPatient(patient);
		response.Visits = (await _patientRepository.GetVisitsAsync(patientId)).Select(MapVisit).ToList();

		return response;
	}

	public async Task<IEnumerable<PatientVisitResponse>?> GetVisitsAsync(Guid patientId)
	{
		var patient = await _patientRepository.GetByIdAsync(patientId);

		return patient == null
			? null
			: (await _patientRepository.GetVisitsAsync(patientId)).Select(MapVisit);
	}

	public async Task<Guid?> CreateVisitAsync(Guid patientId, CreatePatientVisitRequest request)
	{
		var patient = await _patientRepository.GetByIdAsync(patientId);

		if (patient == null)
			return null;

		var visit = CreateVisit(patientId, request);
		return await _patientRepository.CreateVisitAsync(visit);
	}

	public async Task<bool> UpdateAsync(Guid patientId, UpdatePatientRequest request)
	{
		var patient = await _patientRepository.GetByIdAsync(patientId);

		if (patient == null)
			return false;

		patient.FullName = request.FullName;
		patient.Age = request.Age;
		patient.Gender = request.Gender;
		patient.MobileNumber = request.MobileNumber;
		patient.Address = request.Address;
		patient.Allergies = request.Allergies;
		patient.ExistingDiseases = request.ExistingDiseases;
		patient.CurrentMedications = request.CurrentMedications;
		patient.PastSurgeries = request.PastSurgeries;
		patient.AncProfile = request.AncProfile;
		patient.LmpDate = request.LmpDate;
		patient.GestationalAge = request.GestationalAge;
		patient.ExpectedDeliveryDate = request.ExpectedDeliveryDate;
		patient.Trimester = request.Trimester;
		patient.UpdatedOn = DateTime.UtcNow;

		return await _patientRepository.UpdateAsync(patient);
	}

	public Task<bool> DeleteAsync(Guid patientId) => _patientRepository.DeleteAsync(patientId);

	private static PatientVisit CreateVisit(Guid patientId, CreatePatientVisitRequest request) => new()
	{
		PatientVisitId = Guid.NewGuid(), PatientId = patientId, VisitPriority = request.VisitPriority,
		BloodPressure = request.BloodPressure, SugarLevel = request.SugarLevel, Weight = request.Weight,
		Height = request.Height, Temperature = request.Temperature, PulseRate = request.PulseRate,
		SpO2 = request.SpO2, RespiratoryRate = request.RespiratoryRate, CreatedOn = DateTime.UtcNow
	};

	private static PatientVisit CreateVisit(Guid patientId, RegisterPatientRequest request) => new()
	{
		PatientVisitId = Guid.NewGuid(), PatientId = patientId, VisitPriority = request.VisitPriority,
		BloodPressure = request.BloodPressure, SugarLevel = request.SugarLevel, Weight = request.Weight,
		Height = request.Height, Temperature = request.Temperature, PulseRate = request.PulseRate,
		SpO2 = request.SpO2, RespiratoryRate = request.RespiratoryRate, CreatedOn = DateTime.UtcNow
	};

	private static PatientResponse MapPatient(Patient patient) => new()
	{
		PatientId = patient.PatientId, ClinicId = patient.ClinicId, PatientToken = patient.PatientToken,
		FullName = patient.FullName, Age = patient.Age, Gender = patient.Gender,
		MobileNumber = patient.MobileNumber, Address = patient.Address, Allergies = patient.Allergies,
		ExistingDiseases = patient.ExistingDiseases, CurrentMedications = patient.CurrentMedications,
		PastSurgeries = patient.PastSurgeries, AncProfile = patient.AncProfile, LmpDate = patient.LmpDate,
		GestationalAge = patient.GestationalAge, ExpectedDeliveryDate = patient.ExpectedDeliveryDate,
		Trimester = patient.Trimester
	};

	private static PatientVisitResponse MapVisit(PatientVisit visit) => new()
	{
		PatientVisitId = visit.PatientVisitId, VisitPriority = visit.VisitPriority,
		BloodPressure = visit.BloodPressure, SugarLevel = visit.SugarLevel, Weight = visit.Weight,
		Height = visit.Height, Temperature = visit.Temperature, PulseRate = visit.PulseRate,
		SpO2 = visit.SpO2, RespiratoryRate = visit.RespiratoryRate, CreatedOn = visit.CreatedOn
	};
}