export interface CcOperation {
  id: number;
  date: string;
  amount: number;
  label: string;
  categorie: string;
  macroCategory: string;
  isSuggested?: boolean; // Pour l'affichage de l'icône disquette
  comment: string;
  banque: string;
}