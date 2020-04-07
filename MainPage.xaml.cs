using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ItemsRepeaterTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableGroupedCollection<string, Contact> ContactList
        {
            get { return (ObservableGroupedCollection<string, Contact>)GetValue(ContactListProperty); }
            set { SetValue(ContactListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContactList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContactListProperty =
            DependencyProperty.Register(nameof(ContactList), typeof(ObservableGroupedCollection<string, Contact>), typeof(MainPage), new PropertyMetadata(null));

        public MainPage()
        {
            this.InitializeComponent();

            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                ContactList = new ObservableGroupedCollection<string, Contact>((await Contact.GetContactsAsync()).GroupBy(contact => contact.LastName.Substring(0, 1).ToUpper()).OrderBy(g => g.Key));
                ContactsCVS.Source = ContactList;
            });
        }
    }
}
