using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Airgap.Entity
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        bool Contains(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> expression);
        T Find(Expression<Func<T, bool>> expression);
        IEnumerable<T> ExecuteSqlCommand(string command, params object[] parameters);
        T Get(long id);
        T Insert(T entity);
        T Update(T entity);
        bool Delete(T entity);

        int Count(Expression<Func<T, bool>> expression);

        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }
        DbSet<T> Entities { get; }
    }
}
