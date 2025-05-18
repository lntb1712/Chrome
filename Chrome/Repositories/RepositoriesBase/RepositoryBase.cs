using Chrome.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Repositories.RepositoryBase
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly ChromeContext _context;

        public RepositoryBase(ChromeContext context)
        {
            _context = context;
        }

        public async Task AddAsync (T entity, bool saveChanges = true)
        {
            await _context.Set<T>().AddAsync(entity);
            if (saveChanges == true)
            {
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddRangeAsync(IEnumerable<T> entities, bool saveChanges = true)
        {
            if (entities == null || !entities.Any())
            {
                return;
            }

            await _context.AddRangeAsync(entities);
            if (saveChanges == true)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync<TId>(TId id, bool saveChanges = true)
        {
            var entity = await GetByIDAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                if (saveChanges == true)
                {
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteTwoKeyAsync<TId,Tkey>(TId id,Tkey tkey, bool saveChanges=true)
        {
            var entity =await GetTwoKeyAsync(id,tkey);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                if (saveChanges == true)
                {
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteThreeKeyAsync<TId1, TId2, TId3>(TId1 id1, TId2 id2, TId3 id3, bool saveChanges = true)
        {
            var entity = await GetThreeKeyAsync(id1, id2, id3);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                if (saveChanges)
                {
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteFirstByConditionAsync(Expression<Func<T, bool>> predicate, bool saveChanges = true)
        {
            // Tìm bản ghi đầu tiên thỏa mãn điều kiện
            var entity = await _context.Set<T>().FirstOrDefaultAsync(predicate);

            if (entity != null)
            {
                // Xóa bản ghi
                _context.Set<T>().Remove(entity);

                if (saveChanges)
                {
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync() ?? null!;
        }

        public async Task<List<T>> FindListAsync(Expression<Func<T,bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIDAsync<TId>(TId id)
        {
            return await _context.Set<T>().FindAsync(id)??null!;
        }

        public async Task<T> GetTwoKeyAsync<TId1, TId2>(TId1 id, TId2 key)
        {
            return await _context.Set<T>().FindAsync(id, key) ?? null!;
        }

        public async Task<T> GetThreeKeyAsync<TId1, TId2, TId3>(TId1 id1, TId2 id2, TId3 id3)
        {
            return await _context.Set<T>().FindAsync(id1, id2, id3) ?? null!;
        }

        public async Task UpdateAsync(T entity, bool saveChanges = true)
        {
            _context.Set<T>().Update(entity);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }
       
    }
}
