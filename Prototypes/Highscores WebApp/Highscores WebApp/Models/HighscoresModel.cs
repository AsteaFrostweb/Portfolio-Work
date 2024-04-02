using System.ComponentModel.DataAnnotations;

namespace Highscores_WebApp.Models
{
	public class HighscoresModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string? Player_Id { get; set; }
		[Required]
		public int Map_Id { get; set; }
		public float Fastest_Lap { get; set; }
		public int Best_Combo_Score { get; set; }
		public float Best_Combo_Time { get; set; }

	}
}
