using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sports_Application.Models;

namespace SportsApplication.Models
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly SportsApplicationDbContext db;
        public IData Data { get; private set; }
        public UnitOfWork(SportsApplicationDbContext db, IData data)
        {
            this.db = db;
            this.Data = data;
        }

        public int Commit()
        {
            return db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}