
using System.ComponentModel.DataAnnotations;

namespace HandsOnEx.Models
{
    public class FavDestination
    {
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(30)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter a destination")]
        [StringLength(50)]
        public string? DestinationName { get; set; }

        [Range(0, 20000, ErrorMessage = "Please enter an amount between 0 and 20000")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Please select a transportaion mode")]
        [StringLength(30)]
        public string? TravelMode { get; set; }
    }
}
