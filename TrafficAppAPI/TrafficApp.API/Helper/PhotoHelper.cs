using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrafficApp.API.Model;

namespace TrafficApp.API.Helper
{
    public class PhotoHelper
    {
        public string workingFolder { get; set; }
        public PhotoHelper(string workingFolder)
        {
            this.workingFolder = workingFolder;
        }
        public string GetTargetDirectory(string userId)
        {
            try
            {
                string directoryPath = new StringBuilder(this.workingFolder).Append("/").Append(userId).ToString();
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                return directoryPath;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        public async Task<string> Add(HttpRequestMessage request, string userId)
        {
            //var directoryPath = CheckTargetDirectory(userId);
            var directoryPath = this.workingFolder;
            var provider = new PhotoMultipartFormDataStreamProvider(directoryPath);

            await request.Content.ReadAsMultipartAsync(provider);

            var photoUrl = "";
            foreach (var file in provider.FileData)
            {
                var fileName = Guid.NewGuid().ToString();
                var fileInfo = new FileInfo(fileName);
                photoUrl = directoryPath+fileInfo.Name;
            }

            return photoUrl;
        }
    }
}