using NUnit.Framework;
using Moq;
using FakeTestApp.Services;
using FakeTestApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using FakeTestApp.Models;
using System;
using System.Threading.Tasks;

namespace FakeTestApp.Tests
{
    [TestFixture]
    public class UserStatisticsControllerTests
    {
        private Mock<IDataService> _dataService;
        private DurationSetup _durationSetup;
        private UserStatisticsController _testController;

        [SetUp]
        public void Setup()
        {
            _dataService = new Mock<IDataService>();
            _durationSetup = new DurationSetup(60000); 
            _testController = new UserStatisticsController(_dataService.Object, _durationSetup);
        }

        [Test]
        public async Task Get_WithEmptyQueryString_ReturnEmptyJsonObject()
        {
            string query = string.Empty;
            var expectedResult = new JsonResult(new object());

            var testResult = await _testController.Get(query);

            Assert.AreEqual(expectedResult.Value.ToString(), testResult.Value.ToString());
        }

        [Test]
        public async Task Get_WithQueryStringAndNoDataForAQuery_ReturnEmptyJsonObject()
        {
            string query = "UnexpectedGUID";
            _dataService.Setup(m => m.Get(query)).ReturnsAsync((RequestData)null);
            var expectedResult = new JsonResult(new object());

            var testResult = await _testController.Get(query);

            _dataService.Verify(m => m.Get(query), Times.AtLeastOnce());
            Assert.AreEqual(expectedResult.Value.ToString(), testResult.Value.ToString());
        }

        [Test]
        public async Task Get_WithValidQueryGUID_ReturnZeroPercentResult()
        {
            string query = Guid.NewGuid().ToString();
            var validData = new RequestData() { 
                Id = It.IsAny<int>(),
                RequestLocalTime = DateTime.Now,
                QueryId = query,
                UserData = new UserStatisticRequest
                {
                    TimeFrom = DateTime.Now,
                    TimeTo = DateTime.Now,
                    UserId = It.IsAny<string>()
                }
            };
            _dataService.Setup(m => m.Get(query)).ReturnsAsync(validData);

            var testResult = await _testController.Get(query);
            ResponseData testData = (ResponseData) testResult.Value;

            _dataService.Verify(m => m.Get(query), Times.AtLeastOnce());
            Assert.AreEqual(query, testData.Query);
            Assert.AreEqual(0, testData.Percent);
            Assert.IsNull(testData.Result);
        }

        [Test]
        public async Task Get_WithValidQueryGUID_ReturnHalfPercentResult()
        {
            string query = Guid.NewGuid().ToString();
            var validData = new RequestData()
            {
                Id = It.IsAny<int>(),
                RequestLocalTime = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(0.5d * _durationSetup.MaxDuration)),
                QueryId = query,
                UserData = new UserStatisticRequest
                {
                    TimeFrom = DateTime.Now,
                    TimeTo = DateTime.Now,
                    UserId = It.IsAny<string>()
                }
            };
            _dataService.Setup(m => m.Get(query)).ReturnsAsync(validData);

            var testResult = await _testController.Get(query);
            ResponseData testData = (ResponseData)testResult.Value;

            _dataService.Verify(m => m.Get(query), Times.AtLeastOnce());
            Assert.AreEqual(query, testData.Query);
            Assert.AreEqual(50, testData.Percent);
            Assert.IsNull(testData.Result);
        }

        [Test]
        public async Task Get_WithValidQueryGUID_ReturnFullResult()
        {
            string userID = "TestUserID";
            string query = Guid.NewGuid().ToString();
            var validData = new RequestData()
            {
                Id = It.IsAny<int>(),
                RequestLocalTime = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(_durationSetup.MaxDuration)),
                QueryId = query,
                UserData = new UserStatisticRequest
                {
                    TimeFrom = DateTime.Now,
                    TimeTo = DateTime.Now,
                    UserId = userID
                }
            };
            _dataService.Setup(m => m.Get(query)).ReturnsAsync(validData);

            var testResult = await _testController.Get(query);
            ResponseData testData = (ResponseData)testResult.Value;

            _dataService.Verify(m => m.Get(query), Times.AtLeastOnce());
            Assert.AreEqual(query, testData.Query);
            Assert.AreEqual(100, testData.Percent);
            Assert.IsNotNull(testData.Result);
            Assert.AreEqual(userID, testData.Result?.UserId);
            Assert.AreEqual("12", testData.Result?.CountSignIn);
        }

        [Test]
        public async Task Post_WithNullDataRequest_ReturnEmptyJsonObject()
        {
            UserStatisticRequest data = null;
            var expectedResult = new JsonResult(new object());

            var actualResult = await _testController.Post(data);

            Assert.AreEqual(expectedResult.Value.ToString(), actualResult.Value.ToString());
        }

        [Test]
        public async Task Post_WithEmptyUserID_ReturnEmptyJsonObject()
        {
            string userId = string.Empty;
            UserStatisticRequest data = new UserStatisticRequest
            {
                UserId = userId,
                TimeFrom = DateTime.Today,
                TimeTo = DateTime.Now
            };
            var expectedResult = new JsonResult(new object());

            var actualResult = await _testController.Post(data);

            Assert.AreEqual(expectedResult.Value.ToString(), actualResult.Value.ToString());
        }

        [Test]
        public async Task Post_WithInvalidDateTimes_ReturnEmptyJsonObject()
        {
            string userId = "TestUserID";
            UserStatisticRequest data = new UserStatisticRequest
            {
                UserId = userId,
                TimeFrom = DateTime.Today,
                TimeTo = DateTime.Today
            };
            var expectedResult = new JsonResult(new object());

            var actualResult = await _testController.Post(data);

            Assert.AreEqual(expectedResult.Value.ToString(), actualResult.Value.ToString());
        }

        [Test]
        public async Task Post_WithValidUserData_ReturnValidGUID()
        {
            string userId = "TestUserID";
            UserStatisticRequest data = new UserStatisticRequest
            {
                UserId = userId,
                TimeFrom = DateTime.Today,
                TimeTo = DateTime.Now
            };
            _dataService.Setup(m => m.Create(It.IsNotNull<RequestData>())).Verifiable();

            var actualResult = await _testController.Post(data);
            string guidValue = (string)actualResult.Value;
            bool success = Guid.TryParse(guidValue, out _);

            _dataService.Verify(m => m.Create(It.IsNotNull<RequestData>()), Times.AtLeastOnce);
            Assert.IsTrue(success);
        }
    }
}