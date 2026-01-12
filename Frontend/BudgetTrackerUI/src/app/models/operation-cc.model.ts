export interface CcOperation {
  id: number;
  date: string;
  description: string;
  montant: number;
  categorie: string;
  macroCategory: string;
  Comment: string;
  banque: string;
  dateImport: string;
  isModified?: boolean; // Pour l'affichage de l'icône disquette
}