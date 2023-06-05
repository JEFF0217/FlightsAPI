using Business.Exceptions;
using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightManager _flightManager;
        private readonly ILogger<FlightController> _logger;

        public FlightController(IFlightManager flightManager, ILogger<FlightController> logger)
        {
            _flightManager = flightManager;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetFlightData(string origin, string destination)
        {
            try
            {

                if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
                {
                    throw new ArgumentException("Origin and destination must not be null or empty.");
                }

                if (origin.Length != 3 || destination.Length != 3)
                {
                    throw new ArgumentException("Origin and destination must have a length of 3 characters.");
                }

                if (!origin.All(char.IsUpper) || !destination.All(char.IsUpper))
                {
                    throw new ArgumentException("Origin and destination must consist only of uppercase letters.");
                }

                _logger.LogInformation("Searching for flight routes from {origin} to {destination}", origin, destination);
                var flightData = await _flightManager.GetJournies(origin, destination);
                _logger.LogInformation("Flight routes from {origin} to {destination} found successfully.", origin, destination);
                return Ok(flightData);

            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (FlightNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (JourneysNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
