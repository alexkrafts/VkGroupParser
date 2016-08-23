using System;
using System.Linq;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class VkTestsLogged
    {
        private VkParser _parser;

        [TestInitialize]
        public void Init()
        {
            _parser = new VkParser();
            _parser.Auth(TestAuthData.Username, TestAuthData.Password);
        }

        [TestMethod]
        public void GetGroups()
        {
            var groups = _parser.GetGroupsForCurrentUser();
            Assert.IsNotNull(groups);
            Assert.AreNotEqual(0, groups.Count());
        }
        [TestMethod]
        public void SearchGroups()
        {
            var groups = _parser.SearchGroups("vk");
            Assert.IsNotNull(groups);
            Assert.AreNotEqual(0, groups.Count());
        }
        [TestMethod]
        public void GetMusic()
        {
            var music = _parser.GetMusicForCurrentUser();
            Assert.IsNotNull(music);
            Assert.AreNotEqual(0, music.Count());
        }
        [TestMethod]
        public void GetGroupContacts()
        {
            var contacts = _parser.GetGroupContactsById("22822305");
            Assert.IsNotNull(contacts);
            Assert.AreNotEqual(0, contacts.Count());
        }
        [TestMethod]
        public void GetGroupMembers()
        {
            var contacts = _parser.GetGroupMembers("22822305");
            Assert.IsNotNull(contacts);
            Assert.AreNotEqual(0, contacts.Count());
        }
        [TestMethod]
        public void GetUserGroups_DeletedUser()
        {
            var groups = _parser.GetUserGroups(12311);
            Assert.IsNull(groups);
            
        }


    }
}
