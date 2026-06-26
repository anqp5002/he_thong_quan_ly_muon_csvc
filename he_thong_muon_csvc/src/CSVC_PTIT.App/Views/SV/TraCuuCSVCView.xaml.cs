using System.Windows.Controls;
using CSVC_PTIT.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.SV
{
    /// <summary>
    /// Interaction logic for TraCuuCSVCView.xaml
    /// </summary>
    public partial class TraCuuCSVCView : UserControl
    {
        public TraCuuCSVCView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<TraCuuCSVCViewModel>();
        }
    }
}
