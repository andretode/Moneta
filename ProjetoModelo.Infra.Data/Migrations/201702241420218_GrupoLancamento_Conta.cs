namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrupoLancamento_Conta : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrupoLancamento", "ContaId", c => c.Guid(nullable: false));
            CreateIndex("dbo.GrupoLancamento", "ContaId");
            AddForeignKey("dbo.GrupoLancamento", "ContaId", "dbo.Conta", "ContaId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrupoLancamento", "ContaId", "dbo.Conta");
            DropIndex("dbo.GrupoLancamento", new[] { "ContaId" });
            DropColumn("dbo.GrupoLancamento", "ContaId");
        }
    }
}
