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
		Task<Guid> CreateAsync(CreateDoctorRequest request);

		Task<IEnumerable<DoctorResponse>> GetAllAsync();

		Task<DoctorResponse?> GetByIdAsync(Guid doctorId);

		Task<bool> UpdateAsync(Guid doctorId, UpdateDoctorRequest request);

		Task<bool> DeleteAsync(Guid doctorId);
	}
}
