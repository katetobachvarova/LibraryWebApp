using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models.LibraryViewModels
{
    public class ItemMovementViewModel
    {
        public ItemMovement ItemMovement { get; set; }

        public ItemMovementViewModel(ItemMovement itemMovement)
        {
            ItemMovement = itemMovement;
        }

        public ItemMovementViewModel()
        {
            ItemMovement = new ItemMovement();
        }
    }
}
