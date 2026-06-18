using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Xuất PDF Phiếu đăng ký mượn CSVC trong giờ (SV_BM02).
/// Sprint 3 — Task B.13
/// </summary>
public class PhieuMuonTrongGioPdf : IDocument
{
    private readonly BorrowRequest _request;

    public PhieuMuonTrongGioPdf(BorrowRequest request)
    {
        _request = request;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"Phiếu mượn CSVC {_request.RequestCode}",
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
                // ── Tiêu đề ──────────────────────────────────────
                col.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM")
                    .Bold().FontSize(12);
                col.Item().AlignCenter().Text("Độc lập – Tự do – Hạnh phúc")
                    .FontSize(11).Italic();
                col.Item().AlignCenter().Text("---------------o0o---------------")
                    .FontSize(10);

                col.Item().PaddingTop(10).AlignCenter()
                    .Text("PHIẾU ĐĂNG KÝ MƯỢN CƠ SỞ VẬT CHẤT")
                    .Bold().FontSize(14);
                col.Item().AlignCenter().Text("(Mượn trong giờ học / làm việc)")
                    .FontSize(10).Italic();

                col.Item().PaddingTop(4).AlignCenter()
                    .Text($"Mã phiếu: {_request.RequestCode}")
                    .FontSize(11).Bold();

                col.Item().PaddingTop(16).LineHorizontal(1);

                // ── Thông tin người mượn ─────────────────────────
                col.Item().PaddingTop(12).Text("I. THÔNG TIN NGƯỜI MƯỢN").Bold().FontSize(12);

                col.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(4);
                        c.RelativeColumn(3);
                        c.RelativeColumn(4);
                    });

                    // Hàng 1
                    table.Cell().PaddingVertical(4).Text("Họ và tên:").Bold();
                    table.Cell().PaddingVertical(4).Text(_request.Requester?.FullName ?? "—");
                    table.Cell().PaddingVertical(4).Text("MSSV/Mã NV:").Bold();
                    table.Cell().PaddingVertical(4).Text(_request.Requester?.StudentCode ?? "—");

                    // Hàng 2
                    table.Cell().PaddingVertical(4).Text("Lớp/Đơn vị:").Bold();
                    table.Cell().PaddingVertical(4).Text(_request.Requester?.Department?.DepartmentName ?? "—");
                    table.Cell().PaddingVertical(4).Text("Số điện thoại:").Bold();
                    table.Cell().PaddingVertical(4).Text(_request.ContactPhone ?? "—");

                    // Hàng 3
                    table.Cell().PaddingVertical(4).Text("Môn học/Công việc:").Bold();
                    table.Cell().ColumnSpan(3).PaddingVertical(4).Text(_request.Title ?? "—");
                });

                // ── Thời gian mượn ───────────────────────────────
                col.Item().PaddingTop(12).Text("II. THỜI GIAN MƯỢN").Bold().FontSize(12);

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
                    table.Cell().PaddingVertical(4).Text("Giờ bắt đầu:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.BorrowStartAt.ToString("HH:mm"));

                    table.Cell().PaddingVertical(4).Text("Giờ kết thúc:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.BorrowEndAt.ToString("HH:mm"));
                    table.Cell().PaddingVertical(4).Text("Giờ phải trả:").Bold();
                    table.Cell().PaddingVertical(4)
                        .Text(_request.ExpectedReturnAt.ToString("HH:mm dd/MM/yyyy"));
                });

                // ── Danh sách CSVC ───────────────────────────────
                col.Item().PaddingTop(12).Text("III. DANH SÁCH CSVC YÊU CẦU").Bold().FontSize(12);

                col.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(30);   // STT
                        c.ConstantColumn(80);   // Mã CSVC
                        c.RelativeColumn();     // Tên CSVC
                        c.ConstantColumn(50);   // SL yêu cầu
                        c.ConstantColumn(50);   // SL duyệt
                        c.RelativeColumn();     // Ghi chú
                    });

                    // Header
                    static IContainer HeaderCell(IContainer c) =>
                        c.Background("#1565C0").Padding(6).AlignCenter();

                    table.Header(h =>
                    {
                        h.Cell().Element(HeaderCell).Text("STT").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(HeaderCell).Text("Mã CSVC").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(HeaderCell).Text("Tên CSVC").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(HeaderCell).Text("SL YC").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(HeaderCell).Text("SL DY").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(HeaderCell).Text("Ghi chú").Bold().FontColor("#FFFFFF");
                    });

                    // Data rows
                    var assets = _request.BorrowRequestAssets.ToList();
                    for (int i = 0; i < assets.Count; i++)
                    {
                        var item = assets[i];
                        var bg = i % 2 == 0 ? "#FFFFFF" : "#E3F2FD";

                        static IContainer DataCell(IContainer c, string bg) =>
                            c.Background(bg).Padding(5);

                        table.Cell().Element(c => DataCell(c, bg)).AlignCenter()
                            .Text((i + 1).ToString());
                        table.Cell().Element(c => DataCell(c, bg))
                            .Text(item.Asset?.AssetCode ?? "—");
                        table.Cell().Element(c => DataCell(c, bg))
                            .Text(item.Asset?.AssetName ?? "—");
                        table.Cell().Element(c => DataCell(c, bg)).AlignCenter()
                            .Text(item.QuantityRequested.ToString());
                        table.Cell().Element(c => DataCell(c, bg)).AlignCenter()
                            .Text(item.QuantityApproved > 0
                                ? item.QuantityApproved.ToString()
                                : "—");
                        table.Cell().Element(c => DataCell(c, bg))
                            .Text(item.ItemNote ?? "");
                    }
                });

                // ── Ghi chú ──────────────────────────────────────
                if (!string.IsNullOrWhiteSpace(_request.RequestNote))
                {
                    col.Item().PaddingTop(12).Text("IV. GHI CHÚ").Bold().FontSize(12);
                    col.Item().PaddingTop(4).Text(_request.RequestNote);
                }

                // ── Cam kết ──────────────────────────────────────
                col.Item().PaddingTop(12).Text("V. CAM KẾT").Bold().FontSize(12);
                col.Item().PaddingTop(4).Text(
                    "Tôi cam kết sử dụng CSVC đúng mục đích, giữ gìn cẩn thận và hoàn trả " +
                    "đúng thời hạn. Nếu làm hư hỏng hoặc mất mát, tôi hoàn toàn chịu trách nhiệm " +
                    "bồi thường theo quy định của Học viện.");

                // ── Chữ ký ───────────────────────────────────────
                col.Item().PaddingTop(24).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn();
                        c.RelativeColumn();
                    });

                    table.Cell().AlignCenter().Column(inner =>
                    {
                        inner.Item().AlignCenter().Text("NGƯỜI MƯỢN").Bold();
                        inner.Item().AlignCenter().Text("(Ký và ghi rõ họ tên)").Italic().FontSize(9);
                        inner.Item().PaddingTop(40).AlignCenter()
                            .Text(_request.Requester?.FullName ?? "");
                    });

                    table.Cell().AlignCenter().Column(inner =>
                    {
                        inner.Item().AlignCenter()
                            .Text($"TP. HCM, ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}")
                            .FontSize(10).Italic();
                        inner.Item().AlignCenter().Text("QUẢN LÝ CSVC").Bold();
                        inner.Item().AlignCenter().Text("(Ký và ghi rõ họ tên)").Italic().FontSize(9);
                        inner.Item().PaddingTop(40).AlignCenter().Text("");
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

    /// <summary>
    /// Xuất PDF ra mảng byte (dùng trong ViewModel để save/preview).
    /// </summary>
    public static byte[] Generate(BorrowRequest request)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        return new PhieuMuonTrongGioPdf(request).GeneratePdf();
    }

    /// <summary>
    /// Xuất PDF ra file (dùng khi user chọn Save As).
    /// </summary>
    public static void GenerateToFile(BorrowRequest request, string filePath)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        new PhieuMuonTrongGioPdf(request).GeneratePdf(filePath);
    }
}