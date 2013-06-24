using System;
using System.ComponentModel.DataAnnotations;

namespace JabbR.Models
{
    public class BannedIP
    {
        [Key]
        public int Key { get; set; }
        
        public DateTimeOffset When { get; set; }
        public string RemoteIP { get; set; }
    }
}