export interface AccountStatus {
  /** Nom de la banque ou du compte (ex: "Fortuneo", "Livret A") */
  accountName: string;

  /** Type de compte (ex: "Compte Courant", "Épargne") */
  type: string;

  /** * Date de la dernière opération trouvée. 
   * On utilise string pour recevoir le format ISO de l'API 
   */
  lastEntryDate: string | null;

  /** Vrai si une mise à jour est nécessaire (ex: pas de données en décembre 2025) */
  actionRequired: boolean;

  /** Message d'explication (ex: "Mise à jour nécessaire", "À jour") */
  message: string;
}