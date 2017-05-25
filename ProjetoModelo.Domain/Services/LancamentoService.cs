using System;
using System.Collections.Generic;
using System.Linq;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.Interfaces.Repository.ADO;
using Moneta.Domain.Interfaces.Repository.ReadOnly;
using Moneta.Domain.Interfaces.Services;
using Moneta.Domain.ValueObjects;
using Moneta.Infra.CrossCutting.Enums;

namespace Moneta.Domain.Services
{
    public class LancamentoService : ServiceBase<Lancamento>, ILancamentoService
    {
        protected readonly ILancamentoRepository _LancamentoRepository;
        protected readonly ILancamentoParceladoRepository _LancamentoParceladoRepository;
        protected readonly IGrupoLancamentoRepository _GrupoLancamentoRepository;
        protected readonly LancamentoDoMesService _LancamentoDoMesService;

        public LancamentoService(
            IGrupoLancamentoRepository GrupoLancamentoRepository,
            ILancamentoParceladoRepository LancamentoParceladoRepository,
            ILancamentoRepository LancamentoRepository)
            : base(LancamentoRepository)
        {
            _GrupoLancamentoRepository = GrupoLancamentoRepository;
            _LancamentoParceladoRepository = LancamentoParceladoRepository;
            _LancamentoRepository = LancamentoRepository;
            _LancamentoDoMesService = new LancamentoDoMesService(_LancamentoRepository, 
                _LancamentoParceladoRepository, _GrupoLancamentoRepository);
        }

        public override Lancamento GetById(Guid id)
        {
            return _LancamentoRepository.GetById(id);
        }

        public Lancamento GetByIdReadOnly(Guid id)
        {
            return _LancamentoRepository.GetByIdReadOnly(id);
        }

        public ValidationResult Adicionar(Lancamento lancamento)
        {
            var resultadoValidacao = new ValidationResult();

            if (!lancamento.IsValid())
            {
                resultadoValidacao.AdicionarErro(lancamento.ResultadoValidacao);
                return resultadoValidacao;
            }

            base.Add(lancamento);

            return resultadoValidacao;
        }

        public override void Update(Lancamento lancamento)
        {
            if (lancamento.TipoDeTransacao == TipoTransacaoEnum.Transferencia && lancamento.LancamentoTransferencia != null)
            {
                var lancamentoTransferenciaPar = lancamento.LancamentoTransferencia;
                lancamentoTransferenciaPar.Valor = lancamento.Valor * -1;
                lancamentoTransferenciaPar.Descricao = lancamento.Descricao;
                _LancamentoRepository.Update(lancamentoTransferenciaPar);
            }

            _LancamentoRepository.Update(lancamento);
        }

        public void UpdateEmSerie(Lancamento lancamentoEditado)
        {
            var lancamentoUpdateEmSerieService = new LancamentoUpdateEmSerieService(_LancamentoParceladoRepository, _LancamentoRepository);
            lancamentoUpdateEmSerieService.UpdateEmSerie(lancamentoEditado);
        }

        public void RemoveEmSerie(Lancamento lancamento)
        {
            var lancamentoDeleteEmSerieService = new LancamentoDeleteEmSerieService(_LancamentoParceladoRepository, _LancamentoRepository);
            lancamentoDeleteEmSerieService.DeleteEmSerie(lancamento);
            RemoverBaseDaSerieSeIncluso(lancamento);
        }

        /// <summary>
        /// Remove Lancamento Base e LancamentoParcelado quando o primeiro lançamento da série é selecionado para 'remover este e os seguintes'.
        /// </summary>
        /// <param name="lancamento">O lançamento selecionado pelo usuário</param>
        private void RemoverBaseDaSerieSeIncluso(Lancamento lancamento)
        {
            var lancamentoBaseDaSerie = _LancamentoRepository.GetById(lancamento.LancamentoParcelado.LancamentoBaseId);
            if (lancamentoBaseDaSerie.DataVencimento == lancamento.DataVencimento)
            {
                _LancamentoParceladoRepository.Remove(lancamento.LancamentoParcelado);
                _LancamentoRepository.Remove(lancamentoBaseDaSerie);
            }
        }

        public void RemoveTransferencia(Lancamento lancamento)
        {
            var lancamentoPar = _LancamentoRepository.GetByIdReadOnly((Guid)lancamento.LancamentoIdTransferencia);
            _LancamentoRepository.Remove(lancamentoPar);
            _LancamentoRepository.Remove(lancamento);
        }

        public AgregadoLancamentosDoMes GetLancamentosDoMes(AgregadoLancamentosDoMes lancamentosDoMes)
        {
            return _LancamentoDoMesService.GetLancamentosDoMes(lancamentosDoMes);
        }

        public void TrocarPago(IEnumerable<Lancamento> lancamentos)
        {
            if(lancamentos.Count() > 0)
            {
                var pago = lancamentos.First().GrupoLancamento.Pago;
                
                foreach (var lancamento in lancamentos)
                {
                    lancamento.Pago = !pago;
                    _LancamentoRepository.Update(lancamento);
                }
            }
        }

        public int ImportarOfxParaGrupoDeLancamento(string caminhoOfx, Guid contaId, DateTime mesAnoCompetencia, Guid grupoLancamentoId)
        {
            var novosLancamentosOfx = new List<Lancamento>();
            var grupoReceptor = _GrupoLancamentoRepository.GetById(grupoLancamentoId);
            IEnumerable<IExtratoOfx> lancamentosExistentesNoGrupo = IncluirLancamentosDivididosDoGrupo(grupoReceptor);

            IEnumerable<IExtratoOfx> extratosOfx = ImportacaoOfxService.ImportarNovosExtratosOfx(caminhoOfx, contaId, lancamentosExistentesNoGrupo, mesAnoCompetencia, true);

            foreach(var extrato in extratosOfx)
                novosLancamentosOfx.Add(new Lancamento(extrato, grupoLancamentoId));

            foreach (var lancamento in novosLancamentosOfx)
            {
                _LancamentoRepository.Add(lancamento);
            }

            return novosLancamentosOfx.Count();
        }

        private IEnumerable<Lancamento> IncluirLancamentosDivididosDoGrupo(GrupoLancamento grupoReceptor)
        {
            var lancamentosDivididos = new List<Lancamento>();
            foreach (var grupo in grupoReceptor.GruposDeLancamentos)
            {
                lancamentosDivididos.AddRange(grupo.Lancamentos);
            }

            return grupoReceptor.Lancamentos.Union(lancamentosDivididos);
        }


        /// <summary>
        /// Busca as receitas, despesas e saldos da movimentação financeira de uma conta por dia
        /// </summary>
        /// <param name="lancamentosDoMes">Dados informados pelo usuário para filtrar a pesquisa</param>
        /// <param name="resumido">Informe true caso queria um conjunto de dados somente no dia que houve movimentações financeira</param>
        /// <returns>Retorna um conjunto de dados de receitas, despesas e saldos de uma conta por dia. A Tuple é na sequencia: data da movimentação financeira, receita, despesa e saldo.</returns>
        public List<Tuple<DateTime, decimal, decimal, decimal>> GetSaldoDoMesPorDia(AgregadoLancamentosDoMes lancamentosDoMes, bool resumido)
        {
            var listaDeSaldoPorDia = new List<Tuple<DateTime, decimal, decimal, decimal>>();
            var agregadoLancamentosDoMes = GetLancamentosDoMes(lancamentosDoMes);
            var lancamentos = agregadoLancamentosDoMes.LancamentosDoMes;

            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;

            List<DateTime> arrayDataVencimento;
            if (resumido)
                arrayDataVencimento = SomenteDiasDoMesComMovimentacao(mes, ano, lancamentos);
            else
                arrayDataVencimento = TodosDiasDoMes(mes, ano);

            decimal receitaAcumulada = 0;
            decimal despesaAcumulada = 0;
            decimal saldoAcumulado = 0;
            decimal saldoDoMesAnterior = agregadoLancamentosDoMes.SaldoDoMesAnterior;
            if (saldoDoMesAnterior > 0)
                receitaAcumulada = saldoDoMesAnterior;
            else
                despesaAcumulada = saldoDoMesAnterior;
            foreach (var dia in arrayDataVencimento)
            {
                receitaAcumulada += lancamentos.Where(l => l.DataVencimento == dia && l.TipoDeTransacao == TipoTransacaoEnum.Receita).Sum(l => l.Valor);
                despesaAcumulada += lancamentos.Where(l => l.DataVencimento == dia && l.TipoDeTransacao == TipoTransacaoEnum.Despesa).Sum(l => l.Valor);
                saldoAcumulado = receitaAcumulada + despesaAcumulada;
                listaDeSaldoPorDia.Add(new Tuple<DateTime, decimal, decimal, decimal>(dia, receitaAcumulada, Math.Abs(despesaAcumulada), saldoAcumulado));
            }

            return listaDeSaldoPorDia;
        }

        public List<SaldoPorCategoria> GetDespesasPorCategoria(Guid ContaIdFiltro, DateTime mesAnoCompetencia, bool pago)
        {
            var listaDeSaldoPorCategoria = new List<SaldoPorCategoria>();
            var lancamentosDoMes = new AgregadoLancamentosDoMes();
            lancamentosDoMes.MesAnoCompetencia = mesAnoCompetencia;
            
            if (ContaIdFiltro != null)
                lancamentosDoMes.ContaIdFiltro = (Guid) ContaIdFiltro;
            
            var agregadoLancamentosDoMes = GetLancamentosDoMes(lancamentosDoMes);

            var lancamentosFiltrados = agregadoLancamentosDoMes.LancamentosDoMes
                .Where(l => l.TipoDeTransacao == TipoTransacaoEnum.Despesa
                    && l.Categoria.Descricao != Categoria.Nenhum);

            if(pago)
            {
                lancamentosFiltrados = lancamentosFiltrados.Where(l => l.Pago == true);
            }

            var agrupadosPorCategoria = lancamentosFiltrados
                .OrderBy(l => l.Categoria.Descricao)
                .GroupBy(l => l.Categoria);

            foreach (var lancamento in agrupadosPorCategoria)
            {
                var orcadoMensal = lancamento.First().Categoria.OrcamentoMensal;
                listaDeSaldoPorCategoria.Add(new SaldoPorCategoria() { 
                    Categoria = lancamento.First().Categoria.Descricao,
                    CorHex = lancamento.First().Categoria.Cor,
                    OrcamentoMensal = (orcadoMensal == null ? 0 : (decimal)orcadoMensal),
                    Saldo = lancamento.Sum(l => l.Valor)
                });
            }

            return listaDeSaldoPorCategoria;
        }

        #region Metodos Privados
        private List<DateTime> SomenteDiasDoMesComMovimentacao(int mes, int ano, IEnumerable<Lancamento> lancamentos)
        {
            var arrayDataVencimento = new List<DateTime>();
            arrayDataVencimento.Add(new DateTime(ano, mes, 1));
            arrayDataVencimento.AddRange(lancamentos.Select(l => l.DataVencimento).Distinct().ToList());
            arrayDataVencimento.Add(new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)));
            arrayDataVencimento = arrayDataVencimento.Distinct().ToList();

            return arrayDataVencimento;
        }

        private List<DateTime> TodosDiasDoMes(int mes, int ano)
        {
            var datas = new List<DateTime>();
            var dataFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
            var dataCounter = new DateTime(ano, mes, 1);
            while (dataCounter <= dataFinal)
            {
                datas.Add(dataCounter);
                dataCounter = dataCounter.AddDays(1);
            }
            return datas;
        }
        #endregion
    }
}
