using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using JabbR.Models;

namespace JabbR.Migrations
{
    public class MigrationsConfiguration : DbMigrationsConfiguration<JabbrContext>
    {
        public MigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}
