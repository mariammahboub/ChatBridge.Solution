using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IArchiveService
    {
        Task<bool> ArchiveChatAsync(int groupId);
        Task<bool> ArchiveGroupAsync(int groupId);

    }
}
