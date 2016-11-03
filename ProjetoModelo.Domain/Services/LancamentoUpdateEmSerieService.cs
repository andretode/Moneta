using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.CrossCutting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moneta.Domain.Services
{
    public class LancamentoUpdateEmSerieService
    {
        private readonly ILancamentoRepository _LancamentoRepository;
        private readonly ILancamentoParceladoRepository _LancamentoParceladoRepository;

        public LancamentoUpdateEmSerieService(
            ILancamentoParceladoRepository LancamentoParceladoRepository,
            ILancamentoRepository LancamentoRepository)
        {
            _LancamentoParceladoRepository = LancamentoParceladoRepository;
            _LancamentoRepository = LancamentoRepository;
        }

        public void UpdateEmSerie(Lancamento lancamentoEditado)
        {
            var dataVencimentoAnterior = lancamentoEditado.GetDataVencimentoDaParcelaNaSerie();
            var diasDiff = (lancamentoEditado.DataVencimento - dataVencimentoAnterior).TotalDays;

            IEnumerable<Lancamento> lancamentosASeremAlterados;
            if (lancamentoEditado.LancamentoParcelado.TipoDeAlteracaoDaRepeticao == TipoDeAlteracaoDaRepeticaoEnum.AlterarEsteESeguintes)
            {
                AtualizarDataIdSerieDosNaoFakesAnteriores(lancamentoEditado, dataVencimentoAnterior, diasDiff);
                SalvarFakesQueNaoSofreramAlteraçãoNaSerie(lancamentoEditado, dataVencimentoAnterior, diasDiff);
                AtualizarLancamentosNaoFakeQueSofreramAlteracoes(lancamentoEditado, dataVencimentoAnterior, diasDiff);
            }
            else
            {
                lancamentosASeremAlterados = _LancamentoRepository.GetAllReadOnly().Where(l => l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId);
                AtualizarLancamentos(lancamentosASeremAlterados, lancamentoEditado, diasDiff);
            }
        }

        /// <summary>
        /// Atualiza os lançamentos não fake que sofream alterações. Ou seja, este e os seguintes que não são fake, mais o lançamento base.
        /// </summary>
        private void AtualizarLancamentosNaoFakeQueSofreramAlteracoes(Lancamento lancamentoEditado, DateTime dataVencimentoAnterior, double diasDiff)
        {
            var lancamentoBdAtualMaisSeguintes = _LancamentoRepository.GetAllReadOnly().Where(l => l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId && l.DataVencimento >= dataVencimentoAnterior).ToList();
            lancamentoBdAtualMaisSeguintes.Add(_LancamentoRepository.GetByIdReadOnly(lancamentoEditado.LancamentoParcelado.LancamentoBaseId)); //garante que o lançamento base está sendo adicionado
            AtualizarLancamentos(lancamentoBdAtualMaisSeguintes, lancamentoEditado, diasDiff);
        }

        /// <summary>
        /// Atualiza a data de vencimento do ID da Série dos não fakes anteriores
        /// </summary>
        private void AtualizarDataIdSerieDosNaoFakesAnteriores(Lancamento lancamentoEditado, DateTime dataVencimentoAnterior, double diasDiff)
        {
            var lancamentoBdAnteriores = _LancamentoRepository.GetAllReadOnly().Where(l => l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId &&
                l.DataVencimento < dataVencimentoAnterior && !l.BaseDaSerie).ToList();

            foreach (var l in lancamentoBdAnteriores)
            {
                l.AddDaysDataVencimentoDaParcelaNaSerie(diasDiff);
                _LancamentoRepository.Update(l);
            }
        }

        private void AtualizarLancamentos(IEnumerable<Lancamento> lancamentosASeremAlterados, Lancamento lancamentoEditado, double diasDiff)
        {
            foreach (var l in lancamentosASeremAlterados)
            {
                l.Descricao = lancamentoEditado.Descricao;
                l.Valor = lancamentoEditado.Valor;
                l.ContaId = lancamentoEditado.ContaId;
                l.CategoriaId = lancamentoEditado.CategoriaId;
                l.AddDaysDataVencimentoDaParcelaNaSerie(diasDiff);

                if (!l.BaseDaSerie)
                    l.DataVencimento = l.GetDataVencimentoDaParcelaNaSerie(); //.AddDays(diasDiff);
                else
                    l.DataVencimento = l.DataVencimento.AddDays(diasDiff);

                _LancamentoRepository.Update(l);
            }
        }

        /// <summary>
        /// Salva em BD os fakes que não sofreram alterações na série devido ao "Alterar este e seguintes". Atualiza também a data base da série, que é usado como identificado da parcela na série.
        /// </summary>
        private void SalvarFakesQueNaoSofreramAlteraçãoNaSerie(Lancamento lancamentoEditado, DateTime dataVencimentoAnterior, double diasDiff)
        {
            var dataInicio = lancamentoEditado.LancamentoParcelado.DataInicio;
            var lancamentoMaisFake = new LancamentoMaisFakeService(_LancamentoParceladoRepository, _LancamentoRepository);
            var lancamentosMaisFake = lancamentoMaisFake.GetAllMaisFakeAsNoTracking(dataInicio.Month, dataInicio.Year, dataVencimentoAnterior.Month, dataVencimentoAnterior.Year);
            var lancamentosSomenteFake = lancamentosMaisFake.Where(l => l.Fake == true && l.DataVencimento < dataVencimentoAnterior && l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId);
            foreach (var lf in lancamentosSomenteFake)
            {
                lf.AddDaysDataVencimentoDaParcelaNaSerie(diasDiff);
                _LancamentoRepository.Add(lf);
            }
        }
    }
}
