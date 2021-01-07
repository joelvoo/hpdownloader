using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLHTMLReports.DAL
{
    interface IRepository<TEntity> : IDisposable where TEntity : IEntity
    {
        IQueryable<TEntity> GetAll();
        void Delete(TEntity entity);
        void Add(TEntity entity);
    }
}
