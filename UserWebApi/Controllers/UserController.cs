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
    private readonly UserContext _userContext;
    private readonly ILoginBlock _loginBlock;

    public UserController(UserContext userContext, ILoginBlock loginBlock)
    {
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

        if (await GetUserByLoginAsync(createRequest.Login) != null)
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

        await Task.Delay(5000);

        _loginBlock.UnblockLogin(createRequest.Login);

        return Ok(newUser.Entity);
    }

    [HttpDelete]
    public async Task<ActionResult<User>> DeleteUser(string login)
    {
        User? user = await GetUserByLoginAsync(login);
        
        if (user == null)
            return BadRequest();

        UserState userState = await _userContext.UsersState
            .FirstAsync(x => x.Id == user.UserStateId);

        userState.Code = UserStateCode.Blocked;

        await _userContext.SaveChangesAsync();
        
        await _userContext.UsersGroup
            .FirstAsync(x => x.Id == user.UserGroupId);

        return Ok(user);
    }

    [HttpGet("Users")]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        await _userContext.UsersGroup.LoadAsync();
        await _userContext.UsersState.LoadAsync();
        
        List<User> users = await _userContext.Users.ToListAsync();

        return Ok(users);
    }
    
    [HttpGet("{login}")]
    public async Task<ActionResult<User>> GetUser(string login)
    {
        User? user = await GetUserByLoginAsync(login);
        
        if (user == null)
            return BadRequest();
        
        await _userContext.UsersGroup
            .FirstAsync(x => x.Id == user.UserGroupId);
        await _userContext.UsersState
            .FirstAsync(x => x.Id == user.UserStateId);

        return Ok(user);
    }
    
    [HttpGet("PaginatedUsers")]
    public async Task<ActionResult<List<User>>> GetUsersPaginated(int from = 0, int size = 5)
    {
        if (from < 0 || size <= 0)
            return BadRequest();
        
        var users = _userContext.Users
            .Include(x => x.UserGroup)
            .Include(x => x.UserState);
        
        List<User> usersList = await users.Skip(from).Take(size).ToListAsync();
        return Ok(usersList);
    }

    private async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _userContext.Users
            .FirstOrDefaultAsync(x => x.Login == login);
    }
}