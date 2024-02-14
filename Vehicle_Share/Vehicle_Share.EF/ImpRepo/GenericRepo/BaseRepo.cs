using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.EF.Data;

namespace Vehicle_Share.EF.ImpRepo.GenericRepo
{
    public class BaseRepo <T> : IBaseRepo<T> where T : class
    {
        public ApplicationDbContext _context { get; set; }
        private readonly DbSet<T> _dbSet;
        public BaseRepo(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<List<T>> GetAll() 
        {
            return _dbSet.ToList();
        }
       public async  Task<T> GetById(string id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<T> AddAsync(T entity) 
        {
             await _context.Set<T>().AddAsync(entity);
             await _context.SaveChangesAsync();
            return entity;
        }
       public async  Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
           _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<byte[]> GetImageAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null)
            {
                var imageData = (byte[])entity.GetType().GetProperty("Images").GetValue(entity);
                return imageData;
            }

            return null;
        }

        public async Task<Guid> UploadImageAsync(Guid id, byte[] imageData)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null)
            {
                entity.GetType().GetProperty("Images").SetValue(entity, imageData);
                await _context.SaveChangesAsync();
                return id;
            }

            return Guid.Empty; // Return an empty GUID or throw an exception if the entity is not found
        }
    }

}

