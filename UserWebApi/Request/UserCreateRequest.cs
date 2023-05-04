using System.ComponentModel.DataAnnotations;

namespace UserWebApi.Request;

public class UserCreateRequest
{
    [Required] public string Login { get; set; }

    [Required] public string Password { get; set; }

    public bool IsAdmin { get; set; }
}