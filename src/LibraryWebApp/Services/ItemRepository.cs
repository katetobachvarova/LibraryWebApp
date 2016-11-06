using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebApp.Data
{
    public class ItemRepository : IItemRepository
    {

        private ApplicationDbContext context = null;


        public ItemRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        
        public async Task Delete(string id)
        {
            Item existing = await context.Items.SingleOrDefaultAsync(i => i.ItemId.ToString() == id);
            context.Items.Remove(existing);
        }

        public void Insert(Item obj)
        {
            context.Items.Add(obj);
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Item>> SelectAll()
        {
            var allItems = await context.Items.Include(i => i.Title).Include(e => e.Title.Section).ToListAsync();
            return allItems.AsEnumerable();
        }

        public async Task<Item> SelectByID(string id)
        {
            var item = await context.Items.Include(i => i.Title).SingleOrDefaultAsync(m => m.ItemId.ToString() == id);
            return item;
            
        }

        public async Task<Item> SelectByIDAsNoTracking(string id)
        {
            var item = await context.Items.AsNoTracking().Include(i => i.Title).SingleOrDefaultAsync(m => m.ItemId.ToString() == id);
            return item;
        }

        public void Update(Item obj)
        {
            throw new NotImplementedException();
        }
        //bug git
    }
}
