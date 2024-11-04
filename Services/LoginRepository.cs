using System;
using System.Threading.Tasks;
using Dapper;
using EarthquakeReporter.Models;
using BCrypt.Net;

public interface ILoginRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> RegisterUserAsync(User user);
    Task<User> AuthenticateUserAsync(string email, string password);
}

public class LoginRepository : ILoginRepository
{
    private readonly IDapperContext _dapperContext;

    public LoginRepository(IDapperContext dapperContext)
    {
        _dapperContext = dapperContext ?? throw new ArgumentNullException(nameof(dapperContext));
    }

    public async Task<bool> RegisterUserAsync(User user)
    {
        try
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                // Check if the email already exists
                var emailExists = await EmailExistsAsync(user.Email);
                if (emailExists)
                {
                    Console.WriteLine($"Email already exists: {user.Email}");
                    return false;
                }

                // Hash the user's password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                // SQL query to insert user data
                var query = @"
                    INSERT INTO dbo.Users (Name, Surname, Email, Password, Birthday, Province, Country) 
                    VALUES (@Name, @Surname, @Email, @Password, @Birthday, @Province, @Country)";

                // Parameters to be passed to the query
                var parameters = new
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Password = hashedPassword,
                    Birthday = user.Birthday,
                    Province = user.Province, // Ensure these fields are not null or empty
                    Country = user.Country   // Ensure these fields are not null or empty
                };

                // Execute the query
                var result = await connection.ExecuteAsync(query, parameters);
                return result > 0; // Return true if insert was successful
            }
        }
        catch (Exception ex)
        {
            // Log exception
            Console.Error.WriteLine($"Error in RegisterUserAsync: {ex.Message}");
            return false; // Return false if any error occurs
        }
    }

    public async Task<User> AuthenticateUserAsync(string email, string password)
    {
        try
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                // SQL query to retrieve the user by email
                var query = "SELECT * FROM dbo.Users WHERE Email = @Email";
                var parameters = new { Email = email };

                // Retrieve the user from the database
                var user = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);

                if (user == null)
                {
                    Console.WriteLine("User not found");
                    return null; // Return null if user is not found
                }

                // Verify the password using BCrypt
                bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

                if (!passwordValid)
                {
                    Console.WriteLine("Invalid password");
                    return null; // Return null if password is invalid
                }

                return user; // Return the user if authentication is successful
            }
        }
        catch (Exception ex)
        {
            // Log exception
            Console.Error.WriteLine($"Error during authentication: {ex.Message}");
            return null; // Return null if any error occurs
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        try
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                // SQL query to check if the email exists
                var query = "SELECT COUNT(1) FROM dbo.Users WHERE Email = @Email";
                var parameters = new { Email = email };

                // Execute the query and get the result
                var count = await connection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0; // Return true if email exists
            }
        }
        catch (Exception ex)
        {
            // Log exception
            Console.Error.WriteLine($"Error in EmailExistsAsync: {ex.Message}");
            return false; // Return false in case of an error
        }
    }

    
}
