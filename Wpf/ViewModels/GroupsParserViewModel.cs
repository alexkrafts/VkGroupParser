using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Core;
using Microsoft.Practices.Prism.Events;
using Prism.Commands;
using Prism.Regions;
using VkNet.Model;
using CollectionExtensions = System.Collections.ObjectModel.CollectionExtensions;

namespace Wpf.ViewModels
{
    public class GroupsParserViewModel : ViewModelBase, INavigationAware
    {
        private VkParser _parser;
        private string _searchQuery;

        public ObservableCollection<NotifyTask<Group>> Groups { get; set; }

        public ICommand AnalizeCommand => new DelegateCommand<object>(Agregate);

        private void Agregate(object id)
        {
            _eventAggregator.GetEvent<AgregateEvent>().Publish((long)id);
        }


        private IEventAggregator _eventAggregator;

        public GroupsParserViewModel(VkParser parser, IEventAggregator eventAggregator)
        {
            _parser = parser;
            _eventAggregator = eventAggregator;
            Groups = new ObservableCollection<NotifyTask<Group>>();
           
        }


        

        private void Search()
        {
            Groups.Clear();
            CollectionExtensions.AddRange(Groups, _parser.SearchFullGroupInfo(_searchQuery));
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _searchQuery = navigationContext.Parameters["query"].ToString();
            Search();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotSupportedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotSupportedException();
        }
    }
}
