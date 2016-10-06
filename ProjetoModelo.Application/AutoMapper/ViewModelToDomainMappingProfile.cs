using AutoMapper;
using Moneta.Application.ViewModels;
using Moneta.Domain.Entities;

namespace Moneta.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<ContaViewModel, Conta>();
        }
    }
}