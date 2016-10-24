using System.Data.Entity.ModelConfiguration;
using Moneta.Domain.Entities;
using Moneta.Infra.CrossCutting.Enums;

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
            
            Property(c => c.LancamentoBaseId)
                .IsRequired();

            //HasRequired(c => c.LacamentoOriginal)
            //    .WithRequiredDependent(c => c.LancamentoParcelado);

            Ignore(t => t.ResultadoValidacao);
            Ignore(t => t.TipoDeAlteracaoDaRepeticao);
        }
    }
}
