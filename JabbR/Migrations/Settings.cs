using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using JabbR.Models;

namespace JabbR.Migrations
{
    public class Settings : DbMigrationsConfiguration<JabbrContext>
    {
        public Settings()
        {
            AutomaticMigrationsEnabled = false;
            
            CodeGenerator = new CSharpMigrationCodeGenerator();
            //SetSqlGenerator(, new SqlServerMigrationSqlGenerator());
        }
    }
}
