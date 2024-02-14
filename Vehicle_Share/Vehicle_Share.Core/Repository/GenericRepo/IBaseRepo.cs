using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Repository.GenericRepo
{
    public interface IBaseRepo<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(string id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<byte[]> GetImageAsync(Guid id);
        Task<Guid> UploadImageAsync(Guid id, byte[] imageData);


    }
}
