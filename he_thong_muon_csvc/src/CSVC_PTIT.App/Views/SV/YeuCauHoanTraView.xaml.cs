using System.Windows;
using CSVC_PTIT.App.ViewModels.SV;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.SV
{
    public partial class YeuCauHoanTraView : Window
    {
        public YeuCauHoanTraView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<YeuCauHoanTraViewModel>();
        }
    }
}
