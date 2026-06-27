namespace GestionCommerciale.Shared.Configuration;

public static class VelopackConfiguration
{
    public const string PackId = "Sonlighting.GestionCommerciale";
    public const string MainExe = "GestionCommerciale.exe";
    public const string GitHubRepoUrl = "https://github.com/yassin-ajanif/sonlightining";

#if DEBUG
    /// <summary>Set to true to preview the update banner when running from Visual Studio (not a Velopack install).</summary>
    public const bool SimulateUpdateBannerForUiTest = false;
    public const string SimulatedUpdateVersion = "9.9.9";
#endif
}
