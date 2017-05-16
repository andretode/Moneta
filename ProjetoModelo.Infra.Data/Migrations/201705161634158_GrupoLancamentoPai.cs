namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrupoLancamentoPai : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrupoLancamento", "GrupoLancamentoIdPai", c => c.Guid());
            AddColumn("dbo.GrupoLancamento", "GrupoLancamento_GrupoLancamentoId", c => c.Guid());
            CreateIndex("dbo.GrupoLancamento", "GrupoLancamento_GrupoLancamentoId");
            AddForeignKey("dbo.GrupoLancamento", "GrupoLancamento_GrupoLancamentoId", "dbo.GrupoLancamento", "GrupoLancamentoId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrupoLancamento", "GrupoLancamento_GrupoLancamentoId", "dbo.GrupoLancamento");
            DropIndex("dbo.GrupoLancamento", new[] { "GrupoLancamento_GrupoLancamentoId" });
            DropColumn("dbo.GrupoLancamento", "GrupoLancamento_GrupoLancamentoId");
            DropColumn("dbo.GrupoLancamento", "GrupoLancamentoIdPai");
        }
    }
}
