namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Conciliacao_GrupoLancamento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrupoLancamento", "ExtratoBancarioId", c => c.Guid());
            //AddForeignKey("dbo.ExtratoBancario", "ExtratoBancarioId", "dbo.GrupoLancamento", "GrupoLancamentoId");

            /**
ALTER TABLE `moneta`.`grupolancamento` 
ADD INDEX `FK_ExtatoBancarioId_idx` (`ExtratoBancarioId` ASC);
ALTER TABLE `moneta`.`grupolancamento` 
ADD CONSTRAINT `FK_ExtatoBancarioId`
  FOREIGN KEY (`ExtratoBancarioId`)
  REFERENCES `moneta`.`extratobancario` (`ExtratoBancarioId`)
  ON DELETE RESTRICT
  ON UPDATE RESTRICT;
             */
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.ExtratoBancario", "ExtratoBancarioId", "dbo.GrupoLancamento");
            //DropColumn("dbo.GrupoLancamento", "ExtratoBancarioId");
        }
    }
}
