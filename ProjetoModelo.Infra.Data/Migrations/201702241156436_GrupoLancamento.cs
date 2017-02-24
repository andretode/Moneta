namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrupoLancamento : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrupoLancamento",
                c => new
                    {
                        GrupoLancamentoId = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 100, unicode: false),
                        DataVencimento = c.DateTime(nullable: false, precision: 0),
                        DataCadastro = c.DateTime(nullable: false, precision: 0),
                        Pago = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GrupoLancamentoId);
            
            AddColumn("dbo.Lancamento", "GrupoLancamentoId", c => c.Guid());
            CreateIndex("dbo.Lancamento", "GrupoLancamentoId");
            AddForeignKey("dbo.Lancamento", "GrupoLancamentoId", "dbo.GrupoLancamento", "GrupoLancamentoId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lancamento", "GrupoLancamentoId", "dbo.GrupoLancamento");
            DropIndex("dbo.Lancamento", new[] { "GrupoLancamentoId" });
            DropColumn("dbo.Lancamento", "GrupoLancamentoId");
            DropTable("dbo.GrupoLancamento");
        }
    }
}
