namespace JabbR.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemoteIPToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatClients", "RemoteIP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatClients", "RemoteIP");
        }
    }
}
