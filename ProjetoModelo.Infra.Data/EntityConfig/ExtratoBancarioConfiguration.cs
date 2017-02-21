using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Moneta.Domain.Entities;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moneta.Infra.Data.EntityConfig
{
    public class ExtratoBancarioConfiguration : EntityTypeConfiguration<ExtratoBancario>
    {
        public ExtratoBancarioConfiguration()
        {
            HasKey(c => c.ExtratoBancarioId);

            Property(c => c.Descricao)
                .IsRequired();

            Property(c => c.NumeroDocumento)
                .IsRequired();

            Property(c => c.DataCadastro)
                .IsRequired();

            Property(c => c.DataCompensacao)
                .IsRequired();

            Property(c => c.ContaId)
                .IsRequired();

            HasRequired(c => c.Conta)
                .WithMany(c => c.ExtratosBancarios)
                .HasForeignKey(c => c.ContaId);


            Ignore(t => t.ResultadoValidacao);
        }
    }
}
