using GestionCommerciale.Shared.Configuration;
using Velopack;
using Velopack.Exceptions;
using Velopack.Sources;

namespace GestionCommerciale.Shared.Services;

public sealed class AppUpdateService : IAppUpdateService
{
    private readonly IDialogService _dialogService;
    private UpdateManager? _updateManager;

    public AppUpdateService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public string? CurrentVersion => GetUpdateManager().CurrentVersion?.ToString();

    public bool IsInstalled => GetUpdateManager().IsInstalled;

    public async Task CheckForUpdatesOnStartupAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var mgr = GetUpdateManager();
            if (!mgr.IsInstalled)
                return;

            var pending = mgr.UpdatePendingRestart;
            if (pending is not null)
            {
                var restart = await _dialogService.ConfirmAsync(
                    "Mise à jour disponible",
                    $"Une mise à jour (v{pending.Version}) a été téléchargée. Redémarrer maintenant pour l'installer ?",
                    cancellationToken);
                if (restart)
                    mgr.ApplyUpdatesAndRestart(pending);
                return;
            }

            var updateInfo = await mgr.CheckForUpdatesAsync();
            if (updateInfo is null)
                return;

            await mgr.DownloadUpdatesAsync(updateInfo, progress: null, cancellationToken);

            var restartNow = await _dialogService.ConfirmAsync(
                "Mise à jour disponible",
                $"La version {updateInfo.TargetFullRelease.Version} est disponible et a été téléchargée. Redémarrer maintenant pour l'installer ?",
                cancellationToken);

            if (restartNow)
                mgr.ApplyUpdatesAndRestart(updateInfo);
        }
        catch (NotInstalledException)
        {
            // Expected when running from bin/Debug without a Velopack install.
        }
        catch (Exception)
        {
            // Don't block startup on update failures.
        }
    }

    private UpdateManager GetUpdateManager()
    {
        return _updateManager ??= new UpdateManager(
            new GithubSource(VelopackConfiguration.GitHubRepoUrl, accessToken: null, prerelease: false));
    }
}
