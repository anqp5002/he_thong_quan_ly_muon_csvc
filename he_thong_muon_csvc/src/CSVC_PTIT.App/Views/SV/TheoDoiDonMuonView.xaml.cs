using System.Windows.Controls;
using CSVC_PTIT.App.ViewModels.SV;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.SV
{
    public partial class TheoDoiDonMuonView : UserControl
    {
        public TheoDoiDonMuonView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<TheoDoiDonMuonViewModel>();
        }
    }
}
