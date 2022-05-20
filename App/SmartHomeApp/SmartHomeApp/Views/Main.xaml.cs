using Microcharts;
using SmartHomeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace SmartHomeApp.Views
{
    public partial class Main : ContentPage
    {
        public Main()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();

            ((MainViewModel)BindingContext).RefreshDataCommand.Execute(null);
        }
    }
}
