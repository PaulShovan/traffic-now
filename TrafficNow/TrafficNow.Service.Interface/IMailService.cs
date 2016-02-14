using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Service.Interface
{
    public interface IMailService
    {
        void SendMail(string fromAddress, string toAddress);
    }
}
