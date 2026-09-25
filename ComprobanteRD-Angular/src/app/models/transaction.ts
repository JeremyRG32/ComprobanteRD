export interface TransactionDTO {
  voucherId: number;
  sentAt: Date;
  bankReferenceNumber: string;
  customerName: string;
  amount: number;
  status: string;
}

export interface TransactionFilters {
  name: string;
  date: Date;
  status: string;
}
