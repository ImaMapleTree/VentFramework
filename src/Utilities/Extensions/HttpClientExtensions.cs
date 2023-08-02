using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VentLib.Utilities.Extensions;

public static class HttpClientExtensions
{
    /// <summary>
    /// Code via Bruno Zell and Ian Kemp on stackoverflow. https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient
    /// </summary>
    public static async Task<bool> DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
    {
        // Get the http headers first to examine the content length
        using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        long? contentLength = response.Content.Headers.ContentLength;

        await using var download = await response.Content.ReadAsStreamAsync(cancellationToken);
        // Ignore progress reporting when no progress reporter was 
        // passed or when the content length is unknown
        if (progress == null || !contentLength.HasValue) {
            await download.CopyToAsync(destination, cancellationToken);
            return false;
        }

        // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
        Progress<long> relativeProgress = new(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
        // Use extension method to report progress while downloading
        await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
        progress.Report(1);
        destination.Close();
        return true;
    }
}