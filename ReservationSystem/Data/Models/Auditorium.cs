using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Data.Models
{
    public class Auditorium
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public Event Event { get; set; }
        public int Rows { get; set; }
        public int SeatsInRow { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}