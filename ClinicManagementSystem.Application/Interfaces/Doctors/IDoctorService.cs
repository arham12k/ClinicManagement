using ClinicManagementSystem.Application.DTOs.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.Interfaces.Doctors
{
	public interface IDoctorService
	{
		Task<Guid> CreateAsync(Guid clinicId, CreateDoctorRequest request);

		Task<IEnumerable<DoctorResponse>> GetAllAsync(Guid clinicId);

		Task<DoctorResponse?> GetByIdAsync(Guid doctorId, Guid clinicId);

		Task<bool> UpdateAsync(Guid doctorId, Guid clinicId, UpdateDoctorRequest request);

		Task<bool> DeleteAsync(Guid doctorId, Guid clinicId);
	}
}
