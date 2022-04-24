using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ReservationSystem.Data.Models
{
    public class Reservation
    {
        [Key] public int Id { get; set; }
        public Event Event { get; set; }
        public Auditorium Auditorium { get; set; }
        public ReservationType Type { get; set; }
        public AppUser User { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public string UserEmail { get; set; }
        public string IpAdress { get; set; }
    }
}