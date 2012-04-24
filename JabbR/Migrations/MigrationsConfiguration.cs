using System.Data.Entity.Migrations;
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
