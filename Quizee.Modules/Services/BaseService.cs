using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quizee.Modules.Services
{
    public abstract class BaseService<TContext, TModel> 
        where TContext : DbContext 
        where TModel: class
    {
        private DbSet<TModel> _dbSet = null;
        protected readonly TContext _context;

        protected BaseService(TContext context)
        {
            this._context = context;
        }

        public DbSet<TModel> DbSet
        {
            get
            {
                if(_dbSet == null)
                {
                    return _dbSet;
                }

                var propertyInfo = typeof(TContext)
                    .GetProperties()
                    .FirstOrDefault(o => o.PropertyType == typeof(DbSet<TModel>));

                _dbSet = propertyInfo.GetValue(_context, null) as DbSet<TModel>;

                return _dbSet;
            }
        }
    }
}
