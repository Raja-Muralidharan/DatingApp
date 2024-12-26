using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitofWork
    {
        IUserRepository userRepository { get; } 

        IMessageRepository  messageRepository { get; }

        ILikesRepository likesRepository { get; }   

        Task<bool> Complete();

        bool HasChanges();
    }
}