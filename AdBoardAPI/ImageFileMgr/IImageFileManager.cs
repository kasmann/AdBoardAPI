using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AdBoardAPI.ImageFileMgr
{
    public interface IImageFileManager
    {
        public string GenerateURL(string adId, string imageName);
        public Task UploadImageAsync(IFormFile image, string path);
    }
}
