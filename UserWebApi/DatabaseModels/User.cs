using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserWebApi.DatabaseModels;

[Table("user")]
[Index(nameof(Login), IsUnique = true)]
public class User
{
    [Key, Column("id")] public long Id { get; set; }

    [Required, Column("login")] public string Login { get; set; }

    [Required, Column("password")] public string Password { get; set; }

    [ForeignKey("user_group_id")]
    public UserGroup UserGroup { get; set; }

    [ForeignKey("user_state_id")]
    public UserState UserState { get; set; }
}