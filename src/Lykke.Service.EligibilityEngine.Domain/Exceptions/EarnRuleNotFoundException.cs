using System;

namespace Lykke.Service.EligibilityEngine.Domain.Exceptions
{
    public class EarnRuleNotFoundException : Exception
    {
        public EarnRuleNotFoundException(string message): base(message)
        {
        }
    }
}
