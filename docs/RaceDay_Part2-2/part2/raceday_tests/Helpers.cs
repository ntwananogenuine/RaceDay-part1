using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using raceday_api.database;

namespace raceday_tests;

public class RaceDayFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Swap SQL Server for an in-memory database so tests never touch LocalDB
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GetGenericArguments().Contains(typeof(AppDbContext)))).ToList();
            foreach (var d in toRemove) services.Remove(d);

            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(_dbName));
        });
    }

    public RaceDayFactory()
    {
        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }
}

public static class Helpers
{
    public const string Password = "Password123!";

    public static string NewEmail() => $"{Guid.NewGuid():N}@test.com";

    // The API takes its values as parameters (query string), so the tests send them the same way.
    public static Task<HttpResponseMessage> PostParamsAsync(this HttpClient c, string url, object body) =>
        c.PostAsync(url + ToQuery(body), null);

    public static Task<HttpResponseMessage> PutParamsAsync(this HttpClient c, string url, object body) =>
        c.PutAsync(url + ToQuery(body), null);

    private static string ToQuery(object body)
    {
        var parts = body.GetType().GetProperties()
            .Where(p => p.GetValue(body) != null)
            .Select(p =>
            {
                var v = p.GetValue(body)!;
                var text = v is DateTime d
                    ? d.ToString("o", CultureInfo.InvariantCulture)
                    : Convert.ToString(v, CultureInfo.InvariantCulture)!;
                return $"{p.Name}={Uri.EscapeDataString(text)}";
            }).ToList();
        return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
    }

    public static async Task<HttpClient> LoggedInClient(RaceDayFactory f, string role)
    {
        var client = f.CreateClient(); // each client keeps its own session cookie
        var email = NewEmail();
        (await client.PostParamsAsync("/api/auth/register",
            new { fullname = "Test User", email, password = Password, role })).EnsureSuccessStatusCode();
        (await client.PostParamsAsync("/api/auth/login",
            new { email, password = Password })).EnsureSuccessStatusCode();
        return client;
    }

    public static async Task<int> Id(HttpResponseMessage r, string prop)
    {
        var json = await r.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty(prop).GetInt32();
    }

    public static async Task<int> CreateEvent(HttpClient organiser)
    {
        var r = await organiser.PostParamsAsync("/api/events", new
        {
            name = "Test Run", description = "Fun", event_date = DateTime.Today.AddDays(30),
            location = "Pretoria", event_type = "Run", distance_km = 10
        });
        r.EnsureSuccessStatusCode();
        return await Id(r, "eventId");
    }

    public static async Task<int> CreateCategory(HttpClient organiser, int eventId, int max = 100)
    {
        var r = await organiser.PostParamsAsync($"/api/events/{eventId}/categories",
            new { name = "10km", distance_km = 10, max_participants = max });
        r.EnsureSuccessStatusCode();
        return await Id(r, "categoryId");
    }
}
