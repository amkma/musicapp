using System.ComponentModel.DataAnnotations;

namespace MusicApp.Web.Models;

public class Song{
    public int Id {get; set;}

    [Required]
    [StringLength(200)]
    public required string Title{get; set;}

    public int CategoryId {get; set;}   
    public Category Category {get; set;} = null!;

    public int SingerId {get; set;}
    public Singer Singer{get; set;} = null!;
}