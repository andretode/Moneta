namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lancamento : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lancamento",
                c => new
                    {
                        LancamentoId = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 100),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DataVencimento = c.DateTime(nullable: false),
                        ContaId = c.Guid(nullable: false),
                        CategoriaId = c.Guid(nullable: false),
                        DataCadastro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LancamentoId)
                .ForeignKey("dbo.Categoria", t => t.CategoriaId)
                .ForeignKey("dbo.Conta", t => t.ContaId)
                .Index(t => t.ContaId)
                .Index(t => t.CategoriaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lancamento", "ContaId", "dbo.Conta");
            DropForeignKey("dbo.Lancamento", "CategoriaId", "dbo.Categoria");
            DropIndex("dbo.Lancamento", new[] { "CategoriaId" });
            DropIndex("dbo.Lancamento", new[] { "ContaId" });
            DropTable("dbo.Lancamento");
        }
    }
}
