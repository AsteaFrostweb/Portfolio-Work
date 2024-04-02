using Highscores_WebApp.Data;
using Highscores_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class HighscoresController : Controller
{
	private readonly ApplicationDbContext _context;

	public HighscoresController(ApplicationDbContext context)
	{
		_context = context;
	}

    [AllowAnonymous]
    public IActionResult Index(string sortOrder)
    {
        // Retrieve highscores, maps, and users
        var highscores = _context.Highscores.ToList();

        // Sorting logic based on sortOrder parameter
        switch (sortOrder)
        {
            case "FastestLap_ASC":
                highscores = highscores.OrderBy(h => h.Fastest_Lap).ToList();
                break;
            case "FastestLap_DESC":
                highscores = highscores.OrderByDescending(h => h.Fastest_Lap).ToList();
                break;
            case "BestComboScore_ASC":
                highscores = highscores.OrderBy(h => h.Best_Combo_Score).ToList();
                break;
            case "BestComboScore_DESC":
                highscores = highscores.OrderByDescending(h => h.Best_Combo_Score).ToList();
                break;
            case "BestComboTime_ASC":
                highscores = highscores.OrderBy(h => h.Best_Combo_Time).ToList();
                break;
            case "BestComboTime_DESC":
                highscores = highscores.OrderByDescending(h => h.Best_Combo_Time).ToList();
                break;
            default:
                // Default sorting criteria if sortOrder parameter is not specified
                highscores = highscores.OrderBy(h => h.Fastest_Lap).ToList();
                break;
        }

        var viewModel = new HighscoresViewModel
        {
            Highscores = highscores,
            Maps = _context.Maps.ToList(),
            Users = _context.Users.ToList()
        };

        return View(viewModel); // Pass HighscoresViewModel instance to the view
    }

    [HttpGet]
	public IActionResult GetHighScores()
	{
		
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);		

		var userHighScores = _context.Highscores.Where(h => h.Player_Id == userId).ToList();
		return Ok(userHighScores);
	}

    [Authorize]
    [HttpPost]
    public IActionResult UpdateHighScore(HighscoreUpdateModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Check if the user has an existing high score for the specified map
        var existingHighScore = _context.Highscores.FirstOrDefault(h => h.Player_Id == userId && h.Map_Id == model.Map_Id);

        if (existingHighScore == null)
        {
            // If no existing high score found, create a new one
            var newHighScore = new HighscoresModel
            {
                Player_Id = userId,
                Map_Id = model.Map_Id,
                Fastest_Lap = model.FastestLap,
                Best_Combo_Score = model.BestComboScore,
                Best_Combo_Time = model.BestComboTime
            };

            _context.Highscores.Add(newHighScore);
        }
        else
        {
            // If an existing high score found, update its values

            existingHighScore.Fastest_Lap = model.FastestLap;
            existingHighScore.Best_Combo_Score = model.BestComboScore;
            existingHighScore.Best_Combo_Time = model.BestComboTime;
        }

        _context.SaveChanges();

        return Ok();
    }

}