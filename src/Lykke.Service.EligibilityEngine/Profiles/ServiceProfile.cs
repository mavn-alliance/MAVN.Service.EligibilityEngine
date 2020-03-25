using AutoMapper;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using Lykke.Service.EligibilityEngine.Domain.Models;

namespace Lykke.Service.EligibilityEngine.Profiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Client.Enums.ConversionSource, Domain.Models.ConversionSource>(MemberList.Destination);
        }
    }
}
