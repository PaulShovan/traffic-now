using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Service.Interface
{
    public interface IMailService
    {
        bool SendMail(string fromAddress, string toAddress,string subject, string body);
    }
}
