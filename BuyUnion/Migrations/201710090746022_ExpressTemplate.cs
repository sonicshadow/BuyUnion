namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpressTemplate : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExpressTemplates");
        }
    }
}
