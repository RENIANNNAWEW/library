
using Npgsql;
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

    // Display the list of members
    public async Task<IActionResult> Index()
    {
        var members = await _members.GetAllAsync();
        return View(members);
    }

    // Create, GET
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Member());
    }

    // Create, POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        await _members.AddAsync(member);
        return RedirectToAction(nameof(Index));
    }

    // Edit, GET
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

    // Edit, POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Member member)
    {
        await _members.UpdateAsync(member);
        return RedirectToAction(nameof(Index));
    }

    // Delete, GET
    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // Delete, POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            await _members.DeleteAsync(id);
        }
        catch (PostgresException ex) when (
            ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            var member = await _members.GetByIdAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            ViewBag.Error = "This member has loans on record, so the database will not delete them.";

            return View("Delete", member);
        }

        return RedirectToAction(nameof(Index));
    }
}
