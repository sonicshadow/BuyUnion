namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShop : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Shops");
        }
    }
}
