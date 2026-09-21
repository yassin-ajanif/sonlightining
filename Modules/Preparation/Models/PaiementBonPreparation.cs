using GestionCommerciale.Modules.Facturation.Models;
using GestionCommerciale.Shared.Models;

namespace GestionCommerciale.Modules.Preparation.Models;

public class PaiementBonPreparation : BaseEntity
{
    public int BonPreparationId { get; set; }
    public BonPreparation? BonPreparation { get; set; }
    public decimal Montant { get; set; }
    public DateTime Date { get; set; }
    public ModePaiement Mode { get; set; }
    public string Reference { get; set; } = string.Empty;
    /// <summary>True when money is actually received/cleared. False for pending cheque/effet.</summary>
    public bool EstEncaisse { get; set; } = true;
}
