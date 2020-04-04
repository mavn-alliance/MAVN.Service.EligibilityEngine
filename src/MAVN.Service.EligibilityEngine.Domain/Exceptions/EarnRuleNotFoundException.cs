using System;

namespace MAVN.Service.EligibilityEngine.Domain.Exceptions
{
    public class EarnRuleNotFoundException : Exception
    {
        public EarnRuleNotFoundException(string message): base(message)
        {
        }
    }
}
