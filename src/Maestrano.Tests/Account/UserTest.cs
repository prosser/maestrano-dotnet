using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maestrano.Account;
using Maestrano.Helpers;

namespace Maestrano.Tests.Account
{
    [TestClass]
    public class UserTest
    {
        public UserTest()
        {
            MnoHelper.Environment = "test";
            MnoHelper.Api.Id = "app-1";
            MnoHelper.Api.Key = "gfcmbu8269wyi0hjazk4t7o1sndpvrqxl53e1";
        }

        [TestMethod]
        public void All_ItShouldReturnTheListOfUsers()
        {
            var list = User.All();
            Assert.IsTrue(list[0].Id.Contains("usr-"));
            Assert.IsTrue(list[1].Id.Contains("usr-"));
        }

        [TestMethod]
        public void Retrieve_ItShouldReturnASingleUser()
        {
            var obj = User.Retrieve("usr-1");
            Assert.AreEqual("usr-1", obj.Id);
        }

        [TestMethod]
        public void CheckPassword_ItShouldReturnTheRightValues()
        {
            Assert.IsTrue(User.CheckPassword("usr-1", "password"));
            Assert.IsFalse(User.CheckPassword("usr-1", "invalid_password"));
            Assert.IsTrue(User.CheckPassword("j.doe@company.com", "password"));
            Assert.IsFalse(User.CheckPassword("j.doe@company.com", "invalid_password"));
        }
    }
}
