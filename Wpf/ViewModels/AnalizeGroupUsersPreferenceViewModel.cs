using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Core;
using Prism.Commands;
using Prism.Regions;
using VkNet.Enums;
using VkNet.Model;

namespace Wpf.ViewModels
{
    public class AnalizeGroupUsersPreferenceViewModel : Notifier, INavigationAware
    {
        private const int DelayPerRequest = 300;
        private VkParser _parser;
        private Dictionary<long, int> _maleGroupsIdDictionary;
        private Dictionary<Group, int> _femaleGroups;
        private long _targetGroupId;
        private int _maleProgressValue;
        private int _userCountLimit = 50;

        public Dictionary<NotifyTask<Group>, int> MaleGroups { get; set; }

        public int MaleProgressValue
        {
            get { return _maleProgressValue; }
            set
            {
                _maleProgressValue = value;
                NotifyPropertyChanged();
            }
        }

        public int UserCountLimit
        {
            get { return _userCountLimit; }
            set
            {
                _userCountLimit = value;
                NotifyPropertyChanged();
            }
        }
        public ICommand NavigateCommand { get; set; }

        private void Navigate(string obj)
        {
            System.Diagnostics.Process.Start("http://www.vk.com/"+obj);
        }


        private void Analize()
        {
            var list = _parser.GetGroupMembers(_targetGroupId.ToString());
            
            //male 
            var maleTasks = new List<Task>();
            _maleGroupsIdDictionary = new Dictionary<long, int>();
            var delay = DelayPerRequest;
            if (list.Count() < UserCountLimit)
                UserCountLimit = list.Count();
            foreach (var man in list.Where(x => x.Sex == Sex.Male).Take(UserCountLimit).ToList())
            {
                var mdelay = delay;
                maleTasks.Add(GetUserGroups(man.Id, mdelay, _maleGroupsIdDictionary));
                delay += DelayPerRequest;
            }
            Waiter(maleTasks);
        }



        private async Task GetUserGroups(long id, int delay, Dictionary<long, int> groupDict)
        {
            await Task.Delay(delay);
            try
            {
                CheckGroups(_parser.GetUserGroups(id), groupDict);
            }
            catch (Exception e)
            {
                deletedCount++;
            }
        }
        private async void Waiter(List<Task> list)
        {
            await Task.WhenAll(list);
            _maleGroupsIdDictionary = _maleGroupsIdDictionary.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            var delay = DelayPerRequest;
            MaleGroups = new Dictionary<NotifyTask<Group>, int>();
            foreach (var item in _maleGroupsIdDictionary)
            {
                MaleGroups.Add(
                    new NotifyTask<Group>(FillGroup(item.Key, delay),
                    $"Loading {item.Key}"), item.Value);
                delay += DelayPerRequest;
            }
            NotifyPropertyChanged(nameof(MaleGroups));
            if (deletedCount > 0) MessageBox.Show($"Deleted {deletedCount}/{UserCountLimit} users");
            MaleProgressValue = 0;
        }

        private async Task<Group> FillGroup(long id, int delay)
        {
            await Task.Delay(delay);
            var result = new Group() {Id = id};
            try
            {
                result = _parser.GetGroupFullInfoById(id.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }
        private int deletedCount = 0;
        private void CheckGroups(IEnumerable<Group> groups, Dictionary<long, int> groupDict)
        {
            lock (_maleGroupsIdDictionary)
            {
                foreach (var group in groups)
                {
                    if (group.Id == _targetGroupId) continue;
                    if (groupDict.ContainsKey(group.Id))
                        groupDict[group.Id]++;
                    else
                        groupDict.Add(group.Id, 1);
                }
                MaleProgressValue++;
            }
        }

        public AnalizeGroupUsersPreferenceViewModel(VkParser parser)
        {
            _parser = parser;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (long.TryParse(navigationContext.Parameters["id"].ToString(), out _targetGroupId))
                Analize();
                //App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(Analize));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }
    }
}
