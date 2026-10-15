namespace library.Models;

// One Member object holds one row of lending.member.
public class Member
{
    public long MemberId { get; set; }          // member_id
    public string FullName { get; set; } = "";  // full_name
    public string? Email { get; set; }          // email, may be NULL
    public string MemberType { get; set; } = "";// member_type
}