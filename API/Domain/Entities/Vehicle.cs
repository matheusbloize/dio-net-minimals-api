using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dio_net_minimals_api.Domain.Entities;

public class Vehicle {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Make { get; set; } = default!;

    [Required]
    public int Year { get; set; } = default!;
}