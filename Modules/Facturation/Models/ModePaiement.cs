namespace GestionCommerciale.Modules.Facturation.Models;

public enum ModePaiement
{
    Credit = 0,
    Cheque = 1,
    Especes = 2,
    TPE = 3,
    Virement = 4,
    Effet = 5
}

public static class ModePaiementDefaults
{
    /// <summary>Chèque / effet default to not yet collected; other modes are encashed immediately.</summary>
    public static bool DefaultEstEncaisse(ModePaiement mode) =>
        mode is not (ModePaiement.Cheque or ModePaiement.Effet);
}
