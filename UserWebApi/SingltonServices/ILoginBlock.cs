namespace UserWebApi.SingltonServices;

public interface ILoginBlock
{
    bool TryBlockLogin(string login);

    void UnblockLogin(string login);
}