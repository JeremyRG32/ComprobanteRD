export interface ReceiptDto {
  id: string;
  receiptNumber: string;
  issuedAt: string;
  pdfUrl?: string;
  amount: number;
  bankReference?: string;
  customerName: string;
  customerPhone: string;
  companyName: string;
  companyRnc: string;
  companyPhone?: string;
  companyAddress?: string;
}
