using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RbacSystem.Services;

namespace RbacSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly ILogger<ExportController> _logger;

    public ExportController(IExportService exportService, ILogger<ExportController> logger)
    {
        _exportService = exportService;
        _logger = logger;
    }

    /// <summary>
    /// Export all RBAC data as JSON file
    /// </summary>
    /// <remarks>
    /// Downloads a complete JSON file containing all users, groups, roles, permissions, and their relationships.
    /// Perfect for sharing with clients or creating backups.
    /// </remarks>
    /// <response code="200">Returns JSON file with all data</response>
    [HttpGet("json")]
    [ProducesResponseType(typeof(FileResult), 200)]
    public async Task<IActionResult> ExportJson()
    {
        try
        {
            _logger.LogInformation("JSON export requested");
            var data = await _exportService.ExportToJsonAsync();
            var fileName = $"rbac-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";

            return File(data, "application/json", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data to JSON");
            return StatusCode(500, new { error = "Failed to export data to JSON" });
        }
    }

    /// <summary>
    /// Export all RBAC data as CSV file
    /// </summary>
    /// <remarks>
    /// Downloads a CSV file containing all users, groups, roles, permissions, and their relationships.
    /// Each section is clearly separated with headers. Easy to open in Excel or Google Sheets.
    /// </remarks>
    /// <response code="200">Returns CSV file with all data</response>
    [HttpGet("csv")]
    [ProducesResponseType(typeof(FileResult), 200)]
    public async Task<IActionResult> ExportCsv()
    {
        try
        {
            _logger.LogInformation("Excel export requested");
            var data = await _exportService.ExportToCsvAsync();
            var fileName = $"rbac-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";

            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data to Excel");
            return StatusCode(500, new { error = "Failed to export data to Excel" });
        }
    }

    /// <summary>
    /// Export all RBAC data as formatted HTML file
    /// </summary>
    /// <remarks>
    /// Downloads a beautifully formatted HTML file with all data in tables.
    /// Perfect for sharing with clients who want a readable, visual report.
    /// Can be opened in any web browser or converted to PDF.
    /// </remarks>
    /// <response code="200">Returns HTML file with formatted data</response>
    [HttpGet("html")]
    [ProducesResponseType(typeof(FileResult), 200)]
    public async Task<IActionResult> ExportHtml()
    {
        try
        {
            _logger.LogInformation("HTML export requested");
            var html = await _exportService.ExportToHtmlAsync();
            var data = System.Text.Encoding.UTF8.GetBytes(html);
            var fileName = $"rbac-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.html";

            return File(data, "text/html", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data to HTML");
            return StatusCode(500, new { error = "Failed to export data to HTML" });
        }
    }

    /// <summary>
    /// View export data directly in browser (HTML)
    /// </summary>
    /// <remarks>
    /// Displays all RBAC data in a nicely formatted HTML page directly in the browser.
    /// Great for quick review before downloading.
    /// </remarks>
    /// <response code="200">Returns HTML content for browser display</response>
    [HttpGet("preview")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewData()
    {
        try
        {
            _logger.LogInformation("Data preview requested");
            var html = await _exportService.ExportToHtmlAsync();

            return Content(html, "text/html");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating data preview");
            return StatusCode(500, new { error = "Failed to generate preview" });
        }
    }

    /// <summary>
    /// Get export information and available formats
    /// </summary>
    /// <remarks>
    /// Returns metadata about available export formats and what data is included.
    /// </remarks>
    [HttpGet("info")]
    public IActionResult GetExportInfo()
    {
        var info = new
        {
            availableFormats = new[]
            {
                new
                {
                    format = "JSON",
                    endpoint = "/api/export/json",
                    mimeType = "application/json",
                    description = "Machine-readable format with complete data structure",
                    bestFor = "API integration, backup, data migration"
                },
                new
                {
                    format = "CSV",
                    endpoint = "/api/export/csv",
                    mimeType = "text/csv",
                    description = "Comma-separated values with all tables",
                    bestFor = "Excel, Google Sheets, data analysis"
                },
                new
                {
                    format = "HTML",
                    endpoint = "/api/export/html",
                    mimeType = "text/html",
                    description = "Formatted HTML report with styled tables",
                    bestFor = "Client sharing, visual reports, PDF conversion"
                }
            },
            includedData = new[]
            {
                "Users (with status and timestamps)",
                "Groups (departments, teams)",
                "Roles (with descriptions)",
                "Permissions (system capabilities)",
                "User-Group assignments",
                "User-Role assignments (direct)",
                "Group-Role assignments (inherited)",
                "Role-Permission mappings"
            },
            features = new[]
            {
                "Complete data snapshot",
                "Timestamped exports",
                "Includes all relationships",
                "Ready to share with clients",
                "No sensitive data (passwords not included)",
                "Clean, professional formatting"
            }
        };

        return Ok(info);
    }
}