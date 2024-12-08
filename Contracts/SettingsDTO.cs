using MongoDB.Bson.Serialization.Attributes;

namespace RegLab_Test.Contracts
{
    public class SettingsDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public SettingDTO Settings { get; set; }
        public class SettingDTO
        {
            public int? TextSize { get; set; }
            public string? Color { get; set; }
        }
    }
}
