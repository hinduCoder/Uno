namespace WebClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmail : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.__MigrationHistory",
            //    c => new
            //        {
            //            MigrationId = c.String(nullable: false, maxLength: 150),
            //            ContextKey = c.String(nullable: false, maxLength: 300),
            //            Model = c.Binary(nullable: false),
            //            ProductVersion = c.String(nullable: false, maxLength: 32),
            //        })
            //    .PrimaryKey(t => new { t.MigrationId, t.ContextKey });
            
            //CreateTable(
            //    "dbo.User",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false),
            //            Username = c.String(nullable: false, maxLength: 100, unicode: false),
            //            Password = c.String(nullable: false, maxLength: 200, unicode: false),
            //            Email = c.String(nullable: false, maxLength: 100),
            //        })
            //    .PrimaryKey(t => t.Id);
            AddColumn("dbo.User", "email", builder => builder.String(false, 110));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "email");
        }
    }
}
