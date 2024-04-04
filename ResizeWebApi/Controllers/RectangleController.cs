using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;

namespace ResizeWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RectangleController : ControllerBase
    {
        private readonly ILogger<RectangleController> _logger;
        private readonly string _jsonFileName = "dimensions_file.json";
        private readonly string _jsonFilePath;
        private readonly IWebHostEnvironment _env;

        public RectangleController(ILogger<RectangleController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            _jsonFilePath = Path.Combine(_env.ContentRootPath, "Data", _jsonFileName);
        }

        [HttpGet]
        public IActionResult GetDimensions()
        {
            try
            {
                string jsonString = System.IO.File.ReadAllText(_jsonFilePath);
                RectangleDimensions dimensions = JsonSerializer.Deserialize<RectangleDimensions>(jsonString);
                return Ok(dimensions);
            }
            catch (FileNotFoundException)
            {
                return NotFound("JSON file not found");
            }
            catch (JsonException)
            {
                return BadRequest("Invalid JSON format");
            }
        }

        [HttpPut]
        public IActionResult UpdateDimensions(RectangleDimensions dimensions)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(dimensions);
                System.IO.File.WriteAllText(_jsonFilePath, jsonString);
                return NoContent();
            }
            catch (JsonException)
            {
                return BadRequest("Error serializing JSON");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dimensions");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
