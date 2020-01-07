using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gride.Models;
using System;

namespace UnitTestGride
{
    [TestClass]
    public class AvailabilityTests
    {
        [TestMethod]
        public void TestCreateAvailability()
        {
            new Availability { Start = new DateTime(2019, 11, 18, 8, 0, 0), End = new DateTime(2019, 11, 18, 10, 0, 0), Weekly = true };
        }

        [TestMethod]
        public void TestCreateAvailability_WrongStartDate()
        {
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() =>
                new Availability { Start = new DateTime(2019134, 11, 18, 8, 0, 0), End = new DateTime(2019, 11, 18, 10, 0, 0), Weekly = true }
            );
        }

        [TestMethod]
        public void TestCreateAvailability_WrongEndDate()
        {
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() =>
                new Availability { Start = new DateTime(2019134, 11, 18, 8, 0, 0), End = new DateTime(2019, 11, 18, 10, 0, 0), Weekly = true }
            );
        }

        [TestMethod]
        public void TestCreateAvailability_MissingWeekly()
        {
            var availability = new Availability { Start = new DateTime(2019, 11, 18, 8, 0, 0), End = new DateTime(2019, 11, 18, 10, 0, 0) };
            bool expectedValue = false;

            Assert.AreEqual(availability.Weekly, expectedValue, "The value should be false.");
        }

        [TestMethod]
        public void TestCreateAvailability_MissingStartDate()
        {
            new Availability { End = new DateTime(2019, 11, 18, 10, 0, 0), Weekly = true };
        }

        [TestMethod]
        public void TestCreateAvailability_MissingEndDate()
        {
            new Availability { Start = new DateTime(2019, 11, 18, 8, 0, 0), Weekly = true };
        }
    }
}
