using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text.Json.Nodes;

namespace Customers.Api.Services;

public sealed class GitHubService : IGitHubService
{
    private readonly ILogger<GitHubService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public GitHubService(ILogger<GitHubService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public Task<bool> IsValidGitHubUser(string username)
    {
        var client = _httpClientFactory.CreateClient("GitHub");
        return Observable
            .FromAsync(() => client.GetAsync($"/users/{username}"))
            .SubscribeOn(TaskPoolScheduler.Default)
            .Retry(5)
            .Timeout(TimeSpan.FromSeconds(5))
            .Do(x => _logger.LogInformation("Does {Username} exists? {StatusCode}",
                username, x.IsSuccessStatusCode))
            .SelectMany(async x =>
            {
                if (x.StatusCode == HttpStatusCode.Forbidden)
                {
                    var responseBody = await x.Content.ReadFromJsonAsync<JsonObject>();
                    var message = responseBody!["message"]!.ToString();
                    throw new HttpRequestException(message);
                }

                return x.StatusCode == HttpStatusCode.OK;
            })
            .Catch<bool, TimeoutException>(_ => Observable.Return(false))
            .Catch<bool, Exception>(ex => Observable.Return(false))
            .ToTask();
    }
}
