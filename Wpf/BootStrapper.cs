using System.Windows;
using Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Wpf.ViewModels;
using Wpf.Views;

namespace Wpf
{
    public class BootStrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterInstance(new VkParser());
            Container.RegisterInstance<IEventAggregator>(new EventAggregator());
            Container.RegisterTypeForNavigation<GroupsParserView>(nameof(GroupsParserView));
            Container.RegisterTypeForNavigation<LoginView>(nameof(LoginView));
            Container.RegisterTypeForNavigation<AnalizeGroupUsersPreferenceView>(nameof(AnalizeGroupUsersPreferenceView));
        }
    }

}
