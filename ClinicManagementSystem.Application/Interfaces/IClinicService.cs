using ClinicManagementSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.Interfaces
{
	public interface IClinicService
	{
		Task<Guid> CreateAsync(CreateClinicRequest request);

		Task<ClinicResponse?> GetByIdAsync(Guid clinicId);

		Task<IEnumerable<ClinicResponse>> GetAllAsync();

		Task<bool> UpdateAsync(Guid clinicId, CreateClinicRequest request);

		Task<bool> DeleteAsync(Guid clinicId);
	}
}
