using GestionCommerciale.Shared.Models;

namespace GestionCommerciale.Modules.Facturation.Models;

public class Paiement : BaseEntity
{
    public int FactureId { get; set; }
    public Facture? Facture { get; set; }
    public decimal Montant { get; set; }
    public DateTime Date { get; set; }
    public ModePaiement Mode { get; set; }
    public string Reference { get; set; } = string.Empty;
    /// <summary>True when money is actually received/cleared. False for pending cheque/effet.</summary>
    public bool EstEncaisse { get; set; } = true;
}
