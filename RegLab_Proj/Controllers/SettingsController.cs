using Microsoft.AspNetCore.Mvc;
using RegLab_Test.Contracts;
using RegLab_Test.Logger;
using RegLab_Test.SettingsService;

namespace RegLab_Test.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SettingsController : ControllerBase, ISettingsController
    {
        private readonly ISettingsService _settingsService;
        private readonly ILoggerService _logger;

        public SettingsController(ISettingsService settingsService, ILoggerService logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        [HttpGet("Get", Name = "GetUserSetting")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SettingsDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int userId, string settingName)
        {
            var validationResult = Validate();
            if (validationResult != null)
                return validationResult;

            var response = await _settingsService.GetAsync(userId, settingName);
            return HandleApiResponseError(response);
        }

        [HttpGet("GetAll", Name = "GetAllUserSettings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SettingsDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(int userId, string sortBy = "CreatedAt", bool asc = true)
        {
            var validationResult = Validate();
            if (validationResult != null)
                return validationResult;

            var settingsResponse = await _settingsService.GetAllAsync(userId, sortBy, asc);
            return HandleApiResponseError(settingsResponse);

        }

        [HttpPost("AddSetting", Name = "AddSetting")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SettingsDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] SettingsDTO document)
        {
            var validationResult = Validate();
            if (validationResult != null)
                return validationResult;

            var result = await _settingsService.CreateAsync(document);
            return HandleApiResponseError(result);
        }

        [HttpPatch("UpdateSetting", Name = "UpdateSetting")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SettingsDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int userId, string settingsName, [FromBody] SettingsDTO updateSetting)
        {
            var validationResult = Validate();
            if (validationResult != null)
                return validationResult;

            var updatedSetting = await _settingsService.UpdateAsync(userId, settingsName, updateSetting);
            return HandleApiResponseError(updatedSetting);
        }

        [HttpDelete("DeleteSetting", Name = "DeleteSetting")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int userId, string settingsName)
        {
            var validationResult = Validate();
            if (validationResult != null)
                return validationResult;

            var isDeleted = await _settingsService.DeleteAsync(userId, settingsName);
            return HandleApiResponseError(isDeleted);
        }
        private BadRequestObjectResult Validate()
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errorMessages });
            }
            return null;
        }
        private IActionResult HandleApiResponseError<T>(ApiResponse<T> response)
        {
            if (response.Success)
            {
                return Ok(response);
            }

            switch (response.ErrorCode)
            {
                case 400:
                    return BadRequest(response);
                case 404:
                    return NotFound(response);
                case 409:
                    return Conflict(response);
                case 500:
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                default:
                    return BadRequest(response);
            }
        }
    }
}
