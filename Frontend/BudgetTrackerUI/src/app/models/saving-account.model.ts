export interface SavingAccount {
    id?: number;
    name: string;
    owner: string;
    bankName?: string;
    isActive: boolean;
    statements?: SavingStatement[];
}

export interface SavingStatement {
    id?: number;
    accountId: number;
    date: string;
    amount: number;
    note?: string;
}