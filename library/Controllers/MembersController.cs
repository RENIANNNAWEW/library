using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _members;

    public MembersController(MemberRepository members)
    {
        _members = members;
    }

    // Task 13.10: Display the list of members
    public async Task<IActionResult> Index()
    {
        var members = await _members.GetAllAsync();
        return View(members);
    }

    // Task 14.2: Create, GET
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Member());
    }

    // Task 14.3: Create, POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        await _members.AddAsync(member);
        return RedirectToAction(nameof(Index));
    }

    // Task 15.3: Edit, GET
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // Task 15.4: Edit, POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Member member)
    {
        await _members.UpdateAsync(member);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
            return NotFound();

        return View(member);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _members.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}