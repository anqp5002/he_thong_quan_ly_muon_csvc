namespace CSVC_PTIT.Core.DTOs;

public class CreateBorrowRequestDto
{
    // Thông tin người mượn
    public int RequesterId { get; set; }
    public string? ContactPhone { get; set; }

    // Thông tin đơn
    public string? Title { get; set; }
    public string? Purpose { get; set; }
    public string? RequestNote { get; set; }
    public int? RoomId { get; set; }

    // Thời gian
    public DateTime BorrowStartAt { get; set; }
    public DateTime BorrowEndAt { get; set; }

    // Danh sách CSVC cần mượn
    public List<BorrowRequestAssetDto> Assets { get; set; } = new();
}

public class BorrowRequestAssetDto
{
    public int AssetId { get; set; }
    public int QuantityRequested { get; set; }
    public string? ItemNote { get; set; }
}