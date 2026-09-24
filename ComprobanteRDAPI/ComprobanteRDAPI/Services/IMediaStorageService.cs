namespace ComprobanteRDAPI.Services
{
    public interface IMediaStorageService
    {
        Task<string> DownloadAndSaveMediaAsync(string mediaId);
        Task<string> SaveReceiptPdfAsync(Stream pdfStream, string receiptNumber);
    }
}
