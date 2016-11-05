using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models.LibraryViewModels
{
    public class TitleViewModel
    {
        public Title Title { get; set; }

        public TitleViewModel(Title title)
        {
            Title = title;
        }

        public TitleViewModel()
        {
            Title = new Title();
        }
    }
}
