using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models.LibraryViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }

        public ItemViewModel(Item item)
        {
            Item = item;
        }

        public ItemViewModel()
        {
            Item = new Item();
        }
    }
}
