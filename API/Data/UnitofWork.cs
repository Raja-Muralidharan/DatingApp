using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;

namespace API.Data
{
    public class UnitofWork(DataContext context, IUserRepository UserRepository, 
    ILikesRepository LikesRepository, IMessageRepository MessageRepository) : IUnitofWork
    {
        public IUserRepository userRepository => UserRepository;

        public IMessageRepository messageRepository => MessageRepository;

        public ILikesRepository likesRepository => LikesRepository;

        public async Task<bool> Complete()
        {
           return await context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}