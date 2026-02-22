using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites.Booking;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly RepairDbContext _context;

        public BookingRepository(RepairDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Booking> GetQueryableForList()
        {
            return _context.Bookings
                .Where(b => !b.IsDeleted)
                .Include(b => b.StatusNavigation);
        }

        public async Task<Booking?> GetByIdWithStatusAsync(long id)
        {
            return await _context.Bookings
                .Where(b => b.Id == id && !b.IsDeleted)
                .Include(b => b.StatusNavigation)
                .FirstOrDefaultAsync();
        }

        public async Task<long> GetMaxIdAsync()
        {
            return await _context.Bookings
                .Where(b => !b.IsDeleted)
                .MaxAsync(b => (long?)b.Id) ?? 0;
        }
    }
}
