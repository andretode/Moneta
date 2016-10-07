using System.Data.Entity.ModelConfiguration;
using Moneta.Domain.Entities;

namespace Moneta.Infra.Data.EntityConfig
{
    public class LancamentoConfiguration : EntityTypeConfiguration<Lancamento>
    {
        public LancamentoConfiguration()
        {
            HasKey(c => c.LancamentoId);

            Property(c => c.Descricao)
                .IsRequired();

            Property(c => c.DataCadastro)
                .IsRequired();

            Property(c => c.CategoriaId)
                .IsRequired();

            Property(c => c.Pago)
                .IsRequired();

            HasRequired(c => c.Categoria)
                .WithMany(c => c.Lancamentos)
                .HasForeignKey(c => c.CategoriaId);

            Property(c => c.ContaId)
                .IsRequired();

            HasRequired(c => c.Conta)
                .WithMany(c => c.Lancamentos)
                .HasForeignKey(c => c.ContaId);
            
            Ignore(t => t.ResultadoValidacao);
        }
    }
}
