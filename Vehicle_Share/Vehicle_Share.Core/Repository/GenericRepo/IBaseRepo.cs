using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
namespace Vehicle_Share.Core.Repository.GenericRepo
{
    public interface IBaseRepo<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<string> UploadImageAsync(string Folder, IFormFile imageData);
        //    Task<string> UpdateImageAsync(string folder, IFormFile file, string existingFilePath);
        Task RemoveImageAsync(string filePath);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        Task<T> FindAsync(Expression<Func<T, bool>> match, params Expression<Func<T, object>>[] includeProperties);


    }
}
