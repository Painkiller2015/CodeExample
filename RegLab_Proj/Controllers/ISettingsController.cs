using Microsoft.AspNetCore.Mvc;
using RegLab_Test.Contracts;

namespace RegLab_Test.Controllers
{
    /// <summary>
    /// Интерфейс для управления настройками пользователя.
    /// </summary>
    public interface ISettingsController
    {
        /// <summary>
        /// Создает новую настройку для пользователя.
        /// </summary>
        /// <param name="document">Объект настроек, который нужно создать.</param>
        /// <returns>Результат выполнения операции создания.</returns>
        Task<IActionResult> Create(SettingsDTO document);

        /// <summary>
        /// Удаляет настройку пользователя по имени.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, чью настройку нужно удалить.</param>
        /// <param name="settingsName">Имя настройки, которую нужно удалить.</param>
        /// <returns>Результат выполнения операции удаления.</returns>
        Task<IActionResult> Delete(int userId, string settingsName);

        /// <summary>
        /// Получает конкретную настройку пользователя по имени.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, чью настройку нужно получить.</param>
        /// <param name="settingName">Имя настройки, которую нужно получить.</param>
        /// <returns>Настройка пользователя, если она найдена; в противном случае - 404 Not Found.</returns>
        Task<IActionResult> Get(int userId, string settingName);

        /// <summary>
        /// Получает все настройки для указанного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, чьи настройки нужно получить.</param>
        /// <returns>Список настроек пользователя.</returns>
        Task<IActionResult> GetAll(int userId, string sortBy, bool asc);

        /// <summary>
        /// Обновляет существующую настройку пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, чью настройку нужно обновить.</param>
        /// <param name="settingsName">Имя настройки, которую нужно обновить.</param>
        /// <param name="updateSetting">Объект с новыми данными для обновления настройки.</param>
        /// <returns>Результат выполнения операции обновления.</returns>
        Task<IActionResult> Update(int userId, string settingsName, SettingsDTO updateSetting);
    }

}