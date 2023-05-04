namespace UserWebApi.SingltonServices;

public class LoginBlock : ILoginBlock
{
    private HashSet<string> _blockLogin = new HashSet<string>();


    public bool TryBlockLogin(string login)
    {
        if (_blockLogin.Contains(login))
            return false;

        _blockLogin.Add(login);
        return true;
    }

    public void UnblockLogin(string login)
    {
        _blockLogin.Remove(login);
    }
}