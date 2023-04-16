using LoginAPI.DB;
using LoginAPI.Domain;
using StackExchange.Redis;

namespace LoginAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly DbCtx dbContext;
    private readonly ConnectionMultiplexer redisCtx;
    private readonly ISubscriber subscriber;
    private const string REDIS_CHANNEL = "user_login";

    public UserRepository(DbCtx dbContext, ConnectionMultiplexer redisContext)
    {
        this.dbContext = dbContext;
        this.redisCtx = redisContext;
        subscriber = redisContext.GetSubscriber();
        subscriber.Subscribe(REDIS_CHANNEL, SubcribeEvent);
    }

    public bool SaveAccessToken(int uid, string token)
    {
        var redis = redisCtx.GetDatabase();
        if (redis.StringSet($"user:{uid}:jwt", token, TimeSpan.FromHours(1)))
        {
            return subscriber.Publish(REDIS_CHANNEL, $"LoginTime:{DateTime.UtcNow}, User:{uid}, AccessToken:{token}") > 0;
        }
        return false;
    }

    public bool DeleteAccessToken(int uid)
    {
        var redis = redisCtx.GetDatabase();
        if (redis.KeyDelete($"user:{uid}:jwt"))
        {
            return subscriber.Publish(REDIS_CHANNEL, $"LogoutTime:{DateTime.UtcNow}, User:{uid}") > 0;
        }
        return false;
    }

    private void SubcribeEvent(RedisChannel chan, RedisValue value)
    {
        try
        {
            File.AppendAllText("log.text", value.ToString());
        }
        catch (System.Exception)
        {
            Console.WriteLine("寫檔案失敗");
        }

    }

    public User GetUser(string account, string passwd)
    {
        return dbContext.Users.FirstOrDefault(c => c.Account == account && c.Password == passwd);
    }

    public User GetUserById(int uid)
    {
        return dbContext.Users.FirstOrDefault(c => c.Id == uid);
    }
}
