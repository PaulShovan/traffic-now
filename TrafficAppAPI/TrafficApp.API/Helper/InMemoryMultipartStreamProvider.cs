using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace TrafficApp.API.Helper
{
    public class InMemoryMultipartStreamProvider : MultipartStreamProvider
    {
        private NameValueCollection formData = new NameValueCollection();
        private List<HttpContent> fileContents = new List<HttpContent>();
        private Collection<bool> isFormData = new Collection<bool>();

        public NameValueCollection FormData
        {
            get { return this.formData; }
        }

        public List<HttpContent> Files
        {
            get { return this.fileContents; }
        }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
            if (contentDisposition != null)
            {
                this.isFormData.Add(string.IsNullOrEmpty(contentDisposition.FileName));

                return new MemoryStream();
            }
            throw new InvalidOperationException(string.Format("Did not find required '{0}' header field in MIME multipart body part.", "Content-Disposition"));
        }

        public override async Task ExecutePostProcessingAsync()
        {
            for (int index = 0; index < Contents.Count; index++)
            {
                if (this.isFormData[index])
                {
                    HttpContent formContent = Contents[index];

                    ContentDispositionHeaderValue contentDisposition = formContent.Headers.ContentDisposition;
                    string formFieldName = UnquoteToken(contentDisposition.Name) ?? string.Empty;

                    string formFieldValue = await formContent.ReadAsStringAsync();
                    this.FormData.Add(formFieldName, formFieldValue);
                }
                else
                {
                    this.fileContents.Add(this.Contents[index]);
                }
            }
        }

        private static string UnquoteToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }
    }
}