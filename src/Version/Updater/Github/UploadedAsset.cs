using System;
using System.Text.Json.Serialization;

namespace VentLib.Version.Updater.Github;

public class UploadedAsset
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
    
    [JsonPropertyName("id")]
    public ulong ID { get; set; }
    
    [JsonPropertyName("node_id")]
    public string NodeID { get; set; } = null!;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("label")]
    public string Label { get; set; } = null!;
    
    [JsonPropertyName("uploader")]
    public Author Uploader { get; set; } = null!;
    
    [JsonPropertyName("content_type")]
    public string ContentType { get; set; } = null!;
    
    [JsonPropertyName("state")]
    public string State { get; set; } = null!;
    
    [JsonPropertyName("size")]
    public ulong Size { get; set; }
    
    [JsonPropertyName("download_count")]
    public ulong DownloadCount { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; } = null!;
}