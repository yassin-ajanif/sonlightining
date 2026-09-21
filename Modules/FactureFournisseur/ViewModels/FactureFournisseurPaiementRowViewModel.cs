using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionCommerciale.Modules.Facturation.Models;
using GestionCommerciale.Modules.FactureFournisseur.Models;

namespace GestionCommerciale.Modules.FactureFournisseur.ViewModels;

public partial class FactureFournisseurPaiementRowViewModel : ObservableObject
{
    private readonly FactureFournisseurEditViewModel _owner;

    private decimal _snapshotMontant;
    private DateTimeOffset _snapshotDate;
    private ModePaiement _snapshotMode;
    private string _snapshotReference = string.Empty;
    private bool _snapshotEstEncaisse;

    public int Id { get; }

    public bool FactureFournisseurModifiable => _owner.CanEditDraft;

    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private decimal _montant;
    [ObservableProperty] private DateTimeOffset _date;
    [ObservableProperty] private ModePaiement _mode;
    [ObservableProperty] private string _reference = string.Empty;
    [ObservableProperty] private bool _estEncaisse = true;

    public Array ModesPaiement => _owner.ModesPaiement;

    public string EstEncaisseLabel => EstEncaisse
        ? _owner.LblEstEncaisse
        : _owner.LblEstEncaissePending;

    public FactureFournisseurPaiementRowViewModel(FactureFournisseurEditViewModel owner, PaiementFournisseur p)
    {
        _owner = owner;
        Id = p.Id;
        Montant = p.Montant;
        Date = new DateTimeOffset(p.Date);
        Mode = p.Mode;
        Reference = p.Reference;
        EstEncaisse = p.EstEncaisse;
    }

    private bool CanSaveRow() => IsEditing && Montant > 0;

    private bool CanStartEdit() => FactureFournisseurModifiable && !IsEditing;

    private bool CanCancelEdit() => IsEditing;

    partial void OnIsEditingChanged(bool value)
    {
        StartEditCommand.NotifyCanExecuteChanged();
        CancelEditCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnMontantChanged(decimal value) => SaveCommand.NotifyCanExecuteChanged();

    partial void OnEstEncaisseChanged(bool value) => OnPropertyChanged(nameof(EstEncaisseLabel));

    [RelayCommand(CanExecute = nameof(CanStartEdit))]
    private void StartEdit()
    {
        _snapshotMontant = Montant;
        _snapshotDate = Date;
        _snapshotMode = Mode;
        _snapshotReference = Reference;
        _snapshotEstEncaisse = EstEncaisse;
        IsEditing = true;
    }

    [RelayCommand(CanExecute = nameof(CanCancelEdit))]
    private void CancelEdit()
    {
        Montant = _snapshotMontant;
        Date = _snapshotDate;
        Mode = _snapshotMode;
        Reference = _snapshotReference;
        EstEncaisse = _snapshotEstEncaisse;
        IsEditing = false;
    }

    [RelayCommand(CanExecute = nameof(CanSaveRow))]
    private async Task SaveAsync(CancellationToken cancellationToken) =>
        await _owner.CommitPaiementRowAsync(this, cancellationToken);

    [RelayCommand]
    private async Task DeleteAsync(CancellationToken cancellationToken) =>
        await _owner.DeletePaiementRowAsync(this, cancellationToken);
}
