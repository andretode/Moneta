using System.Data.Entity.ModelConfiguration;
using Moneta.Domain.Entities;

namespace Moneta.Infra.Data.EntityConfig
{
    public class ContaConfiguration : EntityTypeConfiguration<Conta>
    {
        public ContaConfiguration()
        {
            HasKey(c => c.ContaId);

            Property(c => c.Descricao)
                .IsRequired();

            Property(c => c.DataCadastro)
                .IsRequired();
            
            Ignore(t => t.ResultadoValidacao);
        }
    }
}
