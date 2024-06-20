using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ArchiveService : IArchiveService
    {
        #region Constructor and Dependencies

        private readonly IChatRepository _groupRepository;

        public ArchiveService(IChatRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        #endregion

        #region Archive Chat

        public async Task<bool> ArchiveChatAsync(int ChatId)
        {
            return await _groupRepository.ArchiveChatAsync(ChatId);
        }

        #endregion

        #region Archive Group

        public async Task<bool> ArchiveGroupAsync(int groupId)
        {
            return await _groupRepository.ArchiveGroupAsync(groupId);
        }

        #endregion
    }
}
