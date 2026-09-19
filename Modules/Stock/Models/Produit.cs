using GestionCommerciale.Shared.Models;

namespace GestionCommerciale.Modules.Stock.Models;

public class Produit : BaseEntity
{
    public string Reference { get; set; } = string.Empty;
    /// <summary>EAN / UPC / code interne, optionnel.</summary>
    public string? CodeBarre { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Unite { get; set; } = "U";
    /// <summary>Prix avant remise (PPV). PrixAchatHT = Ppv × (1 − Remise/100).</summary>
    public decimal Ppv { get; set; }
    /// <summary>Remise en % (0–100). PrixAchatHT = Ppv × (1 − Remise/100).</summary>
    public decimal Remise { get; set; }
    public decimal PrixAchatHT { get; set; }
    public decimal PrixVenteHT { get; set; }
    public decimal TauxTVA { get; set; }
    public decimal StockActuel { get; set; }
    public decimal StockMinimum { get; set; }
    public int? CategorieId { get; set; }
    public Categorie? Categorie { get; set; }
    public bool Actif { get; set; } = true;

    /// <summary>Compressed product photo (JPEG), optional.</summary>
    public byte[]? ImageData { get; set; }

    public static decimal ComputePrixAchatHt(decimal ppv, decimal remisePct)
    {
        var clamped = Math.Clamp(remisePct, 0m, 100m);
        return ppv * (1m - clamped / 100m);
    }

    public static decimal ComputePpvFromPrixAchatHt(decimal prixAchatHt, decimal remisePct)
    {
        var clamped = Math.Clamp(remisePct, 0m, 100m);
        if (clamped >= 100m)
            return 0m;
        return prixAchatHt / (1m - clamped / 100m);
    }
}
