
namespace library.Models;

// The member's page shows two things.
public class MemberDetails
{
    public Member Member { get; set; } = new();

    public List<LoanView> Loans { get; set; } = new();
}
