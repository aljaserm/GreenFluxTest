using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GreenFlux_V15.Models
{
    public class ChargeStation
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public List<Connector> Connectors { get; set; }
    }
}
