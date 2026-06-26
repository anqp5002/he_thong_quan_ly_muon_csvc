using System.Windows.Controls;
using CSVC_PTIT.App.ViewModels.DT;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.DT
{
    public partial class DanhSachDonCanDuyetView : UserControl
    {
        public DanhSachDonCanDuyetView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<DanhSachDonCanDuyetViewModel>();
        }
    }
}
