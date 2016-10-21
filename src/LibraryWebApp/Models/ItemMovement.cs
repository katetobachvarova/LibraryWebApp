using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models
{
    public class ItemMovement
    {
        public int ItemMovementId { get; set; }

        public DateTime Date { get; set; }
        public string Condition { get; set; }

        public string MovementType { get; set; }

        public DateTime Deadline { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }

        public int LibrarianId { get; set; }
        public ApplicationUser Librarian { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
