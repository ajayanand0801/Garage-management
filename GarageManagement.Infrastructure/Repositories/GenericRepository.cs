using GarageManagement.Application.Interfaces;
using GarageManagement.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly RepairDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(RepairDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? queryFunc = null)
        {
            IQueryable<T> query = _dbSet;

            if (queryFunc != null)
                query = queryFunc(query);

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>>? queryFunc = null)
        {
            IQueryable<T> query = _dbSet;

            if (queryFunc != null)
                query = queryFunc(query);

            // Assuming PK property is "Id" of type long
            return await query.FirstOrDefaultAsync(e => EF.Property<long>(e, "Id") == id);
        }

        public async Task<bool> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle specific database update issues (e.g., constraint violations)
                // Log the error if you have a logging setup
                Console.Error.WriteLine($"Database update exception: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");
                throw; // Optional: rethrow or wrap depending on your error flow
            }
        }

        //public async Task<bool> AddAsync(T entity)
        //{

        //    await _dbSet.AddAsync(entity);
        //    return await _context.SaveChangesAsync() > 0;
        //}

        public async Task<bool> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddTransactionAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            // Don't save here — defer to UnitOfWork
            return true;
        }
    }


}
