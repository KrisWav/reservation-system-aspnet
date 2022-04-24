using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data.Models;

namespace ReservationSystem.Data.Services
{
    public class EventService
    {
        private ApplicationDbContext _dbContext;
        public EventService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Event CreateEvent(Event e, List<Auditorium> auditoriums)
        {
            List<Auditorium> auditoria = new List<Auditorium>();
            foreach (var auditorium in auditoriums)
            {
                var a = CreateAuditorium(auditorium, e, 1000);
                auditoria.Add(a);
            }

            e.Auditoriums = auditoria;
            var eEntity = _dbContext.Events.Add(e).Entity;
            _dbContext.SaveChanges();
            return eEntity;
        }
        private Auditorium CreateAuditorium(Auditorium auditorium, Event e, int price)
        {
            List<Seat> seats = new List<Seat>();
            
            for (int j = 1; j < auditorium.Rows+1; j++)
            {
                for (int i = 1; i < auditorium.SeatsInRow+1; i++)
                {
                    var seat = new Seat()
                    {
                        Event = e,
                        Auditorium = auditorium,
                        Row = j,
                        Column = i,
                        Type = SeatType.Regular
                    };
                    seats.Add(seat);
                    _dbContext.Seats.Add(seat);
                }
            }

            auditorium.Seats = seats;
            var aEntity = _dbContext.Auditoriums.Add(auditorium).Entity;
            return aEntity;
        }

        public List<Event> GetEvents()
        {
            return _dbContext.Events.ToList();
        }

        public Event GetEventById(int eventId)
        {
            return _dbContext.Events.Include(e => e.Auditoriums).FirstOrDefault(e => (eventId == e.Id));
        }

        public Auditorium GetAuditoriumById(int auditoriumId)
        {
            return _dbContext.Auditoriums.Include(a => a.Seats).ThenInclude(s => s.Reservation).FirstOrDefault(a => a.Id == auditoriumId);
        }

        
        
        
        //MRDKA
        public Auditorium DeleteAuditorium(Auditorium auditorium)
        {
            var a = _dbContext.Auditoriums.Include(a=> a.Seats).FirstOrDefault(a => a.Id == auditorium.Id);
            var entity = _dbContext.Auditoriums.Remove(a).Entity;
            foreach (var seat in entity.Seats)
            {
                DeleteSeat(seat);
            }
            _dbContext.SaveChanges();
            return entity;
        }

        public Seat DeleteSeat(Seat seat)
        {
            return _dbContext.Seats.Remove(_dbContext.Seats.FirstOrDefault(s => s.Id == seat.Id)).Entity;
        }

        public void DeleteReservation(Reservation reservation)
        {
            if (reservation.User != null)
            {
                var user = _dbContext.AppUsers.Include(u => u.Reservations).FirstOrDefault(u => u.Id == reservation.User.Id);
                user.Reservations.Remove(reservation);
                _dbContext.Users.Update(user);
            }
            _dbContext.Reservations.Remove(reservation);
            _dbContext.SaveChanges();
        }
        public async Task DeleteEvent(Event _event)
        {
            var ev = _dbContext.Events.Include(e => e.Auditoriums).FirstOrDefault(e => e.Id == _event.Id);
            var reservations = _dbContext.Reservations.Include(r => r.Event).Include(r => r.User).Where(r => r.Event.Id == ev.Id);
            foreach (var reservation in reservations)
            {
                DeleteReservation(reservation);
            }
            foreach (var auditorium in ev.Auditoriums)
            {
                DeleteAuditorium(auditorium);
            }
            _dbContext.Events.Remove(ev);
            _dbContext.SaveChanges();
        }

    }
}