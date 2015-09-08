namespace WebClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Users");
            AlterColumn("dbo.Users", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Users", "Id");
        }
    }
}
