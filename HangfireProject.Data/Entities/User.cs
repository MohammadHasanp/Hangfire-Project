using System.ComponentModel.DataAnnotations;

namespace HangfireProject.Data.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Family { get; set; } = null!;

    [Required]
    [MinLength(4, ErrorMessage = "کلمه عبور بابد بیشتر از 4 کاراکتر باشد")]
    public string Password { get; set; } = null!;
}