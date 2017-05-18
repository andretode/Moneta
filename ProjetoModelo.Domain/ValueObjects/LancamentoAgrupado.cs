using Moneta.Infra.CrossCutting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public class LancamentoAgrupado
    {
        public string Descricao { get; set; }
        public List<Lancamento> Lancamentos { get; set; }
        public TipoGrupoLancamentoEnum TipoGrupoLancamento
        {
            get
            {
                var primeiro = Lancamentos.First();

                if (primeiro.GrupoLancamento == null)
                    return TipoGrupoLancamentoEnum.Unico;
                else if (primeiro.GrupoLancamento.GrupoLancamentoIdPai == null)
                    return TipoGrupoLancamentoEnum.GrupoPai;
                else
                    return TipoGrupoLancamentoEnum.GrupoFilho;
            }
        }
    }
}
