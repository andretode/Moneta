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

        public void DeleteEmSerie(Lancamento lancamentoEditado)
        {
            var dataVencimentoAnterior = lancamentoEditado.GetDataVencimentoDaParcelaNaSerie();

            if (lancamentoEditado.LancamentoParcelado.TipoDeAlteracaoDaRepeticao == TipoDeAlteracaoDaRepeticaoEnum.AlterarEsteESeguintes)
            {
                SalvarFakesQueNaoSofreramRemoçãoNaSerie(lancamentoEditado, dataVencimentoAnterior);
                RemoverLancamentosNaoFakeEsteSeguintes(lancamentoEditado, dataVencimentoAnterior);
            }
        }

        /// <summary>
        /// Remove os lançamentos não fake "Este e Seguintes". Ou seja, este e os seguintes que não são fake, mais o lançamento base.
        /// </summary>
        private void RemoverLancamentosNaoFakeEsteSeguintes(Lancamento lancamentoEditado, DateTime dataVencimentoAnterior)
        {
            var lancamentosASeremRemovidos = _LancamentoRepository.GetAllReadOnly().Where(l => l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId && 
                l.DataVencimento >= dataVencimentoAnterior && l.Ativo == true).ToList();
            lancamentoEditado.LancamentoParcelado = _LancamentoParceladoRepository.GetById((Guid)lancamentoEditado.LancamentoParceladoId);
            
            //Garante que o lançamento base está sendo adicionado
            var lancamentoBase = _LancamentoRepository.GetByIdReadOnly(lancamentoEditado.LancamentoParcelado.LancamentoBaseId);
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
        private void SalvarFakesQueNaoSofreramRemoçãoNaSerie(Lancamento lancamentoEditado, DateTime dataVencimentoAnterior)
        {
            var dataInicio = lancamentoEditado.LancamentoParcelado.DataInicio;
            var lancamentoMaisFake = new LancamentoMaisFakeService(_LancamentoParceladoRepository, _LancamentoRepository);
            var lancamentosMaisFake = lancamentoMaisFake.GetAllMaisFakeAsNoTracking(dataInicio.Month, dataInicio.Year, dataVencimentoAnterior.Month, dataVencimentoAnterior.Year);
            var lancamentosSomenteFake = lancamentosMaisFake.Where(l => l.Fake == true && l.DataVencimento < dataVencimentoAnterior && l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId);
            foreach (var lf in lancamentosSomenteFake)
            {
                _LancamentoRepository.Add(lf);
            }
        }
    }
}
