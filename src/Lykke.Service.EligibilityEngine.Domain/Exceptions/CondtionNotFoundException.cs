using System;

namespace Lykke.Service.EligibilityEngine.Domain.Exceptions
{
    public class ConditionNotFoundException : Exception
   {
       public ConditionNotFoundException(string message) : base(message)
       {
       }
   }
}
