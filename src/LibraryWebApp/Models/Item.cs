using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Condition { get; set; }
        public string Material { get; set; }
        public string CurrentLocation { get; set; }



        public int TitleId { get; set; }
        public Title Title { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


    }
}
