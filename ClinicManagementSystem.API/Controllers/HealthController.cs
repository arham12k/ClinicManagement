using ClinicManagementSystem.Infrastructure.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
	private readonly DapperContext _context;

	public HealthController(DapperContext context)
	{
		_context = context;
	}

	[HttpGet("database")]
	public async Task<IActionResult> Database()
	{
		using var connection = _context.CreateConnection();

		var result = await connection.ExecuteScalarAsync<int>("SELECT 1");

		return Ok(new
		{
			Message = "Database Connected Successfully",
			Result = result
		});
	}
}