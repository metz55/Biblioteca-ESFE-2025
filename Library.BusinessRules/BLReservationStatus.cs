using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLReservationStatus
    {
        public async Task<int> CreateReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.CreateReservationStatusAsync(pReservationStatus);
        }

        public async Task<int> UpdateReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.UpdateReservationStatusAsync(pReservationStatus);
        }

        public async Task<int> DeleteReservationStatusAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.DeleteReservationStatusAsync(pReservationStatus);
        }

        public async Task<List<ReservationStatus>> GetAllReservationStatusAsync()
        {
            return await DALReservationStatus.GetAllReservationStatusAsync();
        }

        public async Task<ReservationStatus> GetReservationStatusByIdAsync(ReservationStatus pReservationStatus)
        {
            return await DALReservationStatus.GetReservationStatusByIdAsync(pReservationStatus);
        }

        public async Task<List<ReservationStatus>> GetReservationStatusAsync(ReservationStatus pReservationStatus)
        {

            return await DALReservationStatus.GetReservationStatusAsync(pReservationStatus);
        }

    }
}
