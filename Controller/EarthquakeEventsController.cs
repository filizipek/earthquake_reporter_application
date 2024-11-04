using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EarthquakeReporter.Models;
using EarthquakeReporter.Repositories;

namespace EarthquakeReporter.Controllers
{
    [ApiController]
    [Route("api/earthquakes")]
    public class EarthquakeEventsController : ControllerBase
    {
        private readonly IEarthquakeEventRepository _repository;

        public EarthquakeEventsController(IEarthquakeEventRepository repository)
        {
            _repository = repository;
        }

        // Get all earthquake events
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var earthquakes = await _repository.GetAllAsync();
            return Ok(earthquakes);
        }

        // Get earthquake event by ID
        [HttpGet("{eventId}")]
        [ApiExplorerSettings(IgnoreApi = true)] // Exclude from Swagger
        public async Task<IActionResult> GetByEventId(string eventId)
        {
            var earthquake = await _repository.GetByEventIDAsync(eventId);
            if (earthquake == null)
            {
                return NotFound($"No earthquake found with ID {eventId}");
            }
            return Ok(earthquake);
        }

        // Get earthquakes with magnitude greater than the specified value
        [HttpGet("greater-than/{magnitude}")]
        public async Task<IActionResult> GetGreaterThanMagnitudeAsync(double magnitude)
        {
            var earthquakes = await _repository.GetGreaterThanMagnitudeAsync(magnitude);
            if (earthquakes == null || !earthquakes.Any())
            {
                return NotFound($"No earthquakes found with magnitude greater than {magnitude}");
            }
            return Ok(earthquakes);
        }

        // Create a new earthquake event
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EarthquakeEvent earthquakeEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.CreateAsync(earthquakeEvent);
            return CreatedAtAction(nameof(GetByEventId), new { eventId = earthquakeEvent.EventID }, earthquakeEvent);
        }

        // Delete earthquakes with magnitudes smaller than the given threshold
        [HttpDelete("smaller-than/{magnitude}")]
        public async Task<IActionResult> DeleteSmallerThanMagnitude(double magnitude)
        {
            await _repository.DeleteSmallerThanMagnitudeAsync(magnitude);
            return NoContent();
        }

        // Get earthquakes by country
        [HttpGet("by-country/{country}")]
        public async Task<IActionResult> GetByCountry(string country)
        {
            var earthquakes = await _repository.GetByCountryAsync(country);
            if (earthquakes == null || !earthquakes.Any())
            {
                return NotFound($"No earthquakes found in {country}");
            }
            return Ok(earthquakes);
        }

        // Get earthquakes by province
        [HttpGet("by-province/{province}")]
        public async Task<IActionResult> GetByProvince(string province)
        {
            var earthquakes = await _repository.GetByProvinceAsync(province);
            if (earthquakes == null || !earthquakes.Any())
            {
                return NotFound($"No earthquakes found in {province}");
            }
            return Ok(earthquakes);
        }

        // Update an earthquake event
        [HttpPut("{eventId}")]
        [ApiExplorerSettings(IgnoreApi = true)] // Exclude from Swagger
        public async Task<IActionResult> Update(string eventId, [FromBody] EarthquakeEvent earthquakeEvent)
        {
            if (eventId != earthquakeEvent.EventID)
            {
                return BadRequest("EventID in URL and body do not match.");
            }

            var existingEvent = await _repository.GetByEventIDAsync(eventId);
            if (existingEvent == null)
            {
                return NotFound($"No earthquake found with ID {eventId}");
            }

            await _repository.UpdateAsync(earthquakeEvent);
            return NoContent();
        }

        // Delete an earthquake event by ID
        [HttpDelete("{eventId}")]
        [ApiExplorerSettings(IgnoreApi = true)] // Exclude from Swagger
        public async Task<IActionResult> Delete(string eventId)
        {
            var existingEvent = await _repository.GetByEventIDAsync(eventId);
            if (existingEvent == null)
            {
                return NotFound($"No earthquake found with ID {eventId}");
            }

            await _repository.DeleteAsync(eventId);
            return NoContent();
        }

        [HttpGet("filter")]
public async Task<IActionResult> FilterEarthquakes(
    [FromQuery] double? magnitude = null,
    [FromQuery] string location = null,
    [FromQuery] string filterType = null)
{
    try
    {
        // Step 1: Fetch earthquakes based on location filter if provided
        IEnumerable<EarthquakeEvent> earthquakes;

        if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(filterType))
        {
            if (filterType.Equals("country", StringComparison.OrdinalIgnoreCase))
            {
                earthquakes = await _repository.GetByCountryAsync(location);
                if (earthquakes == null || !earthquakes.Any())
                {
                    return NotFound($"No earthquakes found in country '{location}'.");
                }
            }
            else if (filterType.Equals("province", StringComparison.OrdinalIgnoreCase))
            {
                earthquakes = await _repository.GetByProvinceAsync(location);
                if (earthquakes == null || !earthquakes.Any())
                {
                    return NotFound($"No earthquakes found in province '{location}'.");
                }
            }
            else
            {
                return BadRequest("Invalid filter type.");
            }
        }
        else
        {
            // No location filter, get all events
            earthquakes = await _repository.GetAllAsync();
        }

        // Step 2: Apply Magnitude Filter
        if (magnitude.HasValue)
        {
            var filteredByMagnitude = await _repository.GetGreaterThanMagnitudeAsync(magnitude.Value);

            // Check if the magnitude filter is applied after location filter
            if (earthquakes.Any())
            {
                earthquakes = earthquakes.Intersect(filteredByMagnitude, new EarthquakeEventComparer());
                if (!earthquakes.Any())
                {
                    return NotFound($"No earthquakes found with magnitude greater than {magnitude.Value} in the filtered location.");
                }
            }
            else
            {
                // If no earthquakes were found with the location filter, notify the user
                return NotFound($"No earthquakes found with magnitude greater than {magnitude.Value}.");
            }
        }
        else if (!earthquakes.Any())
        {
            // No earthquakes found after applying location filter
            return NotFound("No earthquakes found with the applied location filter.");
        }

        // If no earthquakes found after all filters
        if (!earthquakes.Any())
        {
            return NotFound("No earthquakes found with the applied filters.");
        }

        return Ok(earthquakes);
    }
    catch (Exception ex)
    {
        // Log exception (consider using a logging framework)
        return StatusCode(500, "Internal server error");
    }
}


    }

    public class EarthquakeEventComparer : IEqualityComparer<EarthquakeEvent>
{
    public bool Equals(EarthquakeEvent x, EarthquakeEvent y)
    {
        if (x == null || y == null) return false;
        return x.EventID == y.EventID;
    }

    public int GetHashCode(EarthquakeEvent obj)
    {
        return obj.EventID?.GetHashCode() ?? 0;
    }
}

}
