using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models.LibraryViewModels
{
    public class FavouriteViewModel
    {
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        public Favourite Favourite { get; set; }

        public FavouriteViewModel(Favourite favourite)
        {
            Favourite = favourite;
        }

        public FavouriteViewModel()
        {
            Favourite = new Favourite();
        }
    }
}
