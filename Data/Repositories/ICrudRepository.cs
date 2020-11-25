﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project_managment.Data.Repositories
{
    public interface ICrudRepository<E>
    {
        Task<E> FindById(long id);
        Task<IEnumerable<E>> FindAll();
        Task<IEnumerable<E>> FindAll(int page, int size);
        Task<long> Save(E entity);
        Task<bool> Remove(E entity);
        Task<bool> RemoveById(long id);
        Task Update(E entity);
    }
}
