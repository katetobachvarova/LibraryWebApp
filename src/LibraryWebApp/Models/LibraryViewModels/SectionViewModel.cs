using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models.LibraryViewModels
{
    public class SectionViewModel
    {
        public Section Section { get; set; }

        public SectionViewModel(Section section)
        {
            Section = section;
        }

        public SectionViewModel()
        {
            Section = new Section();
        }
    }
}
