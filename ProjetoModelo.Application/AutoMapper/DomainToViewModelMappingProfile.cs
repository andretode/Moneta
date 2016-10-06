using AutoMapper;
using Moneta.Application.ViewModels;
using Moneta.Domain.Entities;

namespace Moneta.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Conta, ContaViewModel>();
        }
    }
}