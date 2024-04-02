using System.ComponentModel.DataAnnotations;

namespace Highscores_WebApp.Models
{
    public class HighscoreUpdateModel
    {
       
        public int Player_Id { get; set; }
        public int Map_Id { get; set; }
        public float FastestLap { get; set; }
        public int BestComboScore { get; set; }
        public float BestComboTime { get; set; }
    }
}
