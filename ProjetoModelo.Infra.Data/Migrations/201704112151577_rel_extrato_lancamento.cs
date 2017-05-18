namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rel_extrato_lancamento : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Lancamento", "ExtratoBancarioId", c => c.Guid());
            //CreateIndex("dbo.ExtratoBancario", "ExtratoBancarioId");
            //AddForeignKey("dbo.ExtratoBancario", "ExtratoBancarioId", "dbo.Lancamento", "LancamentoId");

            Sql(@"
                ALTER TABLE `moneta`.`Lancamento` 
                ADD `ExtratoBancarioId` CHAR(36),
                ADD INDEX `IX_ExtratoBancarioId` (`ExtratoBancarioId` ASC);

                ALTER TABLE moneta.Lancamento
                ADD CONSTRAINT FK_Lancamento_ExtratoBancario_ExtratoBancarioId
                FOREIGN KEY (ExtratoBancarioId) REFERENCES moneta.ExtratoBancario(ExtratoBancarioId)
                ON DELETE RESTRICT
                ON UPDATE RESTRICT;
             ");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExtratoBancario", "ExtratoBancarioId", "dbo.Lancamento");
            DropIndex("dbo.ExtratoBancario", new[] { "ExtratoBancarioId" });
            DropColumn("dbo.Lancamento", "ExtratoBancarioId");
        }
    }
}
