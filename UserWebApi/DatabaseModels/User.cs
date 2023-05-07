using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace UserWebApi.DatabaseModels;

[Table("user")]
[Index(nameof(Login), IsUnique = true)]
public class User
{
    [Key, Column("id")] public long Id { get; set; }

    [Required, Column("login")] public string Login { get; set; }

    [Required, Column("password")] public string Password { get; set; }

    [JsonIgnore]
    [Column("user_group_id")] public long UserGroupId { get; set; }
    public UserGroup UserGroup { get; set; }
    
    [JsonIgnore]
    [Column("user_state_id")] public long UserStateId { get; set; }
    public UserState UserState { get; set; }
}