using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Response
{
    public class GenResponseModel<T>
    {
        public string ErrorMesssage { get; set; }
        public bool IsSuccess { get; set; }
        public List<T>? Data { get; set; } = new List<T>();
    }
}
