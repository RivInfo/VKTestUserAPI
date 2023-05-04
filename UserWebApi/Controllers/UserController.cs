using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserWebApi.Contexts;
using UserWebApi.DatabaseModels;
using UserWebApi.Request;
using UserWebApi.SingltonServices;

namespace UserWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserContext _userContext;
    private readonly ILoginBlock _loginBlock;

    public UserController(ILogger<UserController> logger, UserContext userContext, ILoginBlock loginBlock)
    {
        _logger = logger;
        _userContext = userContext;
        _loginBlock = loginBlock;
    }

    [HttpPost]
    public async Task<ActionResult<User>> UserCreate(UserCreateRequest createRequest)
    {
        if (createRequest.IsAdmin)
        {
            UserGroup? userGroupAdmin =
                await _userContext.UsersGroup.FirstOrDefaultAsync(x => x.Code == UserGroupCode.Admin);

            if (userGroupAdmin != null)
                return BadRequest();
        }

        if (await GetUserByLogin(createRequest.Login) != null)
            return BadRequest();

        if (!_loginBlock.TryBlockLogin(createRequest.Login))
            return BadRequest();

        UserGroup userGroup = new UserGroup()
        {
            Code = createRequest.IsAdmin ? UserGroupCode.Admin : UserGroupCode.User
        };

        UserState userState = new UserState()
        {
            Code = UserStateCode.Active
        };

        User user = new User()
        {
            Login = createRequest.Login,
            Password = createRequest.Password,
            UserGroup = userGroup,
            UserState = userState
        };

        await _userContext.UsersGroup.AddAsync(userGroup);
        await _userContext.UsersState.AddAsync(userState);

        var newUser = await _userContext.Users.AddAsync(user);

        await _userContext.SaveChangesAsync();

        _loginBlock.UnblockLogin(createRequest.Login);

        return Ok(newUser.Entity);
    }

    [HttpDelete]
    public async Task<ActionResult<User>> DeleteUser(string login)
    {
        User? user = await GetUserByLogin(login);
        
        if (user == null)
            return BadRequest();

        UserState userState = await _userContext.UsersState
            .FirstAsync(x => x.Id == user.Id);
        
        userState.Code = UserStateCode.Blocked;

        await _userContext.SaveChangesAsync();

        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        await _userContext.UsersGroup.LoadAsync();
        await _userContext.UsersState.LoadAsync();
        
        List<User> users = await _userContext.Users.ToListAsync();

        return Ok(users);
    }

    private async Task<User?> GetUserByLogin(string login)
    {
        return await _userContext.Users
            .FirstOrDefaultAsync(x => x.Login == login);
    }
}