namespace JabbR.Models.Migrations
{
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class RemoveEdmMetadata : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201204232322474_Test"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "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"; }
        }
    }
}
