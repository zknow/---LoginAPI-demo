namespace LoginAPI.Domain;

public class User
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Account { get; set; }
    public string? Password { get; set; }
}

public interface IUserService
{
    public User GetUser(string Account, string passwd);
    public User GetUserById(int uid);
    public string GenAndSaveToken(int uid);
    public bool DeleteAccessToken(int uid);
}

public interface IUserRepository
{
    public User GetUser(string Account, string passwd);
    public User GetUserById(int uid);
    public bool SaveAccessToken(int uid, string token);
    public bool DeleteAccessToken(int uid);
}