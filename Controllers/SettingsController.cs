using Microsoft.AspNetCore.Mvc;
using MongoDB_Service;
using MongoDB_Service.MongoDB;
using MongoDB_Service.MongoDB;
using Microsoft.EntityFrameworkCore;
using UserSettings = RegLab_Test.MongoDB.Entity.UserSettings;
using AutoMapper;
using RegLab_Test.Contracts;
using Microsoft.AspNetCore.SignalR;
using RegLab_Test.SignalR;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RegLab_Test.Redis;

namespace RegLab_Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly DataContext _db = new();
        private readonly ILogger<SettingsController> _logger;
        private readonly IMapper _mapper;
        private readonly IHubContext<SettingsHub, ISettingsHub> _hubContext;
        private readonly IDistributedCache _cache;

        public SettingsController(ILogger<SettingsController> logger, IMapper mapper, IDistributedCache cache, IHubContext<SettingsHub, ISettingsHub> hubContext)
        {
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
            _hubContext = hubContext;
        }

        [HttpGet(Name = "GetUserSetting")]
        public async Task<ActionResult<SettingsDTO>> Get(int userId, string settingName)
        {
            try
            {
                var dto = await RedisServiceExtension.GetSetting(_cache, userId, settingName);

                if (dto is null)
                {
                    var entity = await _db.UserSettings
                        .AsNoTracking()
                        .FirstOrDefaultAsync(doc => doc.UserId == userId && doc.Name == settingName);

                    if (entity == null)
                    {
                        return NotFound();
                    }

                    dto = _mapper.Map<SettingsDTO>(entity);

                    await RedisServiceExtension.AddNewSetting(_cache, dto);
                }

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet(Name = "GetAllUserSettings")]
        public async Task<ActionResult<List<SettingsDTO>>> GetAll(int userId)
        {
            try
            {
                var dto = await RedisServiceExtension.GetAllSettings(_cache, userId);

                if (dto is null)
                {
                    var entity = await _db.UserSettings
                        .AsNoTracking()
                        .Where(doc => doc.UserId == userId).ToListAsync();

                    if (entity.Count == 0)
                    {
                        return NotFound();
                    }

                    dto = entity.Select(el => _mapper.Map<SettingsDTO>(entity)).ToList();

                    await RedisServiceExtension.AddUserSettings(_cache, userId, dto);
                }
                
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost(Name = "AddSetting")]
        public async Task<ActionResult<SettingsDTO>> Create(SettingsDTO document)
        {
            try
            {
                var entity = _mapper.Map<UserSettings>(document);

                await _db.UserSettings.AddAsync(entity);
                await _db.SaveChangesAsync();

                await SettingsHubExtension.SendSettingsAddNotification(_hubContext);
                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch(Name = "UpdateSetting")]
        public async Task<ActionResult<SettingsDTO>> Update(int userId, string settingsName, SettingsDTO newDocument)
        {
            try
            {
                var settingForUpdate =
                    await _db.UserSettings.FirstOrDefaultAsync(doc => doc.UserId == userId && doc.Name == settingsName);

                if (settingForUpdate is null)
                {
                    return NotFound();
                }

                var entity = _mapper.Map<UserSettings>(newDocument);
                settingForUpdate.Update(entity);
                await _db.SaveChangesAsync();

                await SettingsHubExtension.SendSettingsUpdateNotification(_hubContext);
                return Ok(newDocument);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete(Name = "DeleteSetting")]
        public async Task<bool> Delete(int userId, string settingsName)
        {
            var settingForDel = await _db.UserSettings
                .FirstOrDefaultAsync(doc => doc.UserId == userId && doc.Name == settingsName);

            if (settingForDel is null)
            {
                return false;
            }

            _db.UserSettings.Remove(settingForDel);
            await _db.SaveChangesAsync();

            await SettingsHubExtension.SendSettingsDeleteNotification(_hubContext);
            return true;
        }
    }
}
