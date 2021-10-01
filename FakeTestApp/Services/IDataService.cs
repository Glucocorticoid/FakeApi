using FakeTestApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeTestApp.Services
{
    public interface IDataService
    {
        Task Create(RequestData request);
        Task<RequestData> Get(string guid);
    }
}
