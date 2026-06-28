using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CSVC_PTIT.App.ViewModels.DT;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace CSVC_PTIT.App.Views.DT
{
    /// <summary>
    /// Interaction logic for TaoDonNgoaiGioView.xaml
    /// </summary>
    public partial class TaoDonNgoaiGioView : Window
    {
        /// <summary>Đường dẫn file ảnh minh chứng đã chọn</summary>
        public string? SelectedAttachmentPath { get; private set; }

        public TaoDonNgoaiGioView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<TaoDonNgoaiGioViewModel>();
        }

        private void BtnChonAnh_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn ảnh minh chứng",
                Filter = "Ảnh|*.png;*.jpg;*.jpeg;*.bmp|Tất cả|*.*",
                Multiselect = false
            };

            if (dlg.ShowDialog() == true)
            {
                SelectedAttachmentPath = dlg.FileName;
                TxtTenFile.Text = Path.GetFileName(dlg.FileName);
                TxtTenFile.Foreground = System.Windows.Media.Brushes.Black;

                // Hiển thị preview ảnh
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(dlg.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImgPreview.Source = bitmap;
                    ImgPreview.Visibility = Visibility.Visible;
                }
                catch
                {
                    ImgPreview.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
