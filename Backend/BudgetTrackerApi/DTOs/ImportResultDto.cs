public record ImportResultDto(
    string NomDuFichier,
    bool IsSuccessful,
    string MsgErreur,
    int NombreOperationsLus,
    int NombreOperationsAjoutees,
    string? Parser,
    string? DateMin,
    string? DateMax,
    double TempsDeTraitementMs
);