export interface CategoryRule {
  id: number;
  isUsed: boolean;
  pattern: string;
  minAmount?: number;
  maxAmount?: number;
  minDate?: string;
  maxDate?: string;
  commentaire: string;
  category: string;
}

export interface Category {
  id: number;
  name: string;
}