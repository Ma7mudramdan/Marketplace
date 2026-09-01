

using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {

        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;
        private IDbContextTransaction? _transaction;
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); 
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public void Delete(T entity)
        {
             _dbSet.Remove(entity);
             _context.SaveChanges();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            return entity != null;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return  await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
           return await _dbSet.Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }

    

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }


        public async Task<IDisposable> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
