using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukCuk.Core.Exceptions
{
    public class EmployeeValidateException : Exception
    {
        string? MsgErrorValidate = null;
        public EmployeeValidateException(string msg)
        {
            this.MsgErrorValidate = msg;
        }

        public override string Message
        {
            get
            {
                return MsgErrorValidate;
            }
        }
    }
}
