using System.ComponentModel.DataAnnotations;

namespace GreenFlux_V15.Models
{
    public class Connector
    {
        public int Id { get; set; }

        [Required]
        public int ChargeStationId { get; set; }
        public ChargeStation ChargeStation { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Connector Identifier must be between 1 and 5.")]
        public int Identifier { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Max current must be greater than zero.")]
        public int MaxCurrentInAmps { get; set; }
    }
}
