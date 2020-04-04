using AutoMapper;
using MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using MAVN.Service.EligibilityEngine.Domain.Models;

namespace MAVN.Service.EligibilityEngine.Profiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Client.Enums.ConversionSource, Domain.Models.ConversionSource>(MemberList.Destination);
        }
    }
}
