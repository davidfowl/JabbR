using System.Data.Entity.ModelConfiguration;

namespace JabbR.Models.Mapping
{
    public class BannedIPMap : EntityTypeConfiguration<BannedIP>
    {
        public BannedIPMap()
        {
            // Primary Key
            this.HasKey(m => m.Key);

            // Properties
            // Table & Column Mappings
            this.ToTable("BannedIPs");
            this.Property(m => m.Key).HasColumnName("Key");
            this.Property(m => m.When).HasColumnName("When");
            this.Property(m => m.RemoteIP).HasColumnName("RemoteIP");
        }
    }
}