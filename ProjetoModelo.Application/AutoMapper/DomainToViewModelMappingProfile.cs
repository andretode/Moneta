using AutoMapper;
using Moneta.Application.ViewModels;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;

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
            Mapper.CreateMap<Categoria, CategoriaViewModel>();
            Mapper.CreateMap<Lancamento, LancamentoViewModel>()
                .ForMember(d => d.DescricaoMaisNumeroParcela, o => o.MapFrom(s => s.DescricaoMaisNumeroParcela()));
            Mapper.CreateMap<LancamentoParcelado, LancamentoParceladoViewModel>()
                .ForMember(d => d.TipoDeRepeticao, o => o.MapFrom(s => s.TipoDeRepeticao()));
            Mapper.CreateMap<AgregadoLancamentosDoMes, LancamentosDoMesViewModel>();
        }
    }
}