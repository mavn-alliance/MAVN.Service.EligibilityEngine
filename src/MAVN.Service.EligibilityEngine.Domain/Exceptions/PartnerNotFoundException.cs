using System;

namespace MAVN.Service.EligibilityEngine.Domain.Exceptions
{
    public class PartnerNotFoundException: Exception
    {
        public PartnerNotFoundException(string message): base(message)
        {
        }
    }
}
