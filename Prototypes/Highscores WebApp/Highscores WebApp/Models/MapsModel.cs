using System.ComponentModel.DataAnnotations;

namespace Highscores_WebApp.Models
{
	public class MapsModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }



	}
}
