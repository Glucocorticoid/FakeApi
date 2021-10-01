using FakeTestApp.Models;
using FakeTestApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeTestApp.Controllers
{
    [ApiController]
    public class UserStatisticsController : ControllerBase
    {
        private IDataService _dataService;
        private DurationSetup durationSetup;

        public UserStatisticsController(IDataService dataService, DurationSetup durationSetup)
        {
            _dataService = dataService;
            this.durationSetup = durationSetup;
        }

        [Route("report/user_statistics")]
        [HttpPost]
        public async Task<JsonResult> Post(UserStatisticRequest data)
        {
            if (data == null
                || string.IsNullOrWhiteSpace(data.UserId)
                || data.TimeFrom >= data.TimeTo)
                return new JsonResult(new object());

            string query = Guid.NewGuid().ToString();

            RequestData request = new() {
                UserData = data,
                RequestLocalTime = DateTime.Now,
                QueryId = query
            };

            await _dataService.Create(request);

            return new JsonResult(query);
        }

        [Route("report/info")]
        [HttpGet]
        public async Task<JsonResult> Get(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new JsonResult(new object());

            var linkedRequest = await _dataService.Get(query);

            if (linkedRequest == null)
                return new JsonResult(new object());

            int timeSpend = (int)DateTime.Now.Subtract(linkedRequest.RequestLocalTime).TotalMilliseconds;
            int percent = timeSpend > durationSetup.MaxDuration 
                ? 100 
                : (100 * timeSpend / durationSetup.MaxDuration);

            ResponseData info = new(query, percent, percent == 100 
                ? new UserInfoData(linkedRequest.UserData.UserId, "12") 
                : null);

            return new JsonResult(info);
        }
    }
}
