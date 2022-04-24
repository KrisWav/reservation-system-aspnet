using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using ReservationSystem.Data.Models;

namespace ReservationSystem.Data.Containers
{
    public class ReservationContainer
    {
        public List<Seat> Seats { get; set; }
        public Auditorium Auditorium { get; set; }
        public Event Event { get; set; }

        public void ContainReservation(Event _event, Auditorium _auditorium, List<Seat> _seats)
        {
            Seats = _seats;
            Auditorium = _auditorium;
            Event = _event;
        }
    }
}