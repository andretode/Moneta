using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Application.Validation
{
    public class ValidationAppError
    {
        public string Message { get; set; }
        public ValidationAppError(string message)
        {
            this.Message = message;
        }
    }
}
