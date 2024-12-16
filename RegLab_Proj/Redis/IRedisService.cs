using RegLab_Test.Contracts;

namespace RegLab_Test.Redis
{
    public interface IRedisService
    {
        /// <summary>
        /// Добавляет новую настройку для пользователя.
        /// </summary>
        /// <param name="newSetting">Настройка, которую нужно добавить.</param>
        /// <returns>Возвращает true, если настройка была успешно добавлена; иначе false.</returns>
        Task<SettingsDTO> AddNewSettingAsync(SettingsDTO newSetting);

        /// <summary>
        /// Получает все настройки пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список настроек пользователя.</returns>
        Task<List<SettingsDTO>> GetAllUserSettingsAsync(int userId);

        /// <summary>
        /// Получает конкретную настройку пользователя по имени.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="settingName">Имя настройки.</param>
        /// <returns>Настройка пользователя или null, если не найдена.</returns>
        Task<SettingsDTO> GetUserSettingAsync(int userId, string settingName);

        /// <summary>
        /// Удаляет настройку пользователя по имени.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="settingName">Имя настройки для удаления.</param>
        /// <returns>Возвращает true, если настройка была успешно удалена; иначе false.</returns>
        Task<bool> RemoveUserSettingAsync(int userId, string settingName);

        /// <summary>
        /// Обновляет существующую настройку пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="settingName">Имя настройки для обновления.</param>
        /// <param name="updatedSetting">Новые данные настройки.</param>
        /// <returns>Обновленная настройка или null, если обновление не удалось.</returns>
        Task<SettingsDTO> UpdateUserSettingAsync(int userId, string settingName, SettingsDTO updatedSetting);
    }
}