namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inicialmysql : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Conta",
                c => new
                    {
                        ContaId = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 100, unicode: false),
                        Ativo = c.Boolean(nullable: false),
                        DataCadastro = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.ContaId);
            
            CreateTable(
                "dbo.Lancamento",
                c => new
                    {
                        LancamentoId = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 100, unicode: false),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DataVencimento = c.DateTime(nullable: false, precision: 0),
                        Pago = c.Boolean(nullable: false),
                        ContaId = c.Guid(nullable: false),
                        CategoriaId = c.Guid(nullable: false),
                        LancamentoParceladoId = c.Guid(),
                        IdDaParcelaNaSerie = c.String(maxLength: 100, unicode: false),
                        DataCadastro = c.DateTime(nullable: false, precision: 0),
                        BaseDaSerie = c.Boolean(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                        TipoDeTransacao = c.Int(nullable: false),
                        LancamentoIdTransferencia = c.Guid(),
                    })
                .PrimaryKey(t => t.LancamentoId)
                .ForeignKey("dbo.Categoria", t => t.CategoriaId)
                .ForeignKey("dbo.LancamentoParcelado", t => t.LancamentoParceladoId)
                .ForeignKey("dbo.Conta", t => t.ContaId)
                .Index(t => t.ContaId)
                .Index(t => t.CategoriaId)
                .Index(t => t.LancamentoParceladoId)
                .Index(t => t.IdDaParcelaNaSerie, unique: true)
                .Index(t => t.LancamentoIdTransferencia, unique: true);
            
            CreateTable(
                "dbo.Categoria",
                c => new
                    {
                        CategoriaId = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 100, unicode: false),
                        Cor = c.String(nullable: false, maxLength: 100, unicode: false),
                        DataCadastro = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.CategoriaId);
            
            CreateTable(
                "dbo.LancamentoParcelado",
                c => new
                    {
                        LancamentoParceladoId = c.Guid(nullable: false),
                        NumeroParcelas = c.Int(),
                        Periodicidade = c.Int(nullable: false),
                        DataInicio = c.DateTime(nullable: false, precision: 0),
                        DataCadastro = c.DateTime(nullable: false, precision: 0),
                        LancamentoBaseId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.LancamentoParceladoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lancamento", "ContaId", "dbo.Conta");
            DropForeignKey("dbo.Lancamento", "LancamentoParceladoId", "dbo.LancamentoParcelado");
            DropForeignKey("dbo.Lancamento", "CategoriaId", "dbo.Categoria");
            DropIndex("dbo.Lancamento", new[] { "LancamentoIdTransferencia" });
            DropIndex("dbo.Lancamento", new[] { "IdDaParcelaNaSerie" });
            DropIndex("dbo.Lancamento", new[] { "LancamentoParceladoId" });
            DropIndex("dbo.Lancamento", new[] { "CategoriaId" });
            DropIndex("dbo.Lancamento", new[] { "ContaId" });
            DropTable("dbo.LancamentoParcelado");
            DropTable("dbo.Categoria");
            DropTable("dbo.Lancamento");
            DropTable("dbo.Conta");
        }
    }
}
