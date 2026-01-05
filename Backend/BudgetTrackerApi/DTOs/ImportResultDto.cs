namespace BudgetTrackerApp.DTOs
{
    public record ImportResultDto(
        string NomDuFichier,
        bool IsSuccessful,
        string MsgErreur,
        int NombreOperationsLus,
        int NombreOperationsAjoutees,
        string? Parser,
        DateTime? DateMin,
        DateTime? DateMax,
        double TempsDeTraitementMs
    );
}