using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public int Price { get; set; }
        public int SalePrice { get; set; }
        public int VipPrice { get; set; }
        public ICollection<Auditorium> Auditoriums { get; set; }
    }
}