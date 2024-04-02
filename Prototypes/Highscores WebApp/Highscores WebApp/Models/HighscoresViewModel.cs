using Highscores_WebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace Highscores_WebApp.Models
{
    public class HighscoresViewModel
    {
        public List<HighscoresModel>? Highscores { get; set; }
        public List<IdentityUser>? Users { get; set; }
        public List<MapsModel>? Maps { get; set; }
    }
}