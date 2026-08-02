using System.ComponentModel.DataAnnotations;
namespace MusicApp.Web.Models;

public class Category
{
    
    public int Id {get; set;}
    [Required]
    [StringLength(100)]
    public string Name {get; set;} = null!;
    public ICollection<Song> Songs{get; set;} = new List<Song>();
}