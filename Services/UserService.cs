using Dapper;
using EarthquakeReporter.Models; // Replace with your actual namespace
using System.Threading.Tasks;

public interface IUserService
{
    Task<User> GetUserProfileAsync(string userId);
}

namespace EarthquakeReporter.Services
{
    public class UserService : IUserService
    {
        private readonly IDapperContext _context;

        public UserService(IDapperContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserProfileAsync(string userId)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "SELECT Id, Name, Surname, Email, Province, Country, Birthday FROM dbo.Users WHERE Id = @UserId";
                return await connection.QueryFirstOrDefaultAsync<User>(query, new { UserId = userId });
            }
        }
    }
}
