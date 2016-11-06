using LibraryWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Data
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>>SelectAll();
        Task<Item> SelectByID(string id);
        Task<Item> SelectByIDAsNoTracking(string id);
        void Insert(Item obj);
        void Update(Item obj);
        Task Delete(string id);
        Task Save();
        //bug git
    }
}
