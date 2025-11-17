using System.IO;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.Interfaces.Infrastructure
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType = "application/octet-stream");
        Task<bool> DeleteFileAsync(string fileName);
        string GetFileUrl(string fileName);
    }
}















