using AutoMapper;
using Moneta.Application.ViewModels;
using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;
using Moneta.Infra.CrossCutting.Enums;
using System.Linq;

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
            Mapper.CreateMap<CategoriaViewModel, Categoria>();
            Mapper.CreateMap<LancamentoViewModel, Lancamento>()
                .ForMember(d => d.LancamentoParceladoId, o => o.MapFrom(s => s.LancamentoParcelado != null ? s.LancamentoParcelado.LancamentoParceladoId : s.LancamentoParceladoId));
            Mapper.CreateMap<LancamentoParceladoViewModel, LancamentoParcelado>();
            Mapper.CreateMap<LancamentosDoMesViewModel, AgregadoLancamentosDoMes>();
        }
    }
}