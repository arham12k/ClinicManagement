using ClinicManagementSystem.Application.Interfaces.Doctors;
using ClinicManagementSystem.Domain.Entities.Doctor;
using ClinicManagementSystem.Infrastructure.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Infrastructure.Repositories.Doctors
{

	public class DoctorRepository : IDoctorRepository
	{
		private readonly DapperContext _context;

		public DoctorRepository(DapperContext context)
		{
			_context = context;
		}

		public async Task<Guid> CreateAsync(
			Doctor doctor,
			List<DoctorAvailability> availability)
		{
			using var connection = _context.CreateConnection();

			connection.Open();

			using var transaction = connection.BeginTransaction();

			try
			{
				const string doctorSql = @"
INSERT INTO Doctors
(
    Doctor_Id,
    Clinic_Id,
    Full_Name,
    Date_Of_Birth,
    Gender,
    Mobile_Number,
    Address,
    Medical_Registration_Number,
    Registration_State,
    Specialization,
    Sub_Specialization,
    Experience_Years,
    Qualification,
    Consultation_Fees,
    Is_Active,
    Created_On
)
VALUES
(
    @DoctorId,
    @ClinicId,
    @FullName,
    @DateOfBirth,
    @Gender,
    @MobileNumber,
    @Address,
    @MedicalRegistrationNumber,
    @RegistrationState,
    @Specialization,
    @SubSpecialization,
    @ExperienceYears,
    @Qualification,
    @ConsultationFees,
    @IsActive,
    @CreatedOn
);";

				await connection.ExecuteAsync(
					doctorSql,
					doctor,
					transaction);

				const string availabilitySql = @"
INSERT INTO Doctor_Availability
(
    Availability_Id,
    Doctor_Id,
    Session_Type,
    Day_Of_Week,
    From_Time,
    To_Time
)
VALUES
(
    @AvailabilityId,
    @DoctorId,
    @SessionType,
    @DayOfWeek,
    @FromTime,
    @ToTime
);";

				foreach (var item in availability)
				{
					await connection.ExecuteAsync(
						availabilitySql,
						item,
						transaction);
				}

				transaction.Commit();

				return doctor.DoctorId;
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}

		public async Task<IEnumerable<Doctor>> GetAllAsync()
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
SELECT *
FROM Doctors
WHERE Is_Active = TRUE
ORDER BY Full_Name;";

			return await connection.QueryAsync<Doctor>(sql);
		}

		public async Task<Doctor?> GetByIdAsync(Guid doctorId)
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
SELECT *
FROM Doctors
WHERE Doctor_Id = @DoctorId;";

			return await connection.QueryFirstOrDefaultAsync<Doctor>(
				sql,
				new { DoctorId = doctorId });
		}

		public async Task<Doctor?> GetByMedicalRegistrationNumberAsync(
			string registrationNumber)
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
SELECT *
FROM Doctors
WHERE Medical_Registration_Number=@RegistrationNumber
LIMIT 1;";

			return await connection.QueryFirstOrDefaultAsync<Doctor>(
				sql,
				new
				{
					RegistrationNumber = registrationNumber
				});
		}

		public async Task<bool> UpdateAsync(
			Doctor doctor,
			List<DoctorAvailability> availability)
		{
			using var connection = _context.CreateConnection();

			connection.Open();

			using var transaction = connection.BeginTransaction();

			try
			{
				const string updateDoctorSql = @"
UPDATE Doctors
SET
    Full_Name=@FullName,
    Date_Of_Birth=@DateOfBirth,
    Gender=@Gender,
    Mobile_Number=@MobileNumber,
    Address=@Address,
    Registration_State=@RegistrationState,
    Specialization=@Specialization,
    Sub_Specialization=@SubSpecialization,
    Experience_Years=@ExperienceYears,
    Qualification=@Qualification,
    Consultation_Fees=@ConsultationFees,
    Updated_On=@UpdatedOn
WHERE Doctor_Id=@DoctorId;";

				var affectedRows = await connection.ExecuteAsync(
					updateDoctorSql,
					doctor,
					transaction);

				if (affectedRows == 0)
				{
					transaction.Rollback();
					return false;
				}

				const string deleteAvailabilitySql = @"
DELETE FROM Doctor_Availability
WHERE Doctor_Id=@DoctorId;";

				await connection.ExecuteAsync(
					deleteAvailabilitySql,
					new
					{
						doctor.DoctorId
					},
					transaction);

				const string insertAvailabilitySql = @"
INSERT INTO Doctor_Availability
(
    Availability_Id,
    Doctor_Id,
    Session_Type,
    Day_Of_Week,
    From_Time,
    To_Time
)
VALUES
(
    @AvailabilityId,
    @DoctorId,
    @SessionType,
    @DayOfWeek,
    @FromTime,
    @ToTime
);";

				foreach (var item in availability)
				{
					await connection.ExecuteAsync(
						insertAvailabilitySql,
						item,
						transaction);
				}

				transaction.Commit();

				return true;
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}


		public async Task<IEnumerable<DoctorAvailability>> GetAvailabilityAsync(Guid doctorId)
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
        SELECT *
        FROM Doctor_Availability
        WHERE Doctor_Id = @DoctorId
        ORDER BY Session_Type, Day_Of_Week;";

			return await connection.QueryAsync<DoctorAvailability>(
				sql,
				new { DoctorId = doctorId });
		}
	}
}
