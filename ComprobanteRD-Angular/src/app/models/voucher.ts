import { CustomerDTO } from './customer';

export interface VoucherDTO {
  id: number;
  sentAt: Date;
  amount: number;
  bankReferenceNumber: string;
  status: string;
  imageURL: string;
  customerPhone: string;
  customerName: string;
}

export interface DashboardDTO {
  id: number;
  sentAt: Date;
  amount: number;
  status: string;
  customerName: string;
}

export interface RejectVoucherDTO {
  Reason: string;
}
