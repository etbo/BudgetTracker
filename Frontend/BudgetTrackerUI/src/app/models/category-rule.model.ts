export interface CcCategoryRule {
  id: number;
  isUsed: boolean;
  pattern: string;
  minAmount?: number | null;
  maxAmount?: number | null;
  minDate?: string;
  maxDate?: string;
  Comment: string;
  category: string;
}

export interface CcCategory {
  id: number;
  name: string;
  type: string;
}