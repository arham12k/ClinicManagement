using ClinicManagementSystem.Application.Interfaces.Patients;
using ClinicManagementSystem.Domain.Entities.Patient;
using ClinicManagementSystem.Infrastructure.Data;
using Dapper;

namespace ClinicManagementSystem.Infrastructure.Repositories.Patients;

public class PatientRepository : IPatientRepository
{
	private readonly DapperContext _context;

	public PatientRepository(DapperContext context)
	{
		_context = context;
	}

	public async Task<Guid> CreateAsync(Patient patient, PatientVisit firstVisit)
	{
		using var connection = _context.CreateConnection();
		connection.Open();
		using var transaction = connection.BeginTransaction();

		try
		{
			patient.PatientToken = await GeneratePatientTokenAsync(connection, transaction, firstVisit.VisitPriority);

			const string patientSql = @"
INSERT INTO Patients
(
	Patient_Id, Clinic_Id, Doctor_Id, Patient_Token, Full_Name, Age, Gender, Mobile_Number,
    Address, Allergies, Existing_Diseases, Current_Medications, Past_Surgeries,
    Anc_Profile, Lmp_Date, Gestational_Age, Expected_Delivery_Date, Trimester,
    Is_Active, Created_On
)
VALUES
(
	@PatientId, @ClinicId, @DoctorId, @PatientToken, @FullName, @Age, @Gender, @MobileNumber,
    @Address, @Allergies, @ExistingDiseases, @CurrentMedications, @PastSurgeries,
    @AncProfile, @LmpDate, @GestationalAge, @ExpectedDeliveryDate, @Trimester,
    @IsActive, @CreatedOn
);";

			await connection.ExecuteAsync(patientSql, patient, transaction);
			await InsertVisitAsync(connection, transaction, firstVisit);
			transaction.Commit();

			return patient.PatientId;
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	public async Task<IEnumerable<Patient>> GetAllAsync(Guid clinicId, Guid? doctorId)
	{
		using var connection = _context.CreateConnection();

		const string sql = @"
SELECT *
FROM Patients p
WHERE p.Clinic_Id = @ClinicId
	AND p.Is_Active = TRUE
	AND (@DoctorId IS NULL OR p.Doctor_Id = @DoctorId)
ORDER BY Created_On DESC;";

		return await connection.QueryAsync<Patient>(sql, new { ClinicId = clinicId, DoctorId = doctorId });
	}

	public async Task<Patient?> GetByIdAsync(Guid patientId)
	{
		using var connection = _context.CreateConnection();

		const string sql = @"
SELECT *
FROM Patients
WHERE Patient_Id = @PatientId AND Is_Active = TRUE;";

		return await connection.QueryFirstOrDefaultAsync<Patient>(sql, new { PatientId = patientId });
	}

	public async Task<Patient?> GetByNameAndMobileNumberAsync(Guid clinicId, string fullName, string mobileNumber)
	{
		using var connection = _context.CreateConnection();

		const string sql = @"
SELECT *
FROM Patients
WHERE Clinic_Id = @ClinicId
  AND Is_Active = TRUE
  AND LOWER(Full_Name) = LOWER(@FullName)
  AND Mobile_Number = @MobileNumber;";

		return await connection.QueryFirstOrDefaultAsync<Patient>(sql, new { ClinicId = clinicId, FullName = fullName.Trim(), MobileNumber = mobileNumber.Trim() });
	}

	public async Task<IEnumerable<PatientVisit>> GetVisitsAsync(Guid patientId)
	{
		using var connection = _context.CreateConnection();

		const string sql = @"
SELECT *
FROM Patient_Visits
WHERE Patient_Id = @PatientId
ORDER BY Created_On DESC;";

		return await connection.QueryAsync<PatientVisit>(sql, new { PatientId = patientId });
	}

	public async Task<Guid> CreateVisitAsync(PatientVisit visit)
	{
		using var connection = _context.CreateConnection();
		await InsertVisitAsync(connection, null, visit);

		return visit.PatientVisitId;
	}

	public async Task<bool> UpdateAsync(Patient patient)
	{
		using var connection = _context.CreateConnection();

		const string sql = @"
UPDATE Patients
SET
    Full_Name = @FullName,
    Age = @Age,
    Gender = @Gender,
    Mobile_Number = @MobileNumber,
    Address = @Address,
    Allergies = @Allergies,
    Existing_Diseases = @ExistingDiseases,
    Current_Medications = @CurrentMedications,
    Past_Surgeries = @PastSurgeries,
    Anc_Profile = @AncProfile,
    Lmp_Date = @LmpDate,
    Gestational_Age = @GestationalAge,
    Expected_Delivery_Date = @ExpectedDeliveryDate,
    Trimester = @Trimester,
    Updated_On = @UpdatedOn
WHERE Patient_Id = @PatientId AND Is_Active = TRUE;";

		return await connection.ExecuteAsync(sql, patient) > 0;
	}

	public async Task<bool> DeleteAsync(Guid patientId)
	{
		using var connection = _context.CreateConnection();

		const string sql = @"
UPDATE Patients
SET
    Is_Active = FALSE,
    Updated_On = @UpdatedOn
WHERE Patient_Id = @PatientId AND Is_Active = TRUE;";

		return await connection.ExecuteAsync(
			sql,
			new { PatientId = patientId, UpdatedOn = DateTime.UtcNow }) > 0;
	}

	private static async Task InsertVisitAsync(
		System.Data.IDbConnection connection,
		System.Data.IDbTransaction? transaction,
		PatientVisit visit)
	{
		const string sql = @"
INSERT INTO Patient_Visits
(
	Patient_Visit_Id, Patient_Id, Doctor_Id, Visit_Priority, Blood_Pressure, Sugar_Level,
    Weight, Height, Temperature, Pulse_Rate, Sp_O2, Respiratory_Rate, Created_On
)
VALUES
(
	@PatientVisitId, @PatientId, @DoctorId, @VisitPriority, @BloodPressure, @SugarLevel,
    @Weight, @Height, @Temperature, @PulseRate, @SpO2, @RespiratoryRate, @CreatedOn
);";

		await connection.ExecuteAsync(sql, visit, transaction);
	}

	private static async Task<string> GeneratePatientTokenAsync(
		System.Data.IDbConnection connection,
		System.Data.IDbTransaction transaction,
		string visitPriority)
	{
		var prefix = visitPriority == "Emergency" ? "E" : "T";
		await connection.ExecuteAsync("SELECT pg_advisory_xact_lock(hashtext(@Prefix));", new { Prefix = prefix }, transaction);

		var nextNumber = await connection.QuerySingleAsync<int>(@"
SELECT COALESCE(MAX(CAST(SUBSTRING(Patient_Token FROM 2) AS INTEGER)), 0) + 1
FROM Patients
WHERE Patient_Token LIKE @Pattern;", new { Pattern = $"{prefix}%" }, transaction);

		return $"{prefix}{nextNumber:D3}";
	}
}