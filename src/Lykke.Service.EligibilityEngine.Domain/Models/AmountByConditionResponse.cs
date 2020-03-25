using System;

namespace Lykke.Service.EligibilityEngine.Domain.Models
{
    public class AmountByConditionResponse : AmountConvertResponse
    {
        public Guid ConditionId { get; set; }
    }
}
