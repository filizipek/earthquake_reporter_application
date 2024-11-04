using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using EarthquakeReporter.Models;

namespace EarthquakeReporter.Repositories
{
    public interface IEarthquakeEventRepository
    {
        Task<IEnumerable<EarthquakeEvent>> GetAllAsync();
        Task<EarthquakeEvent?> GetByEventIDAsync(string eventId);
        Task<IEnumerable<EarthquakeEvent>> GetByCountryAsync(string country);
        Task<IEnumerable<EarthquakeEvent>> GetByProvinceAsync(string province);
        Task CreateAsync(EarthquakeEvent earthquakeEvent);
        Task UpdateAsync(EarthquakeEvent earthquakeEvent);
        Task DeleteAsync(string eventId);
        Task DeleteAllAsync();
        Task<IEnumerable<EarthquakeEvent>> GetByDateAsync(string date);
        Task DeleteByDateAsync(string date);
        Task<IEnumerable<EarthquakeEvent>> GetByMagnitudeAsync(double magnitude);
        Task DeleteSmallerThanMagnitudeAsync(double magnitude);
        Task<IEnumerable<EarthquakeEvent>> GetGreaterThanMagnitudeAsync(double magnitude);
    }

    public class EarthquakeEventRepository : IEarthquakeEventRepository
    {
        private readonly string _connectionString;

        public EarthquakeEventRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<EarthquakeEvent>> GetAllAsync()
        {
            const string sql = @"
                SELECT
                    Resource,
                    EventID,
                    Location,
                    Latitude,
                    Longitude,
                    Depth,
                    Type,
                    Magnitude,
                    Country,
                    Province,
                    District,
                    Neighborhood,
                    [Date] AS Date,
                    IsEventUpdate,
                    LastUpdateDate
                FROM dbo.EarthquakeEvents";

            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    return await dbConnection.QueryAsync<EarthquakeEvent>(sql);
                }
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error retrieving earthquake events.", ex);
            }
        }


        public async Task<EarthquakeEvent?> GetByEventIDAsync(string eventId)
        {
            const string sql = @"
                SELECT
                    Resource,
                    EventID,
                    Location,
                    Latitude,
                    Longitude,
                    Depth,
                    Type,
                    Magnitude,
                    Country,
                    Province,
                    District,
                    Neighborhood,
                    [Date] AS Date,
                    IsEventUpdate,
                    LastUpdateDate
                FROM dbo.EarthquakeEvents
                WHERE EventID = @EventID";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                return await dbConnection.QueryFirstOrDefaultAsync<EarthquakeEvent>(sql, new { EventID = eventId });
            }
        }

        public async Task<IEnumerable<EarthquakeEvent>> GetByCountryAsync(string country)
        {
            const string sql = @"
                SELECT
                    Resource,
                    EventID,
                    Location,
                    Latitude,
                    Longitude,
                    Depth,
                    Type,
                    Magnitude,
                    Country,
                    Province,
                    District,
                    Neighborhood,
                    [Date] AS Date,
                    IsEventUpdate,
                    LastUpdateDate
                FROM dbo.EarthquakeEvents
                WHERE Country = @Country";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                return await dbConnection.QueryAsync<EarthquakeEvent>(sql, new { Country = country });
            }
        }

        public async Task<IEnumerable<EarthquakeEvent>> GetByProvinceAsync(string province)
        {
            const string sql = @"
                SELECT
                    Resource,
                    EventID,
                    Location,
                    Latitude,
                    Longitude,
                    Depth,
                    Type,
                    Magnitude,
                    Country,
                    Province,
                    District,
                    Neighborhood,
                    [Date] AS Date,
                    IsEventUpdate,
                    LastUpdateDate
                FROM dbo.EarthquakeEvents
                WHERE Province = @Province";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                return await dbConnection.QueryAsync<EarthquakeEvent>(sql, new { Province = province });
            }
        }

        public async Task CreateAsync(EarthquakeEvent earthquakeEvent)
        {
            const string sql = @"
                INSERT INTO dbo.EarthquakeEvents
                (Resource, EventID, Location, Latitude, Longitude, Depth, Type, Magnitude, Country, Province, District, Neighborhood, [Date], IsEventUpdate, LastUpdateDate)
                VALUES
                (@Resource, @EventID, @Location, @Latitude, @Longitude, @Depth, @Type, @Magnitude, @Country, @Province, @District, @Neighborhood, @Date, @IsEventUpdate, @LastUpdateDate)";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.ExecuteAsync(sql, earthquakeEvent);
            }
        }


        public async Task UpdateAsync(EarthquakeEvent earthquakeEvent)
        {
            const string sql = @"
                UPDATE dbo.EarthquakeEvents
                SET
                    Resource = @Resource,
                    Location = @Location,
                    Latitude = @Latitude,
                    Longitude = @Longitude,
                    Depth = @Depth,
                    Type = @Type,
                    Magnitude = @Magnitude,
                    Country = @Country,
                    Province = @Province,
                    District = @District,
                    Neighborhood = @Neighborhood,
                    [Date] = @Date,
                    IsEventUpdate = @IsEventUpdate,
                    LastUpdateDate = @LastUpdateDate
                WHERE EventID = @EventID";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.ExecuteAsync(sql, earthquakeEvent);
            }
        }


        public async Task DeleteAsync(string eventId)
        {
            const string sql = @"
                DELETE FROM dbo.EarthquakeEvents
                WHERE EventID = @EventID";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.ExecuteAsync(sql, new { EventID = eventId });
            }
        }

        public async Task DeleteAllAsync()
        {
            const string sql = @"
                DELETE FROM dbo.EarthquakeEvents";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.ExecuteAsync(sql);
            }
        }

        public async Task<IEnumerable<EarthquakeEvent>> GetByMagnitudeAsync(double magnitude)
        {
            const string sql = @"
                SELECT
                    Resource,
                    EventID,
                    Location,
                    Latitude,
                    Longitude,
                    Depth,
                    Type,
                    Magnitude,
                    Country,
                    Province,
                    District,
                    Neighborhood,
                    [Date] AS Date,
                    IsEventUpdate,
                    LastUpdateDate
                FROM dbo.EarthquakeEvents
                WHERE CAST(Magnitude AS FLOAT) = @Magnitude";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                return await dbConnection.QueryAsync<EarthquakeEvent>(sql, new { Magnitude = magnitude });
            }
        }

        public async Task DeleteSmallerThanMagnitudeAsync(double magnitude)
        {
            const string sql = @"
                DELETE FROM dbo.EarthquakeEvents
                WHERE CAST(Magnitude AS FLOAT) < @Magnitude";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.ExecuteAsync(sql, new { Magnitude = magnitude });
            }
        }

        public async Task<IEnumerable<EarthquakeEvent>> GetGreaterThanMagnitudeAsync(double magnitude)
        {
            const string sql = @"
                SELECT
                    Resource,
                    EventID,
                    Location,
                    Latitude,
                    Longitude,
                    Depth,
                    Type,
                    Magnitude,
                    Country,
                    Province,
                    District,
                    Neighborhood,
                    [Date] AS Date,
                    IsEventUpdate,
                    LastUpdateDate
                FROM dbo.EarthquakeEvents
                WHERE CAST(Magnitude AS FLOAT) > @Magnitude";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                return await dbConnection.QueryAsync<EarthquakeEvent>(sql, new { Magnitude = magnitude });
            }
        }

        public async Task<IEnumerable<EarthquakeEvent>> GetByDateAsync(string date)
        {
            const string sql = @"
                SELECT
                    Resource,
                    EventID,
                    Location,
                    Latitude,
                    Longitude,
                    Depth,
                    Type,
                    Magnitude,
                    Country,
                    Province,
                    District,
                    Neighborhood,
                    [Date] AS Date,
                    IsEventUpdate,
                    LastUpdateDate
                FROM dbo.EarthquakeEvents
                WHERE [Date] = @Date";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                return await dbConnection.QueryAsync<EarthquakeEvent>(sql, new { Date = date });
            }
        }

        public async Task DeleteByDateAsync(string date)
        {
            const string sql = @"
                DELETE FROM dbo.EarthquakeEvents
                WHERE [Date] = @Date";

            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.ExecuteAsync(sql, new { Date = date });
            }
        }
    }
}
