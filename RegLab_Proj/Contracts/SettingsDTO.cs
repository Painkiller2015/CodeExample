namespace RegLab_Test.Contracts
{
    public class SettingsDTO
    {
        public required int UserId { get; set; }
        public required string Name { get; set; }
        public required DateTime CreatedAt { get; set; }

        public SettingDTO Settings { get; set; }
        public SettingsDTO()
        {

        }

        public SettingsDTO Clone()
        {
            return (SettingsDTO)this.MemberwiseClone();
        }

        public class SettingDTO
        {
            public int? TextSize { get; set; }
            public string? Color { get; set; }
        }
    }
}
