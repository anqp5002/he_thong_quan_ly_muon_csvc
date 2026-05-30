using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CSVC_PTIT.App.Views;

/// <summary>
/// Dialog thêm/sửa CSVC.
/// Sprint 1 — Task A.7
/// </summary>
public partial class AssetDialogView : Window
{
    private readonly Asset? _editingAsset;
    public Asset? ResultAsset { get; private set; }

    public AssetDialogView(List<AssetCategory> categories, List<Room> rooms, Asset? editAsset = null)
    {
        InitializeComponent();
        _editingAsset = editAsset;

        CmbCategory.ItemsSource = categories;
        CmbManagementMode.ItemsSource = Enum.GetValues<ManagementMode>();
        CmbCondition.ItemsSource = Enum.GetValues<ConditionStatus>();
        CmbRoom.ItemsSource = rooms;

        if (editAsset != null)
        {
            TxtDialogTitle.Text = "SỬA CSVC";
            TxtAssetCode.Text = editAsset.AssetCode;
            TxtAssetName.Text = editAsset.AssetName;
            CmbCategory.SelectedValue = editAsset.CategoryId;
            CmbManagementMode.SelectedItem = editAsset.ManagementMode;
            TxtTotalQty.Text = editAsset.TotalQuantity.ToString();
            TxtAvailableQty.Text = editAsset.AvailableQuantity.ToString();
            TxtUnit.Text = editAsset.Unit;
            TxtBrand.Text = editAsset.Brand;
            CmbCondition.SelectedItem = editAsset.ConditionStatus;
            CmbRoom.SelectedValue = editAsset.CurrentRoomId;
            TxtDescription.Text = editAsset.Description;
        }
        else
        {
            CmbManagementMode.SelectedIndex = 0;
            CmbCondition.SelectedIndex = 0;
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtAssetCode.Text))
        { ShowError("Vui lòng nhập mã CSVC."); return; }
        if (string.IsNullOrWhiteSpace(TxtAssetName.Text))
        { ShowError("Vui lòng nhập tên CSVC."); return; }
        if (CmbCategory.SelectedValue == null)
        { ShowError("Vui lòng chọn loại CSVC."); return; }

        var asset = _editingAsset ?? new Asset();
        asset.AssetCode = TxtAssetCode.Text.Trim();
        asset.AssetName = TxtAssetName.Text.Trim();
        asset.CategoryId = (int)CmbCategory.SelectedValue;
        asset.ManagementMode = CmbManagementMode.SelectedItem is ManagementMode mm ? mm : ManagementMode.Quantity;
        asset.TotalQuantity = int.TryParse(TxtTotalQty.Text, out var tq) ? tq : 1;
        asset.AvailableQuantity = int.TryParse(TxtAvailableQty.Text, out var aq) ? aq : asset.TotalQuantity;
        asset.Unit = TxtUnit.Text.Trim();
        asset.Brand = TxtBrand.Text.Trim();
        asset.ConditionStatus = CmbCondition.SelectedItem is ConditionStatus cs ? cs : ConditionStatus.Good;
        asset.CurrentRoomId = CmbRoom.SelectedValue as int?;
        asset.Description = TxtDescription.Text.Trim();

        ResultAsset = asset;
        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ShowError(string msg)
    {
        TxtError.Text = msg;
        TxtError.Visibility = Visibility.Visible;
    }
}
