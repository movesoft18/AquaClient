using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquaClient.Classes.Responses
{
    public class ResponseBase<T>
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public T? Data {  get; set; }
    }
}
