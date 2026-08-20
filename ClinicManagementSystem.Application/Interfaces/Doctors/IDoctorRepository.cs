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

		Task<Doctor?> GetByIdAsync(Guid doctorId);

		Task<IEnumerable<Doctor>> GetAllAsync();

		Task<Doctor?> GetByMedicalRegistrationNumberAsync(string registrationNumber);

		Task<bool> UpdateAsync(Doctor doctor, List<DoctorAvailability> availability);

		Task<bool> DeleteAsync(Guid doctorId);

		Task<IEnumerable<DoctorAvailability>> GetAvailabilityAsync(Guid doctorId);
	}
}
