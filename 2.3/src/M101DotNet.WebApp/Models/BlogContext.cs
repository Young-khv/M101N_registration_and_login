using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

namespace M101DotNet.WebApp.Models
{
    public class BlogContext
    {
        public const string CONNECTION_STRING_NAME = "Blog";
        public const string DATABASE_NAME = "blog";
        public const string POSTS_COLLECTION_NAME = "posts";
        public const string USERS_COLLECTION_NAME = "users";

        // This is ok... Normally, these or the entire BlogContext
        // would be put into an IoC container. We aren't using one,
        // so we'll just keep them statically here as they are 
        // thread-safe.
        private static readonly IMongoClient _client;
        private static readonly IMongoDatabase _database;

        static BlogContext()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NAME].ConnectionString;
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(DATABASE_NAME);
        }

        public IMongoClient Client
        {
            get { return _client; }
        }

        public IMongoCollection<User> Users
        {
            get { return _database.GetCollection<User>(USERS_COLLECTION_NAME); }
        }

        public async Task<User> FindUserByEmailAsync(string userEmail)
        {
            var builder = Builders<User>.Filter;

            var filter = builder.Eq(x => x.Email, userEmail);

            var result = await Users.Find(filter).Limit(1).FirstOrDefaultAsync();
            return result;
        }

        public async Task CreateNewUserAsync(User user)
        {
            if(await Users.CountAsync(x => x.Email == user.Email) == 0)
                await Users.InsertOneAsync(user);
        }
    }
}