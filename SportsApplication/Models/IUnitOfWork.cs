using Sports_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsApplication.Models
{
    public interface IUnitOfWork : IDisposable
    {
        IData Data { get; }
        int Commit();
    }
}
