using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models
{
    public class Section
    {
        public int SectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Title> Titles { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


    }
}
