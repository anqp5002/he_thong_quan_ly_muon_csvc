using System.Windows.Controls;

namespace CSVC_PTIT.App.Views;

/// <summary>
/// View tạm (placeholder) cho các chức năng chưa hoàn thiện.
/// Hiển thị icon + tên chức năng + thông báo "đang phát triển".
/// </summary>
public partial class PlaceholderView : UserControl
{
    public PlaceholderView(string pageName)
    {
        InitializeComponent();
        TxtTitle.Text = pageName;
    }
}
