export interface SavingAccount {
    id?: number;
    name: string;
    owner: string;
    bankName?: string;
    isActive: boolean;
    savingStatements?: SavingStatement[];
}

export interface SavingStatement {
    id?: number;
    accountId: number;
    date: string;
    amount: number;
    note?: string;
}