using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ReservationSystem.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
        
    }
}