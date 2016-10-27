namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Transferencia : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lancamento", "LancamentoIdTransferencia", c => c.Guid());
            CreateIndex("dbo.Lancamento", "LancamentoIdTransferencia", unique: true);
            AddForeignKey("dbo.Lancamento", "LancamentoIdTransferencia", "dbo.Lancamento", "LancamentoId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lancamento", "LancamentoIdTransferencia", "dbo.Lancamento");
            DropIndex("dbo.Lancamento", new[] { "LancamentoIdTransferencia" });
            DropColumn("dbo.Lancamento", "LancamentoIdTransferencia");
        }
    }
}
