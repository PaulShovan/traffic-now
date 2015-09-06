using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficAppAPI.Common.Utility
{
    public class JsonResult<T>
    {
        public JsonResult()
        {
        }
        public JsonResult(int count, T data)
        {
            Data = data;
            Count = count;
        }
        public T Data { get; set; }
        public int Count { get; set; }
    }
}
