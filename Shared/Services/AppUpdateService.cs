using GestionCommerciale.Shared.Configuration;
using Velopack;
using Velopack.Exceptions;
using Velopack.Sources;

namespace GestionCommerciale.Shared.Services;

public sealed class AppUpdateService : IAppUpdateService
{
    private readonly IDialogService _dialogService;
    private UpdateManager? _updateManager;
    private UpdateInfo? _pendingUpdateInfo;

    public AppUpdateService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public string? CurrentVersion => GetUpdateManager().CurrentVersion?.ToString();

    public bool IsInstalled => GetUpdateManager().IsInstalled;

    public bool IsUpdateAvailable { get; private set; }

    public string? AvailableVersion { get; private set; }

    public bool IsUpdateDownloaded { get; private set; }

    public bool IsCheckingForUpdates { get; private set; }

    public event EventHandler? UpdateStateChanged;

    public async Task CheckForUpdatesAsync(CancellationToken cancellationToken = default)
    {
        if (IsCheckingForUpdates)
            return;

        try
        {
            IsCheckingForUpdates = true;
            NotifyStateChanged();

            var mgr = GetUpdateManager();
            if (!mgr.IsInstalled)
            {
#if DEBUG
                if (VelopackConfiguration.SimulateUpdateBannerForUiTest)
                    SetUpdateAvailable(VelopackConfiguration.SimulatedUpdateVersion, downloaded: false);
                else
#endif
                    ClearUpdateState();
                return;
            }

            var pending = mgr.UpdatePendingRestart;
            if (pending is not null)
            {
                SetUpdateAvailable(pending.Version.ToString(), downloaded: true);
                return;
            }

            var updateInfo = await mgr.CheckForUpdatesAsync();
            if (updateInfo is null)
            {
                ClearUpdateState();
                return;
            }

            _pendingUpdateInfo = updateInfo;
            SetUpdateAvailable(updateInfo.TargetFullRelease.Version.ToString(), downloaded: false);
        }
        catch (NotInstalledException)
        {
#if DEBUG
            if (VelopackConfiguration.SimulateUpdateBannerForUiTest)
                SetUpdateAvailable(VelopackConfiguration.SimulatedUpdateVersion, downloaded: false);
            else
#endif
                ClearUpdateState();
        }
        catch (Exception)
        {
            // Don't surface update check failures in the UI.
        }
        finally
        {
            IsCheckingForUpdates = false;
            NotifyStateChanged();
        }
    }

    public async Task DownloadAndApplyUpdateAsync(CancellationToken cancellationToken = default)
    {
#if DEBUG
        if (VelopackConfiguration.SimulateUpdateBannerForUiTest && !GetUpdateManager().IsInstalled)
        {
            await _dialogService.ShowInfoAsync(
                "Mise à jour (simulation)",
                "Mode test : la bannière est simulée. Installez via Velopack pour tester les vraies mises à jour.",
                cancellationToken);
            return;
        }
#endif

        try
        {
            var mgr = GetUpdateManager();
            if (!mgr.IsInstalled)
                return;

            VelopackAsset? asset = mgr.UpdatePendingRestart;
            if (asset is null && _pendingUpdateInfo is not null)
            {
                await mgr.DownloadUpdatesAsync(_pendingUpdateInfo, progress: null, cancellationToken);
                asset = _pendingUpdateInfo;
                IsUpdateDownloaded = true;
                NotifyStateChanged();
            }

            if (asset is null)
                return;

            var restart = await _dialogService.ConfirmAsync(
                "Mise à jour disponible",
                $"La version {asset.Version} est prête. Redémarrer maintenant pour l'installer ?",
                cancellationToken);

            if (restart)
                mgr.ApplyUpdatesAndRestart(asset);
        }
        catch (NotInstalledException)
        {
            // Expected when running from bin/Debug without a Velopack install.
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(
                "Mise à jour",
                $"Impossible d'installer la mise à jour : {ex.Message}",
                cancellationToken);
        }
    }

    private void SetUpdateAvailable(string version, bool downloaded)
    {
        IsUpdateAvailable = true;
        AvailableVersion = version;
        IsUpdateDownloaded = downloaded;
        NotifyStateChanged();
    }

    private void ClearUpdateState()
    {
        IsUpdateAvailable = false;
        AvailableVersion = null;
        IsUpdateDownloaded = false;
        _pendingUpdateInfo = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => UpdateStateChanged?.Invoke(this, EventArgs.Empty);

    private UpdateManager GetUpdateManager()
    {
        return _updateManager ??= new UpdateManager(
            new GithubSource(VelopackConfiguration.GitHubRepoUrl, accessToken: null, prerelease: false));
    }
}
