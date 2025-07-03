using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories
{
    public class DALReservationStatus
    {

        #region CRUD
        public static async Task<int> CreateReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pReservationStatus);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var reservationStatus = await dbContext.Reservation_Status.FirstOrDefaultAsync(s => s.RESERVATION_ID == pReservationStatus.RESERVATION_ID);
                reservationStatus.STATUS_NAME = pReservationStatus.STATUS_NAME;
                dbContext.Update(reservationStatus);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var reservationStatus = await dbContext.Reservation_Status.FirstOrDefaultAsync(s => s.RESERVATION_ID == pReservationStatus.RESERVATION_ID);
                dbContext.Reservation_Status.Remove(reservationStatus);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<ReservationStatus>> GetAllReservationStatusAsync()
        {
            var reservationStatus = new List<ReservationStatus>();
            using (var dbContext = new DBContext())
            {
                reservationStatus = await dbContext.Reservation_Status.ToListAsync();
            }
            return reservationStatus;
        }

        public static async Task<ReservationStatus> GetReservationStatusByIdAsync(ReservationStatus pReservationStatus)
        {
            var reservationStatus = new ReservationStatus();
            using (var dbContext = new DBContext())
            {
                reservationStatus = await dbContext.Reservation_Status.FirstOrDefaultAsync(s => s.RESERVATION_ID == pReservationStatus.RESERVATION_ID);
            }
            return reservationStatus;
        }

        internal static IQueryable<ReservationStatus> QuerySelect(IQueryable<ReservationStatus> pQuery, ReservationStatus pReservationStatus)
        {
            if (pReservationStatus.RESERVATION_ID > 0)
                pQuery = pQuery.Where(s => s.RESERVATION_ID == pReservationStatus.RESERVATION_ID);
            if (!string.IsNullOrWhiteSpace(pReservationStatus.STATUS_NAME))
                pQuery = pQuery.Where(s => s.STATUS_NAME.Contains(pReservationStatus.STATUS_NAME));
            pQuery = pQuery.OrderByDescending(s => s.RESERVATION_ID).AsQueryable();
            if (pReservationStatus.Top_Aux > 0)
                pQuery = pQuery.Take(pReservationStatus.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<ReservationStatus>> GetReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            var reservationStatus = new List<ReservationStatus>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Reservation_Status.AsQueryable();
                select = QuerySelect(select, pReservationStatus);
                reservationStatus = await select.ToListAsync();
            }
            return reservationStatus;
        }
        #endregion
    }
}
