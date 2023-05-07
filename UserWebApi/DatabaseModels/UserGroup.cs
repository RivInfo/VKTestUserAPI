using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace UserWebApi.DatabaseModels;

[Table("user_group")]
[Index(nameof(Code))]
public class UserGroup
{
    [Key, Column("id")] public long Id { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required, Column("code")] public UserGroupCode Code { get; set; }

    [Column("description")] public string? Description { get; set; }
}

public enum UserGroupCode
{
    Admin,
    User
}