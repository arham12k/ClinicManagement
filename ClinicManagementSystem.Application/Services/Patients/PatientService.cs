using ClinicManagementSystem.Application.DTOs.Patient;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Application.Interfaces.Doctors;
using ClinicManagementSystem.Application.Interfaces.Patients;
using ClinicManagementSystem.Domain.Entities.Patient;

namespace ClinicManagementSystem.Application.Services.Patients;

public class PatientService : IPatientService
{
	private readonly IPatientRepository _patientRepository;
	private readonly IClinicRepository _clinicRepository;
	private readonly IDoctorRepository _doctorRepository;

	public PatientService(
		IPatientRepository patientRepository,
		IClinicRepository clinicRepository,
		IDoctorRepository doctorRepository)
	{
		_patientRepository = patientRepository;
		_clinicRepository = clinicRepository;
		_doctorRepository = doctorRepository;
	}

	public async Task<(Guid PatientId, string PatientToken)> RegisterAsync(Guid clinicId, Guid? doctorId, RegisterPatientRequest request)
	{
		if (clinicId == Guid.Empty)
			throw new ArgumentException("ClinicId is required.");

		var clinic = await _clinicRepository.GetByIdAsync(clinicId);

		if (clinic == null)
			throw new Exception("Clinic not found.");

		await ValidateDoctorAsync(clinicId, doctorId);

		if (await _patientRepository.GetByNameAndMobileNumberAsync(clinicId, request.FullName, request.MobileNumber) != null)
			throw new InvalidOperationException("Patient is already registered with this name and mobile number.");

		request.VisitPriority = NormalizeVisitPriority(request.VisitPriority);

		var patient = new Patient
		{
			PatientId = Guid.NewGuid(),
			ClinicId = clinicId,
			DoctorId = doctorId,
			FullName = request.FullName,
			Age = request.Age,
			Gender = request.Gender,
			MobileNumber = request.MobileNumber,
			Address = request.Address ?? string.Empty,
			Allergies = request.Allergies ?? string.Empty,
			ExistingDiseases = request.ExistingDiseases ?? string.Empty,
			CurrentMedications = request.CurrentMedications ?? string.Empty,
			PastSurgeries = request.PastSurgeries ?? string.Empty,
			AncProfile = request.AncProfile ?? string.Empty,
			LmpDate = request.LmpDate,
			GestationalAge = request.GestationalAge ?? string.Empty,
			ExpectedDeliveryDate = request.ExpectedDeliveryDate,
			Trimester = request.Trimester ?? string.Empty,
			IsActive = true,
			CreatedOn = DateTime.UtcNow
		};

		var firstVisit = CreateVisit(patient.PatientId, doctorId, request);
		await _patientRepository.CreateAsync(patient, firstVisit);

		return (patient.PatientId, patient.PatientToken);
	}

	public async Task<IEnumerable<PatientResponse>> GetAllAsync(Guid clinicId, Guid? doctorId)
	{
		if (clinicId == Guid.Empty)
			throw new ArgumentException("ClinicId is required.");

		await ValidateDoctorAsync(clinicId, doctorId);

		var patients = await _patientRepository.GetAllAsync(clinicId, doctorId);
		var response = new List<PatientResponse>();

		foreach (var patient in patients)
		{
			var visits = (await _patientRepository.GetVisitsAsync(patient.PatientId)).ToList();
			response.Add(MapPatient(patient, visits));
		}

		return response;
	}

	public async Task<PatientResponse?> GetByIdAsync(Guid patientId)
	{
		var patient = await _patientRepository.GetByIdAsync(patientId);

		if (patient == null)
			return null;

		var visits = (await _patientRepository.GetVisitsAsync(patientId)).ToList();
		var response = MapPatient(patient, visits);
		response.Visits = visits.Select(MapVisit).ToList();
		return response;
	}

	public async Task<IEnumerable<PatientVisitResponse>?> GetVisitsAsync(Guid patientId)
	{
		var patient = await _patientRepository.GetByIdAsync(patientId);

		return patient == null
			? null
			: (await _patientRepository.GetVisitsAsync(patientId)).Select(MapVisit);
	}

	public async Task<Guid?> CreateVisitAsync(Guid patientId, Guid? doctorId, CreatePatientVisitRequest request)
	{
		var patient = await _patientRepository.GetByIdAsync(patientId);

		if (patient == null)
			return null;

		await ValidateDoctorAsync(patient.ClinicId, doctorId);

		var visit = CreateVisit(patientId, doctorId, request);
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

	private static PatientVisit CreateVisit(Guid patientId, Guid? doctorId, CreatePatientVisitRequest request) => new()
	{
		PatientVisitId = Guid.NewGuid(), PatientId = patientId, DoctorId = doctorId, VisitPriority = request.VisitPriority,
		BloodPressure = request.BloodPressure, SugarLevel = request.SugarLevel, Weight = request.Weight,
		Height = request.Height, Temperature = request.Temperature, PulseRate = request.PulseRate,
		SpO2 = request.SpO2, RespiratoryRate = request.RespiratoryRate, CreatedOn = DateTime.UtcNow
	};

	private static PatientVisit CreateVisit(Guid patientId, Guid? doctorId, RegisterPatientRequest request) => new()
	{
		PatientVisitId = Guid.NewGuid(), PatientId = patientId, DoctorId = doctorId, VisitPriority = request.VisitPriority,
		BloodPressure = request.BloodPressure ?? string.Empty, SugarLevel = request.SugarLevel ?? string.Empty, Weight = request.Weight ?? string.Empty,
		Height = request.Height ?? string.Empty, Temperature = request.Temperature ?? string.Empty, PulseRate = request.PulseRate ?? string.Empty,
		SpO2 = request.SpO2 ?? string.Empty, RespiratoryRate = request.RespiratoryRate ?? string.Empty, CreatedOn = DateTime.UtcNow
	};

	private static PatientResponse MapPatient(Patient patient, IReadOnlyCollection<PatientVisit>? visits = null) => new()
	{
		PatientId = patient.PatientId, ClinicId = patient.ClinicId, DoctorId = patient.DoctorId, PatientToken = patient.PatientToken,
		FullName = patient.FullName, Age = patient.Age, Gender = patient.Gender,
		MobileNumber = patient.MobileNumber, Address = patient.Address, Allergies = patient.Allergies,
		ExistingDiseases = patient.ExistingDiseases, CurrentMedications = patient.CurrentMedications,
		PastSurgeries = patient.PastSurgeries, AncProfile = patient.AncProfile, LmpDate = patient.LmpDate,
		GestationalAge = patient.GestationalAge, ExpectedDeliveryDate = patient.ExpectedDeliveryDate,
		Trimester = patient.Trimester, LastVisitDate = visits?.MaxBy(visit => visit.CreatedOn)?.CreatedOn,
		TotalVisits = visits?.Count ?? 0
	};

	private static string NormalizeVisitPriority(string visitPriority) => visitPriority.Trim().ToLowerInvariant() switch
	{
		"normal" => "Normal",
		"emergency" => "Emergency",
		_ => throw new ArgumentException("Visit type must be either Normal or Emergency.")
	};

	private static PatientVisitResponse MapVisit(PatientVisit visit) => new()
	{
		PatientVisitId = visit.PatientVisitId, DoctorId = visit.DoctorId, VisitPriority = visit.VisitPriority,
		BloodPressure = visit.BloodPressure, SugarLevel = visit.SugarLevel, Weight = visit.Weight,
		Height = visit.Height, Temperature = visit.Temperature, PulseRate = visit.PulseRate,
		SpO2 = visit.SpO2, RespiratoryRate = visit.RespiratoryRate, CreatedOn = visit.CreatedOn
	};

	private async Task ValidateDoctorAsync(Guid clinicId, Guid? doctorId)
	{
		if (doctorId.HasValue && await _doctorRepository.GetByIdAsync(doctorId.Value, clinicId) == null)
			throw new ArgumentException("Doctor not found for this clinic.");
	}
}