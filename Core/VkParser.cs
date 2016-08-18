using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace Core
{
    public class VkParser
    {
        private const ulong AppId = 3524567;
        private VkApi _vk = new VkApi();


        public bool Auth(string login = "kravcov_alexey@mail.ru", string password = "Kravc0v_A1exey")
        {
            _vk = new VkApi();
            var auth = new ApiAuthParams()
            {
                ApplicationId = AppId,
                Login = login,
                Password = password,
                Settings = Settings.All
            };
            _vk.Authorize(auth);
            _vk.RequestsPerSecond = 10000;
            return _vk.IsAuthorized;
        }

        public bool LoggedIn()
        {
            if (!_vk.IsAuthorized)
            {
                Debug.WriteLine("User not logged in");
            }
            return _vk.IsAuthorized;
        }
        public IEnumerable<Group> SearchGroups(string filter, GroupSort sort = GroupSort.Normal)
        {
            var searchParams = new GroupsSearchParams { Query = filter, Sort = sort };
            return _vk.Groups.Search(searchParams);
        }
        public IEnumerable<object> GetGroupsForCurrentUser()
        {
            return GetUserGroups(_vk.UserId.Value);
        }

        public IEnumerable<object> GetMusicForCurrentUser()
        {
            if (!LoggedIn()) return null;
            return _vk.Audio.Get(_vk.UserId.Value);
        }



        public IEnumerable<Contact> GetGroupContactsById(string id)
        {
            return _vk.Groups.GetById(new[] { id }, id, GroupsFields.Contacts).First().Contacts;
        }
        public Group GetGroupFullInfoById(string id)
        {
            return _vk.Groups.GetById(new[] { id }, id, GroupsFields.All).First();
        }
        public IEnumerable<NotifyTask<Group>> SearchFullGroupInfo(string query, int limit = 10)
        {
            var list = SearchGroups(query).ToList();
            if (list.Count > limit)
                list = list.Take(limit).ToList();
            var index = 0;
            var delay = 300;
            while (index < list.Count)
            {
                var millisecondsDelay = delay;
                var i = index;
                yield return new NotifyTask<Group>(
                    GroupTask(millisecondsDelay, list[i].Id.ToString()),
                    $"Loading {list[i].Name}");
                index++;
                delay += 300;

            }
        }

        private async Task<Group> GroupTask(int delay, string id)
        {
            await Task.Delay(delay);
            return GetGroupFullInfoById(id);

        }

        public IEnumerable<User> GetGroupMembers(string id)
        {
            var p = new GroupsGetMembersParams() { GroupId = id, Fields = UsersFields.Sex | UsersFields.Country | UsersFields.City };

            return _vk.Groups.GetMembers(p);
        }

        public IEnumerable<Group> GetUserGroups(long id)
        {
            if (!LoggedIn()) return null;
            var g = new GroupsGetParams() { UserId = id, Fields = GroupsFields.Description};
            return _vk.Groups.Get(g);
        }
    }
}
