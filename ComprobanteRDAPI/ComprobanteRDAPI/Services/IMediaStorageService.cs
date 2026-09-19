namespace ComprobanteRDAPI.Services
{
    public interface IMediaStorageService
    {
        Task<string> DownloadAndSaveMediaAsync(string mediaId);
    }
}
