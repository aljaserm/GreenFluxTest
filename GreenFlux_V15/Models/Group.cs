using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GreenFlux_V15.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")]
        public int CapacityInAmps { get; set; }

        public List<ChargeStation> ChargeStations { get; set; }
    }
}
