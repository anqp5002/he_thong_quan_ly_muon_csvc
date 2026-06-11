using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Xuất PDF Đơn mượn CSVC ngoài giờ (DT_BM01).
/// Sprint 3 — Task B.14
/// </summary>
public class DonMuonNgoaiGioPdf : IDocument
{
    private readonly BorrowRequest _request;

    public DonMuonNgoaiGioPdf(BorrowRequest request)
    {
        _request = request;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"Đơn mượn CSVC {_request.RequestCode}",
        Author = "Hệ thống PTIT HCM",
        CreationDate = DateTimeOffset.Now
    };

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

            page.Content().Column(col =>
            {
                // ── Quốc hiệu ────────────────────────────────────
                col.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM")
                    .Bold().FontSize(13);
                col.Item().AlignCenter().Text("Độc lập – Tự do – Hạnh phúc")
                    .FontSize(11).Italic();
                col.Item().AlignCenter().Text("---------------o0o---------------")
                    .FontSize(10);

                col.Item().PaddingTop(12).AlignCenter()
                    .Text("ĐƠN MƯỢN CƠ SỞ VẬT CHẤT")
                    .Bold().FontSize(15);

                col.Item().PaddingTop(4).AlignCenter()
                    .Text($"Mã đơn: {_request.RequestCode}")
                    .FontSize(11).Bold();

                // ── Kính gửi ─────────────────────────────────────
                col.Item().PaddingTop(12).Text(
                    "Kính gửi:  Phòng Tổ Chức Hành Chính – Quản Trị")
                    .FontSize(11);
                col.Item().PaddingLeft(48).Text(
                    "BTV Đoàn Học viện Cơ sở")
                    .FontSize(11);

                col.Item().PaddingTop(12).LineHorizontal(1);

                // ── Thông tin người gửi ──────────────────────────
                col.Item().PaddingTop(12).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(5);
                        c.RelativeColumn(2);
                        c.RelativeColumn(4);
                    });

                    table.Cell().PaddingVertical(4).Text("Người gửi đơn:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.Requester?.FullName ?? "—");
                    table.Cell().PaddingVertical(4).Text("Chức vụ:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.Requester?.Role?.RoleName ?? "Bí thư LCĐ / CN CLB");

                    table.Cell().PaddingVertical(4).Text("Đơn vị:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.Requester?.Department?.DepartmentName ?? "—");
                    table.Cell().PaddingVertical(4).Text("SĐT liên hệ:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.ContactPhone ?? "—");
                });

                // ── I. CSVC cần mượn ─────────────────────────────
                col.Item().PaddingTop(12).Text("I. Tên cơ sở vật chất cần mượn:")
                    .Bold().FontSize(12);

                col.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(35);   // STT
                        c.RelativeColumn();     // Tên CSVC
                        c.ConstantColumn(70);   // Số lượng
                        c.ConstantColumn(80);   // Đơn vị
                    });

                    // Header
                    static IContainer H(IContainer c) =>
                        c.Background("#1565C0").Padding(6).AlignCenter();

                    table.Header(h =>
                    {
                        h.Cell().Element(H).Text("STT").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(H).Text("Tên CSVC").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(H).Text("Số lượng").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(H).Text("Đơn vị").Bold().FontColor("#FFFFFF");
                    });

                    var assets = _request.BorrowRequestAssets.ToList();
                    for (int i = 0; i < assets.Count; i++)
                    {
                        var item = assets[i];
                        var bg = i % 2 == 0 ? "#FFFFFF" : "#E3F2FD";

                        static IContainer D(IContainer c, string bg) =>
                            c.Background(bg).Padding(5);

                        table.Cell().Element(c => D(c, bg)).AlignCenter()
                            .Text((i + 1).ToString());
                        table.Cell().Element(c => D(c, bg))
                            .Text(item.Asset?.AssetName ?? "—");
                        table.Cell().Element(c => D(c, bg)).AlignCenter()
                            .Text(item.QuantityRequested.ToString());
                        table.Cell().Element(c => D(c, bg)).AlignCenter()
                            .Text(item.Asset?.Category?.CategoryName ?? "Cái/Bộ");
                    }

                    // Thêm hàng trống nếu ít CSVC
                    for (int i = assets.Count; i < 3; i++)
                    {
                        table.Cell().Border(1).BorderColor("#E0E0E0").Padding(5)
                            .AlignCenter().Text((i + 1).ToString());
                        table.Cell().Border(1).BorderColor("#E0E0E0").Padding(5).Text("");
                        table.Cell().Border(1).BorderColor("#E0E0E0").Padding(5).Text("");
                        table.Cell().Border(1).BorderColor("#E0E0E0").Padding(5)
                            .AlignCenter().Text("Phòng/Cái/Bộ");
                    }
                });

                // ── II. Trang thiết bị thêm ──────────────────────
                col.Item().PaddingTop(12)
                    .Text($"II. Trang thiết bị cần thêm: {_request.RequestNote ?? "Không có"}")
                    .FontSize(11);

                // ── III. Hỗ trợ khác ─────────────────────────────
                col.Item().PaddingTop(6)
                    .Text("III. Hỗ trợ khác: Không có")
                    .FontSize(11);

                // ── IV. Mục đích + Thời gian ─────────────────────
                col.Item().PaddingTop(6)
                    .Text($"IV. Mục đích sử dụng: {_request.Purpose ?? _request.Title ?? "—"}")
                    .FontSize(11);

                col.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(4);
                        c.RelativeColumn(3);
                        c.RelativeColumn(4);
                    });

                    table.Cell().PaddingVertical(4).Text("Ngày mượn:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.BorrowStartAt.ToString("dd/MM/yyyy"));
                    table.Cell().PaddingVertical(4).Text("Ngày trả:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.ExpectedReturnAt.ToString("dd/MM/yyyy"));

                    table.Cell().PaddingVertical(4).Text("Thời gian bắt đầu:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.BorrowStartAt.ToString("HH:mm"));
                    table.Cell().PaddingVertical(4).Text("Thời gian kết thúc:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.BorrowEndAt.ToString("HH:mm"));
                });

                // ── Cam kết ──────────────────────────────────────
                col.Item().PaddingTop(12).Text(
                    "Cam kết: Chúng tôi cam kết bảo quản tốt tài sản và chịu trách nhiệm " +
                    "bồi thường nếu xảy ra hư hỏng hoặc mất mát.")
                    .FontSize(11).Italic();

                // ── Chữ ký 2 bên ─────────────────────────────────
                col.Item().PaddingTop(24).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn();
                        c.RelativeColumn();
                    });

                    table.Cell().AlignCenter().Column(inner =>
                    {
                        inner.Item().AlignCenter()
                            .Text("NGƯỜI LÀM ĐƠN").Bold();
                        inner.Item().AlignCenter()
                            .Text("(Bí thư LCĐ / CN CLB)").Italic().FontSize(9);
                        inner.Item().AlignCenter()
                            .Text("(Ký, ghi rõ họ tên)").Italic().FontSize(9);
                        inner.Item().PaddingTop(40).AlignCenter()
                            .Text(_request.Requester?.FullName ?? "");
                    });

                    table.Cell().AlignCenter().Column(inner =>
                    {
                        inner.Item().AlignCenter()
                            .Text($"TP. HCM, ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}")
                            .FontSize(10).Italic();
                        inner.Item().AlignCenter()
                            .Text("Ý KIẾN BTV ĐOÀN HVCS").Bold();
                        inner.Item().AlignCenter()
                            .Text("(Bí thư Đoàn)").Italic().FontSize(9);
                        inner.Item().AlignCenter()
                            .Text("(Ký, ghi rõ họ tên)").Italic().FontSize(9);
                        inner.Item().PaddingTop(40).AlignCenter().Text(
                            _request.Approver?.FullName ?? "");
                    });
                });

                // ── Footer ───────────────────────────────────────
                col.Item().PaddingTop(16).LineHorizontal(1);
                col.Item().PaddingTop(4).AlignCenter()
                    .Text($"In lúc: {DateTime.Now:dd/MM/yyyy HH:mm} — Hệ thống Quản lý CSVC PTIT HCM")
                    .FontSize(8).FontColor("#9E9E9E").Italic();
            });
        });
    }

    public static byte[] Generate(BorrowRequest request)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        return new DonMuonNgoaiGioPdf(request).GeneratePdf();
    }

    public static void GenerateToFile(BorrowRequest request, string filePath)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        new DonMuonNgoaiGioPdf(request).GeneratePdf(filePath);
    }
}