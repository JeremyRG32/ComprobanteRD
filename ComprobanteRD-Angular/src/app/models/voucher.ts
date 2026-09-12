import { CustomerDTO } from './customer';

export interface VoucherDTO {
  id: number;
  sentAt: Date;
  amount: number;
  bankReferenceNumber: string;
  status: string;
  imageURL: string;
  customer: CustomerDTO;
}

export interface RejectVoucherDTO {
  Reason: string;
}
