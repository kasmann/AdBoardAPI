using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdBoardAPI.ImageFileMgr
{
    interface IImageFileManager
    {
        public string GenerateURL(string adId, string imageName);
        public Task UploadImageAsync(IFormFile image, string path);
    }
}
