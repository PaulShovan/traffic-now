using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficNow.Api.Response
{
    public class GenericResponse<T> where T : class
    {
        T data;      
        public GenericResponse(T responseData)
        {
            data = responseData;
        }
    }
}