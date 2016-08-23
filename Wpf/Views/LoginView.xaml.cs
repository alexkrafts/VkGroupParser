using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Wpf.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {

        public LoginView()
        {
            InitializeComponent();
            BindingOperations.SetBinding(this, SourceProperty, new Binding()
            {
                Source = DataContext,
                Path = new PropertyPath(nameof(Source))
            });
            BindingOperations.SetBinding(this, NavigatedCommandProperty, new Binding()
            {
                Source = DataContext,
                Path = new PropertyPath(nameof(NavigatedCommand))
            });
            Loaded += LoginView_Loaded;
            Browser.Navigated += Browser_Navigated;
        }

        

        private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            NavigatedCommand.Execute(e.Uri);
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            DoBrowse();
        }

        private void DoBrowse()
        {
            if (!String.IsNullOrEmpty(Source))
            {
                var uri = new Uri(Source, UriKind.RelativeOrAbsolute);
                Browser.Navigate(uri);
            }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(string), typeof(LoginView), new PropertyMetadata(default(string)));

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty NavigatedCommandProperty = DependencyProperty.Register(
            "NavigatedCommand", typeof (ICommand), typeof (LoginView), new PropertyMetadata(default(ICommand)));

        public ICommand NavigatedCommand
        {
            get { return (ICommand) GetValue(NavigatedCommandProperty); }
            set { SetValue(NavigatedCommandProperty, value); }
        }
    }
}
