using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.Models
{
    public class APIException : Exception
    {
        private int _code;

        public int code { get; set; }

        public APIException() : base() { }

        public APIException(string message) : base(message) { }
    }
}
