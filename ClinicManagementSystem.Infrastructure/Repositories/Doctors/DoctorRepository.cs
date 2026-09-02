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
    Experience,
    Qualification,
    Consultation_Fees,
    Available_Days,
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
    @Experience,
    @Qualification,
    @ConsultationFees,
    @AvailableDays,
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

		public async Task<IEnumerable<Doctor>> GetAllAsync(Guid clinicId)
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
SELECT
    Doctor_Id AS DoctorId,
    Clinic_Id AS ClinicId,
    Full_Name AS FullName,
    Date_Of_Birth AS DateOfBirth,
    Gender,
    Mobile_Number AS MobileNumber,
    Address,
    Medical_Registration_Number AS MedicalRegistrationNumber,
    Registration_State AS RegistrationState,
    Specialization,
    Sub_Specialization AS SubSpecialization,
    Experience,
    Qualification,
    Consultation_Fees AS ConsultationFees,
    Available_Days AS AvailableDays,
    Is_Active AS IsActive,
    Created_On AS CreatedOn,
    Updated_On AS UpdatedOn
FROM Doctors
WHERE Is_Active = TRUE AND Clinic_Id = @ClinicId
ORDER BY Full_Name;";

			return await connection.QueryAsync<Doctor>(sql, new { ClinicId = clinicId });
		}

		public async Task<Doctor?> GetByIdAsync(Guid doctorId, Guid clinicId)
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
SELECT
    Doctor_Id AS DoctorId,
    Clinic_Id AS ClinicId,
    Full_Name AS FullName,
    Date_Of_Birth AS DateOfBirth,
    Gender,
    Mobile_Number AS MobileNumber,
    Address,
    Medical_Registration_Number AS MedicalRegistrationNumber,
    Registration_State AS RegistrationState,
    Specialization,
    Sub_Specialization AS SubSpecialization,
    Experience,
    Qualification,
    Consultation_Fees AS ConsultationFees,
    Available_Days AS AvailableDays,
    Is_Active AS IsActive,
    Created_On AS CreatedOn,
    Updated_On AS UpdatedOn
FROM Doctors
WHERE Doctor_Id = @DoctorId AND Clinic_Id = @ClinicId AND Is_Active = TRUE;";

			return await connection.QueryFirstOrDefaultAsync<Doctor>(
				sql,
				new { DoctorId = doctorId, ClinicId = clinicId });
		}

		public async Task<Doctor?> GetByMedicalRegistrationNumberAsync(
			string registrationNumber)
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
SELECT
    Doctor_Id AS DoctorId,
    Clinic_Id AS ClinicId,
    Full_Name AS FullName,
    Date_Of_Birth AS DateOfBirth,
    Gender,
    Mobile_Number AS MobileNumber,
    Address,
    Medical_Registration_Number AS MedicalRegistrationNumber,
    Registration_State AS RegistrationState,
    Specialization,
    Sub_Specialization AS SubSpecialization,
    Experience,
    Qualification,
    Consultation_Fees AS ConsultationFees,
    Available_Days AS AvailableDays,
    Is_Active AS IsActive,
    Created_On AS CreatedOn,
    Updated_On AS UpdatedOn
FROM Doctors
WHERE Medical_Registration_Number=@RegistrationNumber AND Is_Active = TRUE
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
			List<DoctorAvailability> availability,
			Guid clinicId)
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
    Experience=@Experience,
    Qualification=@Qualification,
    Consultation_Fees=@ConsultationFees,
    Available_Days=@AvailableDays,
    Updated_On=@UpdatedOn
WHERE Doctor_Id=@DoctorId AND Clinic_Id=@FilterClinicId;";

				var parameters = new Dapper.DynamicParameters(doctor);
				parameters.Add("FilterClinicId", clinicId);

				var affectedRows = await connection.ExecuteAsync(
					updateDoctorSql,
					parameters,
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

		public async Task<bool> DeleteAsync(Guid doctorId, Guid clinicId)
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
UPDATE Doctors
SET
    Is_Active = FALSE,
    Updated_On = @UpdatedOn
WHERE Doctor_Id = @DoctorId AND Clinic_Id = @ClinicId AND Is_Active = TRUE;";

			return await connection.ExecuteAsync(
				sql,
				new { DoctorId = doctorId, ClinicId = clinicId, UpdatedOn = DateTime.UtcNow }) > 0;
		}


		public async Task<IEnumerable<DoctorAvailability>> GetAvailabilityAsync(Guid doctorId)
		{
			using var connection = _context.CreateConnection();

			const string sql = @"
SELECT
    Availability_Id AS AvailabilityId,
    Doctor_Id AS DoctorId,
    Session_Type AS SessionType,
    Day_Of_Week AS DayOfWeek,
    From_Time AS FromTime,
    To_Time AS ToTime
FROM Doctor_Availability
WHERE Doctor_Id = @DoctorId
ORDER BY Session_Type, Day_Of_Week;";

			return await connection.QueryAsync<DoctorAvailability>(
				sql,
				new { DoctorId = doctorId });
		}
	}
}
