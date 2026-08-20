using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Domain.Common
{
	public abstract class AuditableEntity
	{
		public DateTime CreatedOn { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
