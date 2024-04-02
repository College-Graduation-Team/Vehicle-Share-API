using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Response
{
    public class ResponseForOneModel<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorMesssage { get; set; }
        public T Data { get; set; }    
    }
}
