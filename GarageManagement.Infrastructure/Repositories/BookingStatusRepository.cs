using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.Booking;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Infrastructure.Repositories
{
    public class BookingStatusRepository : IBookingStatusRepository
    {
        private readonly RepairDbContext _context;

        public BookingStatusRepository(RepairDbContext context)
        {
            _context = context;
        }

        public async Task<BookingStatus?> GetByIdAndActiveAsync(long id)
        {
            return await _context.BookingStatuses
                .Where(s => s.Id == id && s.IsActive && !s.IsDeleted)
                .FirstOrDefaultAsync();
        }
        public async Task<BookingStatus?> GetByIdAndActiveAsync(string name)
        {
            return await _context.BookingStatuses
                .Where(s => s.StatusName.Trim().ToLower() == name && s.IsActive && !s.IsDeleted)
                .FirstOrDefaultAsync();
        }

    }
}
