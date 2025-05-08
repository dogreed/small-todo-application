using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using small_todo_application.Data;
using small_todo_application.Models;
using small_todo_application.ViewModel;

namespace small_todo_application.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
	private readonly AppDbContext _context;

	public HomeController(ILogger<HomeController> logger, AppDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

	


	public IActionResult Blog(int page = 1)
	{
		int pageSize = 3; // Or make it configurable
		var posts = _context.BlogPosts
			.OrderByDescending(p => p.CreatedAt);

		var paginatedPosts = PaginatedList<BlogPost>.Create(posts, page, pageSize);

		return View(paginatedPosts);
	}

	public IActionResult Details(int id)
	{
		var post = _context.BlogPosts.Find(id);
		if (post == null) return NotFound();
		return View(post);
	}


	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
