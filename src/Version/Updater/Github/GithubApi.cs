using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VentLib.Logging;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Version.Updater.Github;

public class GithubApi
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(GithubApi));
    private const string LatestRepoURL = "https://api.github.com/repos/{0}/{1}/releases/latest";
    
    private HttpClient httpClient = new();
    private HttpClient downloadClient = new();

    public GithubApi(string? token = null)
    {
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VentFramework", "0.0.1"));
        downloadClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VentFramework", "0.0.1"));
        if (token != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            downloadClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        downloadClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            
    }
    
    public async void GetLatestRelease(string owner, string repoName, Action<Release> callback)
    {
        bool isFinished = false;
        Task<HttpResponseMessage> task = httpClient.GetAsync(LatestRepoURL.Formatted(owner, repoName));
#pragma warning disable CS4014
        task.ContinueWith(_ => isFinished = true);
#pragma warning restore CS4014
        int retries = 0;
        while (!isFinished && retries < 200)
        {
            await Task.Delay(100);
            retries++;
        }

        if (isFinished) callback(task.Result.Deserialize<Release>());
    }

    public Progress<float> DownloadUpdate(string url, string filename, Action<FileInfo> callback)
    {
        log.Trace($"Beginning Download of file: {url} to {filename}", "ModUpdater");
        Progress<float> progress = new();
        FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write);
        SyncTaskWaiter<bool> taskWaiter = new(downloadClient.DownloadAsync(url, fileStream, progress));
        Async.WaitUntil(() => taskWaiter.Finished, b => b, _ => callback(new FileInfo(filename)), delay: 1, maxRetries: 120);
        return progress;
    }
}