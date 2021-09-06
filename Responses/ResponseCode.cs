using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Responses
{
    public enum ResponseCode
    {
        NO_ENCRYPTION_KEY,
        LOGIN_SUCCESS,
        INVALID_CREDENTIALS,
        ENCRYPTION_ERROR,
    }
}
