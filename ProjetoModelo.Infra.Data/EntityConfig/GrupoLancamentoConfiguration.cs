using System.Data.Entity.ModelConfiguration;
using Moneta.Domain.Entities;

namespace Moneta.Infra.Data.EntityConfig
{
    public class GrupoLancamentoConfiguration : EntityTypeConfiguration<GrupoLancamento>
    {
        public GrupoLancamentoConfiguration()
        {
            HasKey(c => c.GrupoLancamentoId);

            Property(c => c.Descricao)
                .IsRequired();

            Property(c => c.DataVencimento)
                .IsRequired();

            Property(c => c.DataCadastro)
                .IsRequired();

            HasMany(l => l.Lancamentos)
                .WithOptional(l => l.GrupoLancamento)
                .HasForeignKey(l => l.GrupoLancamentoId);

            Property(c => c.ContaId)
                .IsRequired();

            HasRequired(c => c.Conta)
                .WithMany(c => c.GruposLancamento)
                .HasForeignKey(c => c.ContaId);
            
            Ignore(t => t.ResultadoValidacao);
        }
    }
}
