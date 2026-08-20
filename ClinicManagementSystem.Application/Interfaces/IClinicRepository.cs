using ClinicManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Application.Interfaces
{
	public interface IClinicRepository
	{
		Task<Guid> CreateAsync(Clinic clinic);

		Task<Clinic?> GetByIdAsync(Guid clinicId);

		Task<Clinic?> GetByEmailAsync(string email);

		Task<IEnumerable<Clinic>> GetAllAsync();

		Task<bool> UpdateAsync(Clinic clinic);

		Task<bool> DeleteAsync(Guid clinicId);
	}
}
