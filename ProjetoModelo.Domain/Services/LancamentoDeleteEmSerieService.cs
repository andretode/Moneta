using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.CrossCutting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Services
{
    public class LancamentoDeleteEmSerieService
    {
        private readonly ILancamentoRepository _LancamentoRepository;
        private readonly ILancamentoParceladoRepository _LancamentoParceladoRepository;

        public LancamentoDeleteEmSerieService(
            ILancamentoParceladoRepository LancamentoParceladoRepository,
            ILancamentoRepository LancamentoRepository)
        {
            _LancamentoParceladoRepository = LancamentoParceladoRepository;
            _LancamentoRepository = LancamentoRepository;
        }

        public void DeleteEmSerie(Lancamento lancamentoRemovido)
        {
            lancamentoRemovido.Ativo = false;

            var dataVencimentoAnterior = lancamentoRemovido.GetDataVencimentoDaParcelaNaSerie();
            
            if (lancamentoRemovido.LancamentoParcelado.TipoDeAlteracaoDaRepeticao == TipoDeAlteracaoDaRepeticaoEnum.AlterarApenasEste)
            {
                lancamentoRemovido.LancamentoParcelado = null;
                if (lancamentoRemovido.Fake)
                    _LancamentoRepository.Add(lancamentoRemovido);
                else
                    _LancamentoRepository.Update(lancamentoRemovido);
            }
            else if (lancamentoRemovido.LancamentoParcelado.TipoDeAlteracaoDaRepeticao == TipoDeAlteracaoDaRepeticaoEnum.AlterarEsteESeguintes)
            {
                SalvarFakesQueNaoSofreramRemoçãoNaSerie(lancamentoRemovido, dataVencimentoAnterior);
                RemoverLancamentosNaoFakeEsteSeguintes(lancamentoRemovido, dataVencimentoAnterior);
            }
        }

        /// <summary>
        /// Remove os lançamentos não fake "Este e Seguintes". Ou seja, este e os seguintes que não são fake, mais o lançamento base.
        /// </summary>
        private void RemoverLancamentosNaoFakeEsteSeguintes(Lancamento lancamentoRemovido, DateTime dataVencimentoAnterior)
        {
            var lancamentosASeremRemovidos = _LancamentoRepository.GetAllReadOnly().Where(l => l.LancamentoParceladoId == lancamentoRemovido.LancamentoParceladoId && 
                l.DataVencimento >= dataVencimentoAnterior && l.Ativo == true).ToList();
            lancamentoRemovido.LancamentoParcelado = _LancamentoParceladoRepository.GetById((Guid)lancamentoRemovido.LancamentoParceladoId);
            
            //Garante que o lançamento base está sendo adicionado
            var lancamentoBase = _LancamentoRepository.GetByIdReadOnly(lancamentoRemovido.LancamentoParcelado.LancamentoBaseId);
            if (!lancamentosASeremRemovidos.Contains(lancamentoBase))
                lancamentosASeremRemovidos.Add(lancamentoBase); 

            foreach (var l in lancamentosASeremRemovidos)
            {
                l.Ativo = false;
                _LancamentoRepository.Update(l);
            }
        }

        /// <summary>
        /// Salva em BD os fakes que não sofreram remoção na série devido ao "Remover este e seguintes".
        /// </summary>
        private void SalvarFakesQueNaoSofreramRemoçãoNaSerie(Lancamento lancamentoRemovido, DateTime dataVencimentoAnterior)
        {
            var dataInicio = lancamentoRemovido.LancamentoParcelado.DataInicio;
            var lancamentoMaisFake = new LancamentoMaisFakeService(_LancamentoParceladoRepository, _LancamentoRepository);
            var lancamentosMaisFake = lancamentoMaisFake.GetAllMaisFakeAsNoTracking(dataInicio.Month, dataInicio.Year, dataVencimentoAnterior.Month, dataVencimentoAnterior.Year);
            var lancamentosSomenteFake = lancamentosMaisFake.Where(l => l.Fake == true && l.DataVencimento < dataVencimentoAnterior && l.LancamentoParceladoId == lancamentoRemovido.LancamentoParceladoId);
            foreach (var lf in lancamentosSomenteFake)
            {
                _LancamentoRepository.Add(lf);
            }
        }
    }
}
