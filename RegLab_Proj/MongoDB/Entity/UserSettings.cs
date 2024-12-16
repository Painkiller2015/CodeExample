using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RegLab_Test.Mongodb.UserSettings.Entity
{
    [Index(nameof(UserId))]
    [Index(nameof(Name))]
    [Index(nameof(CreatedAt))]
    public class UserSettings
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("user_id")]
        public int UserId { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("name")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("settings")]
        public Setting Settings { get; set; }
        public class Setting
        {
            [BsonElement("text_size")]
            public int? TextSize { get; set; }
            [BsonElement("color")]
            public string? Color { get; set; }
        }
        public void Update(UserSettings us)
        {
            Name = us.Name;
            Settings = us.Settings;
        }
    }
}
