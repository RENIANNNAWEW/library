using System;
using System.ComponentModel.DataAnnotations;

namespace library.Models
{
    public class Note
    {
        public long NoteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Body { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}