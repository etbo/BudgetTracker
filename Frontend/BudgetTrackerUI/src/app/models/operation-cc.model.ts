export interface CcOperation {
  id: number;
  date: string;
  amount: number;
  label: string;
  category: string;
  macroCategory: string;
  isSuggested?: boolean; // Pour l'affichage de l'icône disquette
  comment: string;
  bank: string;
}