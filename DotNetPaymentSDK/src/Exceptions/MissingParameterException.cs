using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetPaymentSDK.src.Exceptions
{
    public class MissingParameterException(string message) : Exception(message)
    {
    }
}