
using System.ComponentModel.DataAnnotations;

namespace library.Models;

// One Member object holds one row of lending.member.
public class Member
{
    public long MemberId { get; set; }

    // Name must not exceed 80 characters.
    [StringLength(80, ErrorMessage = "The name can be at most 80 characters.")]

    // Name must follow Lastname, Firstname format.
    [RegularExpression(
        @"^[A-Za-zÑñ .'-]+, [A-Za-zÑñ .'-]+$",
        ErrorMessage = "Write the name as Lastname, Firstname."
    )]
    public string FullName { get; set; } = "";

    // Email must not exceed 120 characters.
    [StringLength(120, ErrorMessage = "The email can be at most 120 characters.")]

    // Email must have a valid format when provided.
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$",
        ErrorMessage = "Enter an email such as ana@ubaguio.edu."
    )]
    public string? Email { get; set; }

    public string MemberType { get; set; } = "";
}
