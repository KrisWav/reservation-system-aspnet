using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Data.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime HappeningDate { get; set; }
        public string Organiser { get; set; }
        public string Venue { get; set; }
        public ICollection<Auditorium> Auditoriums { get; set; }
    }
}