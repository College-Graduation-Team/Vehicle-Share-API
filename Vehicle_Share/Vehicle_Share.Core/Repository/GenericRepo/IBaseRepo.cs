using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
namespace Vehicle_Share.Core.Repository.GenericRepo
{
    public interface IBaseRepo<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<string> UploadImageAsync(string Folder, IFormFile imageData, string SubFolder);
        //    Task<string> UpdateImageAsync(string folder, IFormFile file, string existingFilePath);
        Task RemoveImageAsync(string filePath);
        Task<T> FindAsync(Expression<Func<T, bool>> match);



    }
}
