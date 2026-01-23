export interface FichierTraite {
  nomDuFichier: string;
  isSuccessful: boolean;
  msgErreur: string;
  nombreOperationsLus: number;
  nombreOperationsAjoutees: number;
  parser: string | null;
  dateMin: string | null;
  dateMax: string | null;
  tempsDeTraitementMs: number;
  dateImport?: Date;        // Optionnel pour le frontend
}