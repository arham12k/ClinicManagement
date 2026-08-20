using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using Dapper;

namespace ClinicManagementSystem.Infrastructure.Data;

public class DapperContext
{
	private readonly IConfiguration _configuration;

	public DapperContext(IConfiguration configuration)
	{
		_configuration = configuration;
		DefaultTypeMap.MatchNamesWithUnderscores = true;
		SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
	}

	public IDbConnection CreateConnection()
	{
		return new NpgsqlConnection(
			_configuration.GetConnectionString("DefaultConnection"));
	}

	private sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
	{
		public override DateOnly Parse(object value) => value switch
		{
			DateOnly date => date,
			DateTime dateTime => DateOnly.FromDateTime(dateTime),
			_ => DateOnly.Parse(value.ToString()!)
		};

		public override void SetValue(IDbDataParameter parameter, DateOnly value)
		{
			parameter.DbType = DbType.Date;
			parameter.Value = value.ToDateTime(TimeOnly.MinValue);
		}
	}
}