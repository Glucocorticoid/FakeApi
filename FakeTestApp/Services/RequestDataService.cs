using FakeTestApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeTestApp.Services
{
    public class RequestDataService : IDataService
    {
        private RequestContext _context;

        public RequestDataService(RequestContext context)
        {
            this._context = context;
        }

        public async Task Create(RequestData request)
        {
            await _context.AddAsyncFake(request);
        }

        public async Task<RequestData> Get(string guid)
        {
            return await _context.GetAsyncFake(guid);
        }
    }
}
