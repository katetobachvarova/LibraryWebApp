using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models
{
    public class Title
    {
        public int TitleId { get; set; }
        public string Name { get; set; }
        public string Annotation { get; set; }
        public string Author { get; set; }
        public short Year { get; set; }
        public string ISBN { get; set; }
        public string Type { get; set; }

        //TODO
        //public List<Image> Images { get; set; }
        public string Publisher { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }

        public ICollection<Item> Items { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


    }
}
