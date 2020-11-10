using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_managment.Services
{
    public interface ICrudRepository<E>
    {
        Task<E> FindById(long id);
        Task<IEnumerable<E>> FindAll(int limit = 0, int offset = 0);
        Task Save(E entity);
        Task Remove(E entity);
        Task RemoveById(long id);
        Task Update(E entity);
    }
}
