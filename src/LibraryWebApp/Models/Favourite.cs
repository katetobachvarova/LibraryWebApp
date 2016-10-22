using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models
{
    public class Favourite
    {
        public int FavouriteId { get; set; }
        public string Url { get; set; }
        public string Comment { get; set; }
        public string ItemIndex { get; set; }


        //public int ItemId { get; set; }
        //public Item Item { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }



    }
}
