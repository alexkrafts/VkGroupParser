using System;
using System.Linq;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class VkTestStatic
    {
        private VkParser _parser;

        [TestInitialize]
        public void Init()
        {
            _parser = new VkParser();
        }
        [TestMethod]
        public void CheckAuth()
        {
            Assert.IsTrue(_parser.Auth(TestAuthData.Username,TestAuthData.Password));
        }
        //[TestMethod]
        //public void SearchGroups()
        //{
        //    var groups = _parser.SearchGroups("vk");
        //    Assert.IsNotNull(groups);
        //    Assert.AreNotEqual(0, groups.Count());
        //}
    }
}