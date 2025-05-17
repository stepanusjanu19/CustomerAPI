using AutoMapper;
using Domain.Model;
using Application.DTO;
using Application.Request;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateCustomerRequest, Customer>();
        CreateMap<UpdateCustomerRequest, Customer>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
                srcMember != null && 
                (!(srcMember is string) || !string.IsNullOrWhiteSpace((string)srcMember))
            ));
        CreateMap<Customer, CustomerDto>();
    }
}
