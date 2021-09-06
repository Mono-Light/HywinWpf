using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Responses
{
    public class LoginResponse : Response
    {
        public string message { get; set; }
        public string data { get; set; }
        public string details { get; set; }
    }
}
