using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace library.Controllers;

// Answers every address that starts with /Loans.
public class LoansController : Controller
{
    private readonly LoanRepository _loans;
    private readonly MemberRepository _members;
    private readonly BookRepository _books;

    public LoansController(
        LoanRepository loans,
        MemberRepository members,
        BookRepository books)
    {
        _loans = loans;
        _members = members;
        _books = books;
    }

    // GET /Loans
    public async Task<IActionResult> Index()
    {
        var loans = await _loans.GetAllAsync();

        return View(loans);
    }

    // Fills the Borrow form's two dropdowns.
    private async Task FillDropdownsAsync()
    {
        ViewBag.Members = new SelectList(
            await _members.GetAllAsync(),
            "MemberId",
            "FullName"
        );

        ViewBag.Books = new SelectList(
            await _books.GetAllAsync(),
            "BookId",
            "Title"
        );
    }

    // GET /Loans/Borrow
    public async Task<IActionResult> Borrow()
    {
        await FillDropdownsAsync();

        return View(new BorrowForm());
    }


    // POST /Loans/Borrow
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(BorrowForm form)
    {
        // 1. Check if the form is valid.
        if (ModelState.IsValid)
        {
            // 2. Try to borrow the book.
            bool saved = await _loans.BorrowAsync(
                form.MemberId!.Value,
                form.BookId!.Value,
                form.DaysAllowed
            );

            // 3. Check if the loan was saved.
            if (saved)
            {
                // 4. Return to the Loans list.
                return RedirectToAction(nameof(Index));
            }

            // 5. Explain why nothing was saved.
            ModelState.AddModelError(
                "",
                $"This member already has {LoanRepository.MaxOpenLoans} books out. Nothing was saved."
            );
        }

        // 6. Fill both dropdowns again.
        await FillDropdownsAsync();

        // 7. Show the form again.
        return View(form);
    }

}