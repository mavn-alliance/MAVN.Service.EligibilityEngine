using System;

namespace MAVN.Service.EligibilityEngine.Domain.Exceptions
{
    public class BurnRuleNotFoundException : Exception
    {
        public BurnRuleNotFoundException(string message): base(message)
        {
        }
    }
}
