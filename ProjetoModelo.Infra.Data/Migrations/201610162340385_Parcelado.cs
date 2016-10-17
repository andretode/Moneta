namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Parcelado : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LancamentoParcelado",
                c => new
                    {
                        LancamentoParceladoId = c.Guid(nullable: false),
                        NumeroParcelas = c.Int(nullable: false),
                        DataInicio = c.DateTime(nullable: false),
                        DataCadastro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LancamentoParceladoId);
            
            AddColumn("dbo.Lancamento", "LancamentoParceladoId", c => c.Guid());
            CreateIndex("dbo.Lancamento", "LancamentoParceladoId");
            AddForeignKey("dbo.Lancamento", "LancamentoParceladoId", "dbo.LancamentoParcelado", "LancamentoParceladoId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lancamento", "LancamentoParceladoId", "dbo.LancamentoParcelado");
            DropIndex("dbo.Lancamento", new[] { "LancamentoParceladoId" });
            DropColumn("dbo.Lancamento", "LancamentoParceladoId");
            DropTable("dbo.LancamentoParcelado");
        }
    }
}
