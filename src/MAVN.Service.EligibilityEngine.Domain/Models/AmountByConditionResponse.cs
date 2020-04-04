using System;

namespace MAVN.Service.EligibilityEngine.Domain.Models
{
    public class AmountByConditionResponse : AmountConvertResponse
    {
        public Guid ConditionId { get; set; }
    }
}
