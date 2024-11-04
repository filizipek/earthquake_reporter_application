using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public interface IFamilyMemberRepository
{
    Task<List<FamilyMember>> GetFamilyMembersByEmailAsync(string userEmail);
    Task AddFamilyMemberAsync(FamilyMemberDto memberDto);
}

public class FamilyMemberRepository : IFamilyMemberRepository
{
    private readonly IDapperContext _context;
    private readonly ILogger<FamilyMemberRepository> _logger;

    public FamilyMemberRepository(IDapperContext context, ILogger<FamilyMemberRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<FamilyMember>> GetFamilyMembersByEmailAsync(string userEmail)
    {
        const string query = "SELECT * FROM FamilyMembers WHERE UserEmail = @UserEmail";

        try
        {
            using var connection = _context.CreateConnection();
            var members = await connection.QueryAsync<FamilyMember>(query, new { UserEmail = userEmail });
            return members.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving family members.");
            throw new Exception("Error retrieving family members.", ex);
        }
    }

    public async Task AddFamilyMemberAsync(FamilyMemberDto memberDto)
    {
        const string query = "INSERT INTO FamilyMembers (Name, Surname, Province, Country, UserEmail) VALUES (@Name, @Surname, @Province, @Country, @UserEmail)";

        var parameters = new
        {
            Name = memberDto.Name,
            Surname = memberDto.Surname,
            Province = memberDto.Province,
            Country = memberDto.Country,
            UserEmail = memberDto.UserEmail
        };

        try
        {
            using var connection = _context.CreateConnection();
            var existingCountQuery = "SELECT COUNT(*) FROM FamilyMembers WHERE UserEmail = @UserEmail";
            var existingCount = await connection.ExecuteScalarAsync<int>(existingCountQuery, new { UserEmail = memberDto.UserEmail });

            if (existingCount >= 5)
            {
                throw new InvalidOperationException("You have reached the maximum number of family members.");
            }

            await connection.ExecuteAsync(query, parameters);
            _logger.LogInformation("Family member added successfully: {@MemberDto}", memberDto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error adding family member. Details: {@MemberDto}", memberDto);
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding family member. Details: {@MemberDto}", memberDto);
            throw new Exception("Error adding family member.", ex);
        }
    }

}
