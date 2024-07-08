using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DotNetPaymentSDK.src.Exceptions
{
    public class ParseException(string message) : Exception(message)
    {
    }
}