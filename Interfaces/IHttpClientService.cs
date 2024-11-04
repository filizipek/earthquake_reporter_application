namespace EarthquakeReporter.Services
{
    public interface IHttpClientService<T> where T : new()
    {
        Task<T> GetDataAsync(string resource, CancellationToken cancellationToken = default);
        // Define other methods as needed...
    }
}
