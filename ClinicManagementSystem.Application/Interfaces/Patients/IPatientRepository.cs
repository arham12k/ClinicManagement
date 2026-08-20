using ClinicManagementSystem.Domain.Entities.Patient;

namespace ClinicManagementSystem.Application.Interfaces.Patients;

public interface IPatientRepository
{
	Task<Guid> CreateAsync(Patient patient, PatientVisit firstVisit);
	Task<Patient?> GetByIdAsync(Guid patientId);
	Task<IEnumerable<Patient>> GetAllAsync();
	Task<IEnumerable<PatientVisit>> GetVisitsAsync(Guid patientId);
	Task<Guid> CreateVisitAsync(PatientVisit visit);
	Task<bool> UpdateAsync(Patient patient);
	Task<bool> DeleteAsync(Guid patientId);
}