using System;
using System.Linq;
using System.Windows.Input;
using Core;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Prism.Regions;
using Wpf.Views;
using Prism.Commands;

namespace Wpf.ViewModels
{
    public class AgregateEvent : CompositePresentationEvent<long> { }
    public class TokenReceived : CompositePresentationEvent<string> { }
    public class MainViewModel : Notifier
    {
        private readonly IRegionManager _RegionManager;
        private VkParser _parser;
        private string _searchQuery = "vk";
        private IEventAggregator _eventAggregator;

        public MainViewModel(IRegionManager regionManager, VkParser parser, IEventAggregator eventAggregator)
        {
            _RegionManager = regionManager;
            _parser = parser;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AgregateEvent>().Subscribe(Analize, ThreadOption.UIThread);
            _eventAggregator.GetEvent<TokenReceived>().Subscribe(Logged, ThreadOption.UIThread);
            regionManager.Regions.CollectionChanged += Regions_CollectionChanged;
            
        }

        private void Regions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count>0 && NotLoggedIn)
                LogIn();
        }

        private void Logged(string token)
        {
            RemoveRegionHolder();
            NotifyPropertyChanged(nameof(LoggedIn));
            NotifyPropertyChanged(nameof(NotLoggedIn));
        }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
                NotifyPropertyChanged();
            }
        }


        private void Search()
        {
            RemoveRegionHolder();
            var query = new UriQuery
            {
                {"query", _searchQuery}
            };

            _RegionManager.RequestNavigate("ContentRegion", nameof(GroupsParserView) + query.ToString());
        }
        public ICommand SearchCommand => new DelegateCommand(Search);

        public ICommand LogInCommand => new DelegateCommand(LogIn);

        public bool NotLoggedIn => !_parser.LoggedIn();



        public bool LoggedIn => _parser.LoggedIn();

        private void LogIn()
        {
            RemoveRegionHolder();
            _RegionManager.RequestNavigate("ContentRegion", nameof(LoginView));
        }

        private void RemoveRegionHolder()
        {
            if (_RegionManager.Regions["ContentRegion"].Views.Any())
            {
                var loginView = _RegionManager.Regions["ContentRegion"].Views.ElementAt(0);
                _RegionManager.Regions["ContentRegion"].Remove(loginView);
            }
        }
        private void Analize(long id)
        {
            RemoveRegionHolder();
            var query = new UriQuery
            {
                {"id", id.ToString()}
            };

            _RegionManager.RequestNavigate("ContentRegion", nameof(AnalizeGroupUsersPreferenceView) + query);
        }
    }
}
