namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removernavegacaotransferencia : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lancamento", "LancamentoIdTransferencia", "dbo.Lancamento");
        }
        
        public override void Down()
        {
            AddForeignKey("dbo.Lancamento", "LancamentoIdTransferencia", "dbo.Lancamento", "LancamentoId", cascadeDelete: true);
        }
    }
}
