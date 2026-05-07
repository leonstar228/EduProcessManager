using EduProcessManager.Data;
using EduProcessManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers.Tests;

[Authorize(Roles = "Teacher,Admin")]
public class TestResultsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TestResultsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Results(string? search, string searchBy = "all", string sortOrder = "date_desc")
    {
        var attempts = _db.TestAttempts
            .Include(a => a.Test)
            .Include(a => a.Student)
            .AsQueryable();

        if (User.IsInRole("Teacher"))
        {
            var userId = _userManager.GetUserId(User);
            attempts = attempts.Where(a => a.Test != null && a.Test.AuthorId == userId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();

            int.TryParse(search, out var searchNumber);
            DateTime.TryParse(search, out var searchDate);

            attempts = searchBy switch
            {
                "student" => attempts.Where(a =>
                    a.Student != null &&
                    a.Student.FullName.ToLower().Contains(search)),

                "test" => attempts.Where(a =>
                    a.Test != null &&
                    a.Test.Title.ToLower().Contains(search)),

                "score" => attempts.Where(a =>
                    searchNumber > 0 &&
                    (a.Score == searchNumber || a.MaxScore == searchNumber)),

                "date" => attempts.Where(a =>
                    (searchDate != default && a.FinishedAt.Date == searchDate.Date) ||
                    (searchNumber > 0 &&
                        (a.FinishedAt.Day == searchNumber ||
                         a.FinishedAt.Month == searchNumber ||
                         a.FinishedAt.Year == searchNumber ||
                         a.FinishedAt.Hour == searchNumber ||
                         a.FinishedAt.Minute == searchNumber))),

                _ => attempts.Where(a =>
                    (a.Student != null && a.Student.FullName.ToLower().Contains(search)) ||
                    (a.Test != null && a.Test.Title.ToLower().Contains(search)) ||
                    (searchNumber > 0 && (a.Score == searchNumber || a.MaxScore == searchNumber)) ||
                    (searchDate != default && a.FinishedAt.Date == searchDate.Date) ||
                    (searchNumber > 0 &&
                        (a.FinishedAt.Day == searchNumber ||
                         a.FinishedAt.Month == searchNumber ||
                         a.FinishedAt.Year == searchNumber ||
                         a.FinishedAt.Hour == searchNumber ||
                         a.FinishedAt.Minute == searchNumber)))
            };
        }

        attempts = sortOrder switch
        {
            "student_asc" => attempts.OrderBy(a => a.Student!.FullName),
            "student_desc" => attempts.OrderByDescending(a => a.Student!.FullName),

            "test_asc" => attempts.OrderBy(a => a.Test!.Title),
            "test_desc" => attempts.OrderByDescending(a => a.Test!.Title),

            "score_asc" => attempts.OrderBy(a => a.Score),
            "score_desc" => attempts.OrderByDescending(a => a.Score),

            "date_asc" => attempts.OrderBy(a => a.FinishedAt),
            _ => attempts.OrderByDescending(a => a.FinishedAt)
        };

        ViewBag.Search = search;
        ViewBag.SearchBy = searchBy;
        ViewBag.SortOrder = sortOrder;

        return View(await attempts.ToListAsync());
    }
}