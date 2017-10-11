namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderAddPhoneNumberAndConsignee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PhoneNumber", c => c.String());
            AddColumn("dbo.Orders", "Consignee", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Consignee");
            DropColumn("dbo.Orders", "PhoneNumber");
        }
    }
}
