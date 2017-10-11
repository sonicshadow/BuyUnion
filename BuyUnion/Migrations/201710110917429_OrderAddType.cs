namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderAddType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Type");
        }
    }
}
