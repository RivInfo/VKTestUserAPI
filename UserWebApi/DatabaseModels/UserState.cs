using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserWebApi.DatabaseModels;

[Table("user_state")]
public class UserState
{
    [Key, Column("id")] public long Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required, Column("code")] public UserStateCode Code { get; set; }

    [Column("description")] public string? Description { get; set; }
}

public enum UserStateCode
{
    Active,
    Blocked
}