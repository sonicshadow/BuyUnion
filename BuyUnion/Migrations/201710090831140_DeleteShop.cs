namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteShop : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "ShopID");
            DropColumn("dbo.Products", "ShopID");
            DropColumn("dbo.Products", "ExpressTemplateID");
            DropTable("dbo.ExpressTemplates");
            DropTable("dbo.Shops");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Shops",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        Logo = c.String(),
                        Address = c.String(),
                        Info = c.String(),
                        Contact = c.String(),
                        OwnID = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ExpressTemplates",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ShopID = c.String(),
                        Group = c.String(),
                        Provinces = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        First = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Additional = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Products", "ExpressTemplateID", c => c.Int());
            AddColumn("dbo.Products", "ShopID", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "ShopID", c => c.Int(nullable: false));
        }
    }
}
