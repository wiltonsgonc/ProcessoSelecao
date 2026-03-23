using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ProcessoSelecao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AdminController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("reset-identities")]
    public async Task<ActionResult> ResetIdentities()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            return BadRequest("Connection string not found");

        var tables = new[] { "ProcessosSelecao", "Candidatos", "Avaliadores", "Baremas", "Documentos" };
        var results = new Dictionary<string, object>();

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            foreach (var table in tables)
            {
                try
                {
                    using var command = new SqlCommand($"DELETE FROM {table}", connection);
                    await command.ExecuteNonQueryAsync();

                    using var reseedCommand = new SqlCommand($"DBCC CHECKIDENT ('{table}', RESEED, 0)", connection);
                    await reseedCommand.ExecuteNonQueryAsync();

                    results[table] = "Identity reset successfully";
                }
                catch (Exception ex)
                {
                    results[table] = $"Error: {ex.Message}";
                }
            }

            return Ok(new { message = "All identities have been reset", details = results });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
