using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data.Models;

namespace ReservationSystem.Data.Services
{
    public class ReservationService
    {
        private ApplicationDbContext _dbContext;
        private IHttpContextAccessor _httpContext;
        private UserService _userService;
        
        public ReservationService(ApplicationDbContext dbContext, IHttpContextAccessor httpContext, UserService userService)
        {
            _dbContext = dbContext;
            _httpContext = httpContext;
            _userService = userService;
        }

        public List<Reservation> GetUserReservations()
        {
            var user = _userService.GetCurrentUser().Result;
            var userReservations =
                _dbContext.AppUsers.Include(u => u.Reservations).ThenInclude(r => r.Seats).Include(u=> u.Reservations).ThenInclude(r => r.Event).FirstOrDefault(u => u.Id == user.Id);
            return userReservations.Reservations.ToList();
        }

        public void DeleteReservation(Reservation reservation)
        {
            _dbContext.Reservations.Remove(reservation);
            _dbContext.SaveChanges();
        }
        public Reservation GetReservation(int id)
        {
            return _dbContext.Reservations.Include(r => r.Seats).Include(r => r.Auditorium).Include(r => r.Event).FirstOrDefault(r => r.Id == id);
        }
        public Reservation CreateReservationAsync(Reservation reservation)
        {
            reservation.IpAdress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
            var user = _userService.GetCurrentUser().Result;
            if (user != null)
            {
                reservation.User = user;
                reservation.UserEmail = user.Email;
                reservation.PhoneNumber = user.PhoneNumber;
            }

            if (VerifySeats(reservation))
            {
                var result = _dbContext.Reservations.Add(reservation);
                _dbContext.SaveChanges();
                return result.Entity;
            }
            else
            {
                return null;
            }
        }

        
        
        public bool VerifyUserReservationSeatsCount(Reservation reservation)
        {
            var user = _userService.GetCurrentUser().Result;

            if (user != null)
            {
                int currentSeatCount = GetUserReservationSeatsCount(reservation);
                var roles = _userService.GetUserRoles(user);
                if (roles.Count == 0)
                {

                    var totalSeats = currentSeatCount + reservation.Seats.Count;
                    if (totalSeats > 20)
                    {
                        return false;
                    }
                    else return true;
                }
                else return true;
            }
            else
            {
                var currentIpSeatsCount = GetUserReservationSeatsCount(reservation);
                var ipSeatsCount = currentIpSeatsCount + reservation.Seats.Count;
                if (ipSeatsCount > 6)
                {
                    return false;
                }
                else return true;
            }
        }

        private int GetUserReservationSeatsCount(Reservation reservation)
        {
            var user = _userService.GetCurrentUser().Result;
            if (user != null)
            {
                var userReservations = _dbContext.AppUsers
                    .Include(u => u.Reservations).ThenInclude(r => r.Seats)
                    .Include(u => u.Reservations).ThenInclude(r => r.Event)
                    .FirstOrDefault(u => user.Id == u.Id)
                    .Reservations.Where(r => r.Event == reservation.Event).ToList();
                int userEventSeatsCount = 0;

                if (userReservations != null)
                {
                    foreach (var r in userReservations)
                    {
                        userEventSeatsCount += r.Seats.Count;
                    }

                    return userEventSeatsCount;
                }
                else return 0;
            }
            else
            {
                var ipEventSeatsCount = 0;
                var ipAdress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
                var ipReservations = _dbContext.Reservations
                    .Include(r => r.Seats)
                    .Include(r => r.Event)
                    .Where(r => r.Event == reservation.Event).Where(r => r.IpAdress == ipAdress).ToList();

                if (ipReservations.Count > 0)
                {
                    foreach (var res in ipReservations)
                    {
                        ipEventSeatsCount += res.Seats.Count;
                    }
                    return ipEventSeatsCount;
                }
                else
                {
                    return 0;
                }
            }
        }
        public bool VerifySeats(Reservation reservation)
        {
            var seatsToVerify = reservation.Seats;
            foreach (var seat in seatsToVerify)
            {
                var s = _dbContext.Seats.Include(s => s.Reservation).FirstOrDefault(s => s.Id == seat.Id);
                _dbContext.Entry(s).Reload();
                var se = _dbContext.Seats.Include(s => s.Reservation).FirstOrDefault(s => s.Id == seat.Id);
                if (se.Reservation != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}