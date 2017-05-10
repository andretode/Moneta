using System.Data.Entity.ModelConfiguration;
using Moneta.Domain.Entities;

namespace Moneta.Infra.Data.EntityConfig
{
    public class CategoriaConfiguration : EntityTypeConfiguration<Categoria>
    {
        public CategoriaConfiguration()
        {
            HasKey(c => c.CategoriaId);

            Property(c => c.Descricao)
                .IsRequired();

            Property(c => c.Cor)
                .IsRequired();

            Property(c => c.OrcamentoMensal)
                .IsOptional();

            Property(c => c.DataCadastro)
                .IsRequired();

            HasMany(l => l.Lancamentos)
                .WithRequired(l => l.Categoria)
                .HasForeignKey(l => l.CategoriaId);
            
            Ignore(t => t.ResultadoValidacao);
        }
    }
}
