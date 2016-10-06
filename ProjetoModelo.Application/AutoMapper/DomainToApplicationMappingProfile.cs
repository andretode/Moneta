using AutoMapper;
using Moneta.Application.Validation;
using Moneta.Domain.ValueObjects;

namespace Moneta.Application.AutoMapper
{
    public class DomainToApplicationMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToApplicationMapping"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<ValidationError, ValidationAppError>();
            Mapper.CreateMap<ValidationResult, ValidationAppResult>();
        }
    }
}
