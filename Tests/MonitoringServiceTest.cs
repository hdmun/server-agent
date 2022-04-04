using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerAgent.Monitoring.Interactor;
using ServerAgent.Monitoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class MonitoringServiceTest
    {
        [TestMethod]
        public void TimeChecker_Create_Test()
        {
            var detectModel = new DetectTimeModel()
            {
                DeadlockMin = 3,
                StoppedMin = 10,
                Checker = "datetime"
            };
            var datetimeChecker = TimeCheckerFactory.Create(detectModel);
            Assert.IsNotNull(datetimeChecker);
            Assert.IsInstanceOfType(datetimeChecker, typeof(DateTimeCheker));

            detectModel.Checker = "timegettime";
            var timegettimeChecker = TimeCheckerFactory.Create(detectModel);
            Assert.IsNotNull(timegettimeChecker);
            Assert.IsInstanceOfType(timegettimeChecker, typeof(TimeGetTimeChecker));
        }

        [TestMethod]
        public void TimeChecker_Stopped_Test()
        {
            var detectModel = new DetectTimeModel()
            {
                DeadlockMin = 3,
                StoppedMin = 10,
                Checker = "datetime"
            };
            var datetimeChecker = TimeCheckerFactory.Create(detectModel);
            Assert.IsNotNull(datetimeChecker);
            Assert.IsInstanceOfType(datetimeChecker, typeof(DateTimeCheker));
            Assert.IsFalse(datetimeChecker.IsStopped);

            detectModel.Checker = "timegettime";
            var timegettimeChecker = TimeCheckerFactory.Create(detectModel);
            Assert.IsNotNull(timegettimeChecker);
            Assert.IsInstanceOfType(timegettimeChecker, typeof(TimeGetTimeChecker));
            Assert.IsFalse(timegettimeChecker.IsStopped);
        }
    }
}
