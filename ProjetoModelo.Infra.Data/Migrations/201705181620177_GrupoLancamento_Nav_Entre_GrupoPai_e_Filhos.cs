namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrupoLancamento_Nav_Entre_GrupoPai_e_Filhos : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.GrupoLancamento", "GrupoLancamentoIdPai");
            //RenameColumn(table: "dbo.GrupoLancamento", name: "GrupoLancamento_GrupoLancamentoId", newName: "GrupoLancamentoIdPai");
            //RenameIndex(table: "dbo.GrupoLancamento", name: "IX_GrupoLancamento_GrupoLancamentoId", newName: "IX_GrupoLancamentoIdPai");

            Sql(@"
                ALTER TABLE GrupoLancamento DROP COLUMN GrupoLancamentoIdPai;
                ALTER TABLE GrupoLancamento DROP FOREIGN KEY FK_520992b6a26545ea9e67113595d23cab;
                ALTER TABLE GrupoLancamento CHANGE COLUMN GrupoLancamento_GrupoLancamentoId GrupoLancamentoIdPai VARCHAR(36) CHARACTER SET 'latin1' COLLATE 'latin1_bin' NULL DEFAULT NULL;
                ALTER TABLE GrupoLancamento RENAME INDEX IX_GrupoLancamento_GrupoLancamentoId TO IX_GrupoLancamentoIdPai;
            ");
            AddForeignKey("dbo.GrupoLancamento", "GrupoLancamentoIdPai", "dbo.GrupoLancamento", "GrupoLancamentoId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.GrupoLancamento", name: "IX_GrupoLancamentoIdPai", newName: "IX_GrupoLancamento_GrupoLancamentoId");
            RenameColumn(table: "dbo.GrupoLancamento", name: "GrupoLancamentoIdPai", newName: "GrupoLancamento_GrupoLancamentoId");
            AddColumn("dbo.GrupoLancamento", "GrupoLancamentoIdPai", c => c.Guid());
        }
    }
}
