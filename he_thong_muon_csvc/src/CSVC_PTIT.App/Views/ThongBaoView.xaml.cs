using System.Windows.Controls;
using CSVC_PTIT.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views
{
    public partial class ThongBaoView : UserControl
    {
        public ThongBaoView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ThongBaoViewModel>();
        }
    }
}
