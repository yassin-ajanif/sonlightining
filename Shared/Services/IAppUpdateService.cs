namespace GestionCommerciale.Shared.Services;

public interface IAppUpdateService
{
    string? CurrentVersion { get; }
    bool IsInstalled { get; }
    Task CheckForUpdatesOnStartupAsync(CancellationToken cancellationToken = default);
}
