namespace PinWindow;

internal sealed record UpdateInfo(Version Version, string ReleaseUrl);

internal static class UpdateChecker
{
    private const string LatestReleasePageUrl =
        "https://github.com/helliong/pinWindow/releases/latest";

    private static readonly HttpClient Client = CreateClient();

    public static async Task<UpdateInfo?> CheckAsync()
    {
        try
        {
            using var response = await Client.GetAsync(
                LatestReleasePageUrl,
                HttpCompletionOption.ResponseHeadersRead
            );

            var redirectUrl = response.Headers.Location;

            if (redirectUrl is null)
            {
                return null;
            }

            if (!redirectUrl.IsAbsoluteUri)
            {
                redirectUrl = new Uri(new Uri("https://github.com"), redirectUrl);
            }

            var tag = redirectUrl.Segments.LastOrDefault()?.Trim('/');

            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }

            var normalizedTag = Uri.UnescapeDataString(tag).Trim().TrimStart('v', 'V');

            if (!Version.TryParse(normalizedTag, out var latestVersion))
            {
                return null;
            }

            var currentVersion = typeof(UpdateChecker).Assembly.GetName().Version;

            if (currentVersion is null || latestVersion.CompareTo(currentVersion) <= 0)
            {
                return null;
            }

            return new UpdateInfo(latestVersion, redirectUrl.ToString());
        }
        catch
        {
            // Ошибка сети не должна мешать работе PinWindow.
            return null;
        }
    }

    private static HttpClient CreateClient()
    {
        var handler = new HttpClientHandler { AllowAutoRedirect = false };

        var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(10) };

        client.DefaultRequestHeaders.UserAgent.ParseAdd("PinWindow/3.3.3");

        return client;
    }
}
