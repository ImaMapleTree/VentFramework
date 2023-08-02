
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VentLib.Logging;
using VentLib.Utilities.Extensions;
using VentLib.Version.Git;
using VentLib.Version.Updater.Github;

namespace VentLib.Version.Updater;

public class ModUpdater
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(ModUpdater));
    private static Regex _regex = new("/github\\.com\\/(.*)\\/(.*)\\.git");
    private static DirectoryInfo _backupsDirectory = new("Backups");
    
    private GitVersion currentVersion;
    private string repositoryOwner;
    private string repositoryName;
    private GithubApi githubApi = null!;

    public Release? Latest;
    public bool HasUpdate;

    private List<(bool onlyTriggerUpdate, Action<Release>)> releaseCallbacks = new();
    
    private ModUpdater(GitVersion version)
    {
        if (!_backupsDirectory.Exists) _backupsDirectory.Create();
        currentVersion = version;
        if (version.RepositoryUrl == null) throw new ConstraintException("Unable to utilize ModUpdater. Local repository does not have a proper origin set.");
        var groups = _regex.Match(version.RepositoryUrl!).Groups;
        repositoryOwner = groups[1].Value;
        repositoryName = groups[2].Value;
    }
    
    public static ModUpdater Default()
    {
        if (VersionControl.Instance.Version is not GitVersion gitVersion) throw new ConstraintException("ModUpdate.Default() cannot be used by non Git-Version projects!");
        return new ModUpdater(gitVersion);
    }

    public static ModUpdater ForVersion(GitVersion version, string repositoryUrl)
    {
        version = version.Clone();
        version.RepositoryUrl = repositoryUrl;
        return new ModUpdater(version);
    }
    
    public void EstablishConnection(string? githubToken = null)
    {
        githubApi = new GithubApi(githubToken);
        githubApi.GetLatestRelease(repositoryOwner, repositoryName, ReleaseCallback);
    }

    public void RegisterReleaseCallback(Action<Release> callback, bool onlyTriggerOnUpdate)
    {
        releaseCallbacks.Add((onlyTriggerOnUpdate, callback));
    }

    public Progress<float>? Update(System.Reflection.Assembly? assemblyToUpdate = null, Action<Exception>? errorCallback = null)
    {
        if (Latest == null)
        {
            log.Warn("Update Metadata is not present. Make sure to call & wait-for ModUpdater.EstablishConnection()");
            return null;
        }
        
        assemblyToUpdate ??= System.Reflection.Assembly.GetCallingAssembly();
        string assemblyFileName = assemblyToUpdate.GetName().Name!;
        
        UploadedAsset? dllAsset = Latest!.GetDLLAsset($"{assemblyFileName}.dll");
        if (dllAsset != null) return Update(assemblyToUpdate, dllAsset, errorCallback);
        
        log.Warn("Cannot download update! No update dll found attached to release!");
        return null;
    }

    public Progress<float> Update(System.Reflection.Assembly assembly, UploadedAsset asset, Action<Exception>? errorCallback = null)
    {
        return githubApi.DownloadUpdate(asset.Url, asset.Name, file => ProcessUpdate(assembly, file, errorCallback));
    }

    private void ReleaseCallback(Release release)
    {
        Latest = release;
        log.High($"Retrieved Release Metadata... ({release.TagName} (Created At: {release.CreatedAt})", "ReleaseChecker");
        DateTime localCommitTime = DateTime.Parse(currentVersion.CommitDate!).ToUniversalTime();
        HasUpdate = IsReleaseNewer(release.CreatedAt, localCommitTime);
        if (HasUpdate) log.High($"New Release Found! {release.Name} ({release.TagName})", "ReleaseChecker");
        releaseCallbacks.Where(c => !c.onlyTriggerUpdate || HasUpdate).ForEach(c => c.Item2(release));
    }

    private void ProcessUpdate(System.Reflection.Assembly assembly, FileInfo downloadedFile, Action<Exception>? errorCallback = null)
    {
        string assemblyLocation = assembly.Location;

        FileInfo rootFile = new(assemblyLocation);
        if (!rootFile.Exists)
        {
            log.Exception("Unable to complete mod update. Could not get location of updated assembly!");
            return;
        }

        int uniqueNumber = 0;
        FileInfo newLocation = _backupsDirectory.GetFile(rootFile.Name);
        while (true)
        {
            try
            {
                rootFile.MoveTo(newLocation.FullName, true);
                break;
            }
            catch (UnauthorizedAccessException)
            {
                newLocation = _backupsDirectory.GetFile(rootFile.Name + $"-{uniqueNumber++}");
            }
            catch (Exception ex)
            {
                log.Exception($"Critical Error! Could not move file {assemblyLocation} to {newLocation}. Aborting and deleting downloaded file", ex);
                downloadedFile.Delete();
                errorCallback?.Invoke(ex);
                return;
            }
        }

        downloadedFile.MoveTo(assemblyLocation);
        log.High($"Finished Updating: {rootFile}", "ModUpdater");
    }

    private static bool IsReleaseNewer(DateTime releaseTime, DateTime localCommitTime)
    {
        if (releaseTime.Year > localCommitTime.Year) return true;
        if (releaseTime.Year < localCommitTime.Year) return false;
        
        if (releaseTime.Month > localCommitTime.Month) return true;
        if (releaseTime.Month < localCommitTime.Month) return false;
        
        if (releaseTime.Day > localCommitTime.Day) return true;
        if (releaseTime.Day < localCommitTime.Day) return false;
        
        if (releaseTime.Hour > localCommitTime.Hour) return true;
        if (releaseTime.Hour < localCommitTime.Hour) return false;
        
        return releaseTime.Minute > localCommitTime.Minute;
    }
}
