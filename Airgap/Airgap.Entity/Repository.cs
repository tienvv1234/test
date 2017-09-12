using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Airgap.Entity
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationContext context;
        private DbSet<T> entities;
        string errorMessage = string.Empty;

        public Repository(ApplicationContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public bool Contains(Expression<Func<T, bool>> expression)
        {
            return entities.Count(expression) > 0;
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> expression)
        {
            return entities.Where(expression);
        }

        public T Find(Expression<Func<T, bool>> expression)
        {
            return entities.FirstOrDefault(expression);
        }

        public IEnumerable<T> ExecuteSqlCommand(string command, params object[] parameters)
        {
            return entities.FromSql(command, parameters);
        }

        public T Get(long id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }
        public T Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public T Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            context.SaveChanges();
            return entity;
        }

        public bool Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
                //return false;
            }
            entities.Remove(entity);
            context.SaveChanges();
            return true;
        }
        public int Count(Expression<Func<T, bool>> expression)
        {
            return entities.Count(expression);
        }

        /// <summary>
        /// Gets a table
        /// </summary>
        public IQueryable<T> Table
        {
            get
            {
                return Entities;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public IQueryable<T> TableNoTracking
        {
            get
            {
                return Entities.AsNoTracking();
            }
        }

        /// <summary>
        /// Entities
        /// </summary>
        public DbSet<T> Entities
        {
            get
            {
                if (entities == null)
                    entities = context.Set<T>();
                return entities;
            }
        }

    }
}
