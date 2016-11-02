using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Moneta.Domain.Entities;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

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

            Property(c => c.BaseDaSerie)
                .IsRequired();

            Property(c => c.IdDaParcelaNaSerie)
                .IsOptional()
                .HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute("IX_IdDaParcelaNaSerie") { IsUnique = true }));

            Property(c => c.Ativo)
                .IsRequired();

            Property(c => c.TipoDeTransacao)
                .IsRequired();

            HasRequired(c => c.Categoria)
                .WithMany(c => c.Lancamentos)
                .HasForeignKey(c => c.CategoriaId);

            Property(c => c.ContaId)
                .IsRequired();

            HasRequired(c => c.Conta)
                .WithMany(c => c.Lancamentos)
                .HasForeignKey(c => c.ContaId);

            Property(c => c.LancamentoParceladoId)
                .IsOptional();

            HasOptional(c => c.LancamentoParcelado)
                .WithMany(c => c.Lancamentos)
                .HasForeignKey(c => c.LancamentoParceladoId);

            Property(c => c.LancamentoIdTransferencia)
                .IsOptional()
                .HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute("IX_LancamentoIdTransferencia") { IsUnique = true }));

            //HasOptional(c => c.LancamentoTransferencia)
            //    .WithMany(c => c.LancamentosTransferencia)
            //    .HasForeignKey(c => c.LancamentoIdTransferencia)
            //    .WillCascadeOnDelete();

            Ignore(t => t.ResultadoValidacao);
            Ignore(t => t.Fake);
            Ignore(t => t.LancamentoTransferencia);
        }
    }
}
