using Core.Entities;

namespace Core.Settings
{
    public class BackupSettings:BaseEntity
    {
        public BackupInterval Interval { get; set; }
    }

    public enum BackupInterval
    {
        None,       
        Daily,      
        Weekly,     
        Monthly     
    }
}
