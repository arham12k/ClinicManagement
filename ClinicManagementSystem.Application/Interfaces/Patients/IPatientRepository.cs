using ClinicManagementSystem.Domain.Entities.Patient;

namespace ClinicManagementSystem.Application.Interfaces.Patients;

public interface IPatientRepository
{
	Task<Guid> CreateAsync(Patient patient, PatientVisit firstVisit);
	Task<Patient?> GetByNameAndMobileNumberAsync(Guid clinicId, string fullName, string mobileNumber);
	Task<Patient?> GetByIdAsync(Guid patientId);
	Task<IEnumerable<Patient>> GetAllAsync(Guid clinicId, Guid? doctorId);
	Task<IEnumerable<PatientVisit>> GetVisitsAsync(Guid patientId);
	Task<Guid> CreateVisitAsync(PatientVisit visit);
	Task<bool> UpdateAsync(Patient patient);
	Task<bool> DeleteAsync(Guid patientId);
}