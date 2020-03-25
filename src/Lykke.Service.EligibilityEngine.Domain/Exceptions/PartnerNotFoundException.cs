using System;

namespace Lykke.Service.EligibilityEngine.Domain.Exceptions
{
    public class PartnerNotFoundException: Exception
    {
        public PartnerNotFoundException(string message): base(message)
        {
        }
    }
}
