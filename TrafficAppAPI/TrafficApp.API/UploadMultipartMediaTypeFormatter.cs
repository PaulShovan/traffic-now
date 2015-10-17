using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;

namespace TrafficApp.API
{
    public class UploadMultipartMediaTypeFormatter : MediaTypeFormatter
    {
        public UploadMultipartMediaTypeFormatter()
        {
            SupportedMediaTypes
        .Add(new MediaTypeHeaderValue("multipart/form-data"));
        }
        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }
    }
}