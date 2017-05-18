using Moneta.Infra.CrossCutting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moneta.Application.ViewModels
{
    public class LancamentoAgrupadoViewModel
    {
        public string Descricao { get; set; }
        public List<LancamentoViewModel> Lancamentos { get; set; }

        public TipoGrupoLancamentoEnum TipoGrupoLancamento { get; private set; }

        public GrupoLancamentoViewModel GrupoLancamento
        {
            get
            {
                return Lancamentos.First().GrupoLancamento;
            }
        }

        public Guid? GrupoLancamentoId { 
            get
            {
                return GrupoLancamento.GrupoLancamentoId;
            } 
        }

        public bool Pago {
            get
            {
                return Lancamentos.Where(l => !l.Pago).Count().Equals(0);
            }
        }
    }
}
