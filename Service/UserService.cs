using LoginAPI.Domain;
using LoginAPI.Utility;

namespace LoginAPI.Service;

public class UserService : IUserService
{
    private readonly IUserRepository repo;

    public UserService(IUserRepository repo)
    {
        this.repo = repo;
    }

    public User GetUser(string username, string passwd)
    {
        return repo.GetUser(username, passwd);
    }

    public User GetUserById(int uid)
    {
        return repo.GetUserById(uid);
    }

    public string GenAndSaveToken(int uid)
    {
        var accessToken = JwtUtility.CreateAccessToken(uid);
        if (repo.SaveAccessToken(uid, accessToken))
        {
            return accessToken;
        }
        else
        {
            return string.Empty;
        }
    }

    public bool DeleteAccessToken(int uid)
    {
        return repo.DeleteAccessToken(uid);
    }
}
