using ClinicManagementSystem.Application.DTOs.Patient;

namespace ClinicManagementSystem.Application.Interfaces.Patients;

public interface IPatientService
{
	Task<(Guid PatientId, string PatientToken)> RegisterAsync(RegisterPatientRequest request);
	Task<IEnumerable<PatientResponse>> GetAllAsync();
	Task<PatientResponse?> GetByIdAsync(Guid patientId);
	Task<IEnumerable<PatientVisitResponse>?> GetVisitsAsync(Guid patientId);
	Task<Guid?> CreateVisitAsync(Guid patientId, CreatePatientVisitRequest request);
	Task<bool> UpdateAsync(Guid patientId, UpdatePatientRequest request);
	Task<bool> DeleteAsync(Guid patientId);
}