using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace ReservationSystem.Data.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        public Event Event { get; set; }
        public Auditorium Auditorium { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public SeatType Type { get; set; }
        public double Price { get; set; }
        public Reservation Reservation { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
}