using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.Entities;
using System.Collections.Generic;
using Moneta.Domain.Services;

namespace Moneta.Domain.Test
{
    [TestClass]
    public class LancamentoMaisFakeServiceTest
    {
        [TestMethod]
        public void ValidaOsLancamentosDeBdMaisFakeRetornados()
        {
            var lancamentos = new List<Lancamento>();
            var lancamento_fixo_base_0110216 = new Lancamento()
            {
                Descricao = "despesa fixa 1 (semanal)",
                Valor = -100,
                Ativo = true,
                DataVencimento = new DateTime(2016, 10, 1),
                LancamentoParceladoId = Guid.Parse("e17f384a-9c71-4047-a2e2-4046e603db1d"),
                LancamentoId = Guid.Parse("bf7c51f5-b0a4-47ac-9bc5-7f8622f3a957")
            };
            lancamentos.Add(lancamento_fixo_base_0110216);
            lancamentos.Add(new Lancamento() { Descricao = "despesa 2", Valor = -200, Ativo = true, DataVencimento = new DateTime(2016, 10, 2)});
            lancamentos.Add(new Lancamento() { Descricao = "despesa 3", Valor = -50, Ativo = true, DataVencimento = new DateTime(2016, 10, 2)});
            lancamentos.Add(new Lancamento() { Descricao = "salario 4", Valor = 2000, Ativo = true, DataVencimento = new DateTime(2016, 10, 4) });

            //inclusao de lançamentos que estão com data de vencimento em outro mes e nao devem ser contabilizado
            var lancamento_040916 = new Lancamento() { Descricao = "salario 5", Valor = 1001, Ativo = true, DataVencimento = new DateTime(2016, 9, 4) };
            lancamentos.Add(lancamento_040916);
            var lancamento_041116 = new Lancamento() { Descricao = "salario 5", Valor = 1002, Ativo = true, DataVencimento = new DateTime(2016, 11, 4)};
            lancamentos.Add(lancamento_041116);

            //este está desativado e nao deve ser recuperado
            var lancamento_fixo_071016 = new Lancamento()
            {
                Descricao = "despesa fixa 1 (semanal)",
                Valor = -100,
                Ativo = false,
                DataVencimento = new DateTime(2016, 10, 7),
                IdDaParcelaNaSerie = "bf7c51f5-b0a4-47ac-9bc5-7f8622f3a957" + (new DateTime(2016, 10, 8))
            };
            lancamentos.Add(lancamento_fixo_071016);

            //incluir uma despesa fixa ativa com valor diferente para confirmar que um fake nao será criado no seu lugar
            var lancamento_fixo_171015 = new Lancamento()
            {
                Descricao = "despesa fixa 1 (semanal)",
                Valor = -1,
                Ativo = true,
                DataVencimento = new DateTime(2016, 10, 17),
                IdDaParcelaNaSerie = "bf7c51f5-b0a4-47ac-9bc5-7f8622f3a957" + (new DateTime(2016, 10, 15))
            };
            lancamentos.Add(lancamento_fixo_171015);

            var lancamentosParcelados = new List<LancamentoParcelado>();
            lancamentosParcelados.Add(new LancamentoParcelado() { DataInicio = new DateTime(2016, 10, 1), LancamentoBaseId = Guid.Parse("bf7c51f5-b0a4-47ac-9bc5-7f8622f3a957"), Periodicidade=7,
                LancamentoParceladoId = Guid.Parse("e17f384a-9c71-4047-a2e2-4046e603db1d")});

            var mockLancamentoRepository = new Mock<ILancamentoRepository>();
            mockLancamentoRepository.Setup(l => l.GetAll(true)).Returns(lancamentos);
            mockLancamentoRepository.Setup(l => l.GetById(Guid.Parse("bf7c51f5-b0a4-47ac-9bc5-7f8622f3a957"))).Returns(lancamento_fixo_base_0110216);

            var mockLancamentoParceladoRepository = new Mock<ILancamentoParceladoRepository>();
            mockLancamentoParceladoRepository.Setup(l => l.GetAll()).Returns(lancamentosParcelados);

            var lancamentoMaisFakeService = new LancamentoMaisFakeService(mockLancamentoParceladoRepository.Object, mockLancamentoRepository.Object);
            var resultado = lancamentoMaisFakeService.GetAllMaisFake(10, 2016);

            Assert.IsTrue(resultado.Count == 7);
            Assert.IsFalse(resultado.Contains(lancamento_040916));
            Assert.IsFalse(resultado.Contains(lancamento_041116));
            Assert.IsFalse(resultado.Contains(lancamento_fixo_071016));
            Assert.IsTrue(resultado.Contains(lancamento_fixo_171015));
        }
    }
}
