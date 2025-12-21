export interface OperationCC {
  id: number;
  date: string;
  description: string;
  montant: number;
  categorie: string;
  commentaire: string;
  banque: string;
  dateImport: string;
  isModified?: boolean; // Pour l'affichage de l'icône disquette
}