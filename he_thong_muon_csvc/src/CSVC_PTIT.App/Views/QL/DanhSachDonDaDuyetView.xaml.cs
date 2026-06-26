using System.Windows.Controls;
using CSVC_PTIT.App.ViewModels.QL;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.QL
{
    public partial class DanhSachDonDaDuyetView : UserControl
    {
        public DanhSachDonDaDuyetView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<DanhSachDonDaDuyetViewModel>();
        }
    }
}
