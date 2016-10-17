using System.Data.Entity.ModelConfiguration;
using Moneta.Domain.Entities;

namespace Moneta.Infra.Data.EntityConfig
{
    public class LancamentoParceladoConfiguration : EntityTypeConfiguration<LancamentoParcelado>
    {
        public LancamentoParceladoConfiguration()
        {
            HasKey(c => c.LancamentoParceladoId);

            Property(c => c.NumeroParcelas)
                .IsOptional();

            Property(c => c.Periodicidade)
                .IsRequired();

            Property(c => c.DataCadastro)
                .IsRequired();

            Property(c => c.DataInicio)
                .IsRequired();

            HasMany(l => l.Lancamentos)
                .WithOptional(l => l.LancamentoParcelado);

            Ignore(t => t.ResultadoValidacao);
        }
    }
}
