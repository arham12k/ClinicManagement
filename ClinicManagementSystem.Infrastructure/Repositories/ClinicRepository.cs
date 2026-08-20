using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Data;
using Dapper;

namespace ClinicManagementSystem.Infrastructure.Repositories;

public class ClinicRepository : IClinicRepository
{
	private readonly DapperContext _context;

	public ClinicRepository(DapperContext context)
	{
		_context = context;
	}

	public async Task<Guid> CreateAsync(Clinic clinic)
	{
		using var connection = _context.CreateConnection();

		var sql = @"
INSERT INTO Clinics
(
    Clinic_Id,
    Clinic_Name,
    Owner_Name,
    Mobile_Number,
    Email,
    Address,
    City,
    State,
    Pincode,
    Is_Active,
    Created_On
)
VALUES
(
    @ClinicId,
    @ClinicName,
    @OwnerName,
    @MobileNumber,
    @Email,
    @Address,
    @City,
    @State,
    @Pincode,
    @IsActive,
    @CreatedOn
);";

		await connection.ExecuteAsync(sql, clinic);

		return clinic.ClinicId;
	}

	public async Task<Clinic?> GetByEmailAsync(string email)
	{
		using var connection = _context.CreateConnection();

		var sql = @"SELECT * FROM Clinics
					WHERE Email=@Email AND Is_Active = TRUE
                    LIMIT 1;";

		return await connection.QueryFirstOrDefaultAsync<Clinic>(sql, new { Email = email });
	}

	public async Task<Clinic?> GetByIdAsync(Guid clinicId)
	{
		using var connection = _context.CreateConnection();

		var sql = @"SELECT * FROM Clinics
					WHERE Clinic_Id=@ClinicId AND Is_Active = TRUE;";

		return await connection.QueryFirstOrDefaultAsync<Clinic>(sql, new { ClinicId = clinicId });
	}

	public async Task<IEnumerable<Clinic>> GetAllAsync()
	{
		using var connection = _context.CreateConnection();

		var sql = @"SELECT * FROM Clinics
					WHERE Is_Active = TRUE
                    ORDER BY Created_On DESC;";

		return await connection.QueryAsync<Clinic>(sql);
	}

	public async Task<bool> UpdateAsync(Clinic clinic)
	{
		using var connection = _context.CreateConnection();

		const string sql = @"
UPDATE Clinics
SET
    Clinic_Name = @ClinicName,
    Owner_Name = @OwnerName,
    Mobile_Number = @MobileNumber,
    Email = @Email,
    Address = @Address,
    City = @City,
    State = @State,
    Pincode = @Pincode,
    Updated_On = @UpdatedOn
WHERE Clinic_Id = @ClinicId AND Is_Active = TRUE;";

		return await connection.ExecuteAsync(sql, clinic) > 0;
	}

	public async Task<bool> DeleteAsync(Guid clinicId)
	{
		using var connection = _context.CreateConnection();

		const string sql = @"
UPDATE Clinics
SET
    Is_Active = FALSE,
    Updated_On = @UpdatedOn
WHERE Clinic_Id = @ClinicId AND Is_Active = TRUE;";

		return await connection.ExecuteAsync(
			sql,
			new { ClinicId = clinicId, UpdatedOn = DateTime.UtcNow }) > 0;
	}
}