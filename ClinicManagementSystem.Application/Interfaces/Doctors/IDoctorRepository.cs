using ClinicManagementSystem.Domain.Entities.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.Interfaces.Doctors
{
	public interface IDoctorRepository
	{
		Task<Guid> CreateAsync(Doctor doctor, List<DoctorAvailability> availability);

		Task<Doctor?> GetByIdAsync(Guid doctorId, Guid clinicId);

		Task<IEnumerable<Doctor>> GetAllAsync(Guid clinicId);

		Task<Doctor?> GetByMedicalRegistrationNumberAsync(string registrationNumber);

		Task<bool> UpdateAsync(Doctor doctor, List<DoctorAvailability> availability, Guid clinicId);

		Task<bool> DeleteAsync(Guid doctorId, Guid clinicId);

		Task<IEnumerable<DoctorAvailability>> GetAvailabilityAsync(Guid doctorId);
	}
}
