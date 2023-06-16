using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace VentLib.Version.Updater.Github;

public class Release
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
    
    [JsonPropertyName("assets_url")]
    public string AssetsUrl { get; set; } = null!;
    
    [JsonPropertyName("upload_url")]
    public string UploadUrl { get; set; } = null!;
    
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = null!;
    
    [JsonPropertyName("id")]
    public ulong ID { get; set; }
    
    [JsonPropertyName("author")]
    public Author Author { get; set; } = null!;
    
    [JsonPropertyName("node_id")]
    public string NodeID { get; set; } = null!;
    
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = null!;
    
    [JsonPropertyName("target_commitish")]
    public string TargetCommitish { get; set; } = null!;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("draft")]
    public bool IsDraft { get; set; }
    
    [JsonPropertyName("prerelease")]
    public bool IsPreRelease { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
    
    [JsonPropertyName("assets")]
    public List<UploadedAsset> Assets { get; set; } = null!;

    [JsonPropertyName("tarball_url")]
    public string TarballUrl { get; set; } = null!;

    [JsonPropertyName("zipball_url")]
    public string ZipballUrl { get; set; } = null!;

    [JsonPropertyName("body")]
    public string Body { get; set; } = null!;

    public bool ContainsDLL(string dllName) => Assets.Any(a => a.Name.Contains(dllName));

    public UploadedAsset? GetDLLAsset(string dllName) => Assets.FirstOrDefault(a => a.Name.Contains(dllName));
}