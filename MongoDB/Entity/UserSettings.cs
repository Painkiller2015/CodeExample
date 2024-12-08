using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RegLab_Test.MongoDB.Entity
{
    [Index(nameof(UserId))]
    public class UserSettings
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("user_id")]
        public int UserId { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("settings")]
        public Setting Settings { get; set; }
        public class Setting
        {
            [BsonIgnoreIfDefault]
            [BsonElement("text_size")]
            public int? TextSize { get; set; }
            [BsonIgnoreIfDefault]
            [BsonElement("color")]
            public string? Color { get; set; }
        }

        public void Update(UserSettings userSettings)
        {
            UserId = userSettings.UserId;
            Name = userSettings.Name;
            Settings = userSettings.Settings;
        }
    }
}
