using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficNow.Api.Response
{
    public class ResponseMessage
    {
        public string message;
        public ResponseMessage(string msg)
        {
            message = msg;
        }
    }
}