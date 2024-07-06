using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using System;
using System.Linq.Expressions;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.EF.Data;

namespace Vehicle_Share.EF.ImpRepo.GenericRepo
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class
    {
        public ApplicationDbContext _context { get; set; }

        private readonly DbSet<T> _dbSet;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BaseRepo(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<T>> GetAllAsync()
        {
            return _dbSet.ToList();
        }
        public async Task<T> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<int> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<string> UploadImageAsync(string Folder, IFormFile file, string SubFolder )
        {
            var path = _webHostEnvironment.WebRootPath + "/" + Folder + "/" + SubFolder + "/";
            
            // var extention = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + "." + file.FileName;
            if (file.Length > 0)
            {
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, Folder, SubFolder))) {
                    Console.WriteLine($"\n\n========Creating directory to Upload Image at: \"{Path.Combine(_webHostEnvironment.WebRootPath, Folder, SubFolder)}\"=======\n\n");
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, Folder, SubFolder));
                }
                using (FileStream stream = File.Create(path + fileName))
                {
                    await file.CopyToAsync(stream);
                    await stream.FlushAsync();
                    return $"/{Folder}/{SubFolder}/{fileName}";
                }

            }
            else
            {

                return "failed to upload . ";
            }
        }
        public async Task RemoveImageAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            // folder/ file name 
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {

            return await _dbSet.FirstOrDefaultAsync(match);
        }


        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply includes if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply predicate if provided
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }



        /*        public IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector, Func<T, bool> predicate = null)
           {
               var query = _dbSet.AsQueryable();
               if (predicate != null)
               {
                   query = query.Where(predicate).AsQueryable();
               }
               return query.Select(selector).ToList();
           }
   */

    }

}

