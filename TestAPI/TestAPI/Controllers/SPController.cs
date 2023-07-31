using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;
using TestAPI.Model;
using System.Data;
using Microsoft.Data.SqlClient;

namespace TestAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class SPController : ControllerBase
	{
		
		//private readonly string connectionString = "Server=ARSH\\SQLEXPRESS;user id=testuser;password=12345;Database=DISCUSSION_FORUM;Integrated Security = true"; // Replace with your actual connection string

		public IConfiguration configuration;
		public string conStr { get; set; }
		public SPController(IConfiguration _configuration)
		{
			configuration = _configuration;
			conStr = configuration.GetConnectionString("SPConnection");
		}
	
		[HttpGet]
		public IActionResult GetStoredProceduresBetweenDates(DateTime startDate, DateTime endDate)
		{
			List<StoredProcedureInfo> storedProcedures = new List<StoredProcedureInfo>();

			using (SqlConnection connection = new SqlConnection(conStr))
			{
				connection.Open();

				// Query to retrieve stored procedures and their creation dates.
				string query = @"
                    SELECT name, create_date
                    FROM sys.objects
                    WHERE type = 'P' AND Convert(Nvarchar,modify_date,23) >= Convert(Nvarchar,@StartDate,23) 
					AND Convert(Nvarchar,modify_date,23) <= Convert(Nvarchar,@EndDate,23);
                ";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
					command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string spName = reader.GetString(0);
							DateTime createdDate = reader.GetDateTime(1);
							string spText = GetStoredProcedureText(spName);

							storedProcedures.Add(new StoredProcedureInfo
							{
								Name = spName,
								CreatedDate = createdDate,
								Text = spText
							});
						}
					}
				}
			}

			return Ok(storedProcedures);
		}

		// Helper method to retrieve the text of a specific stored procedure.
		private string GetStoredProcedureText(string spName)
		{
			using (SqlConnection connection = new SqlConnection(conStr))
			{
				connection.Open();

				string query = @"
                    SELECT [definition]
                    FROM sys.sql_modules m
                    INNER JOIN sys.objects o ON m.object_id = o.object_id
                    WHERE o.type = 'P' AND o.name = @SpName;
                ";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.Add("@SpName", SqlDbType.NVarChar, 200).Value = spName;

					object spText = command.ExecuteScalar();

					return spText != null ? spText.ToString() : string.Empty;
				}
			}
		}


		}
}
