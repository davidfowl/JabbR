namespace JabbR.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserIsBanned : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatUsers", "IsBanned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatUsers", "IsBanned");
        }
    }
}
