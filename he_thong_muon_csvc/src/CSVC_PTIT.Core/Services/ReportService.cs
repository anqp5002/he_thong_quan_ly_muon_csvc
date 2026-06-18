using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CSVC_PTIT.Core.Services;

public class ReportService : IReportService
{
    private readonly CsvcDbContext _context;

    public ReportService(CsvcDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateCheckoutPdfAsync(int checkoutId)
    {
        var checkout = await _context.Checkouts
            .Include(c => c.CheckedOutByUser)
            .Include(c => c.CheckedOutToUser)
            .Include(c => c.CheckoutItems)
            .ThenInclude(ci => ci.Asset)
            .FirstOrDefaultAsync(c => c.CheckoutId == checkoutId);

        if (checkout == null) throw new Exception("Không tìm thấy phiếu bàn giao.");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text("PHIẾU BÀN GIAO CƠ SỞ VẬT CHẤT").SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Text($"Mã phiếu: {checkout.CheckoutCode}");
                    col.Item().Text($"Ngày bàn giao: {checkout.CheckoutAt:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Bên giao (QL CSVC): {checkout.CheckedOutByUser.FullName}");
                    col.Item().Text($"Bên nhận: {checkout.CheckedOutToUser.FullName}");

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // STT
                            columns.RelativeColumn();   // Tên CSVC
                            columns.ConstantColumn(80); // Số lượng
                            columns.RelativeColumn();   // Tình trạng
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("STT").Bold();
                            header.Cell().Text("Tên CSVC").Bold();
                            header.Cell().Text("Số lượng").Bold();
                            header.Cell().Text("Tình trạng").Bold();
                        });

                        int stt = 1;
                        foreach (var item in checkout.CheckoutItems)
                        {
                            table.Cell().Text(stt.ToString());
                            table.Cell().Text(item.Asset.AssetName);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text(item.ConditionBefore ?? "Tốt");
                            stt++;
                        }
                    });

                    col.Item().Text($"Ghi chú: {checkout.CheckoutNote}");
                    
                    col.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Text("BÊN NHẬN\n(Ký và ghi rõ họ tên)");
                        row.RelativeItem().AlignCenter().Text("BÊN GIAO\n(Ký và ghi rõ họ tên)");
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateReturnPdfAsync(int returnId)
    {
        var returnObj = await _context.Returns
            .Include(r => r.ReceivedByUser)
            .Include(r => r.ReturnedByUser)
            .Include(r => r.Checkout)
            .Include(r => r.ReturnItems)
            .ThenInclude(ri => ri.Asset)
            .FirstOrDefaultAsync(r => r.ReturnId == returnId);

        if (returnObj == null) throw new Exception("Không tìm thấy phiếu trả.");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                
                page.Header().Text("PHIẾU NHẬN TRẢ VÀ ĐỐI SOÁT").SemiBold().FontSize(20).FontColor(Colors.Green.Darken2);

                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Text($"Mã phiếu giao gốc: {returnObj.Checkout.CheckoutCode}");
                    col.Item().Text($"Ngày trả: {returnObj.ReturnedAt:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Người nhận (QL CSVC): {returnObj.ReceivedByUser.FullName}");
                    col.Item().Text($"Người trả: {returnObj.ReturnedByUser.FullName}");

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); 
                            columns.RelativeColumn();   
                            columns.ConstantColumn(80); 
                            columns.RelativeColumn();   
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("STT").Bold();
                            header.Cell().Text("Tên CSVC").Bold();
                            header.Cell().Text("SL Trả").Bold();
                            header.Cell().Text("Tình trạng").Bold();
                        });

                        int stt = 1;
                        foreach (var item in returnObj.ReturnItems)
                        {
                            table.Cell().Text(stt.ToString());
                            table.Cell().Text(item.Asset.AssetName);
                            table.Cell().Text(item.QuantityReturned.ToString());
                            table.Cell().Text(item.ConditionAfter ?? "Tốt");
                            stt++;
                        }
                    });
                    
                    col.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Text("NGƯỜI TRẢ\n(Ký và ghi rõ họ tên)");
                        row.RelativeItem().AlignCenter().Text("NGƯỜI NHẬN\n(Ký và ghi rõ họ tên)");
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateDamageReportPdfAsync(int reportId)
    {
        var report = await _context.DamageReports
            .Include(r => r.Asset)
            .Include(r => r.ReportedByUser)
            .Include(r => r.ResponsibleUser)
            .FirstOrDefaultAsync(r => r.ReportId == reportId);

        if (report == null) throw new Exception("Không tìm thấy biên bản sự cố.");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                
                page.Header().Text("BIÊN BẢN SỰ CỐ CƠ SỞ VẬT CHẤT").SemiBold().FontSize(20).FontColor(Colors.Red.Darken2);

                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Text($"Ngày lập: {report.ReportedAt:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Người lập (QL CSVC): {report.ReportedByUser.FullName}");
                    col.Item().Text($"Người chịu trách nhiệm: {report.ResponsibleUser.FullName}");
                    col.Item().Text($"Tài sản: {report.Asset.AssetName} (Mã: {report.Asset.AssetCode})");
                    col.Item().Text($"Loại sự cố: {report.IncidentType}");
                    col.Item().Text($"Mức độ: {report.Severity}");
                    col.Item().Text($"Mô tả chi tiết: {report.Description}");
                    col.Item().Text($"Chi phí bồi thường dự kiến: {report.EstimatedCompensation:N0} VNĐ");
                    
                    col.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Text("NGƯỜI CHỊU TRÁCH NHIỆM\n(Ký và ghi rõ họ tên)");
                        row.RelativeItem().AlignCenter().Text("NGƯỜI LẬP BIÊN BẢN\n(Ký và ghi rõ họ tên)");
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateInventoryReportPdfAsync()
    {
        var assets = await _context.Assets.Include(a => a.Category).ToListAsync();
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                
                page.Header().Text("BÁO CÁO TỒN KHO CSVC").SemiBold().FontSize(20);

                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Text($"Thời gian báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}");

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();   
                            columns.ConstantColumn(80); 
                            columns.ConstantColumn(80); 
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Tên CSVC").Bold();
                            header.Cell().Text("Tổng SL").Bold();
                            header.Cell().Text("Khả dụng").Bold();
                        });

                        foreach (var asset in assets)
                        {
                            table.Cell().Text(asset.AssetName);
                            table.Cell().Text(asset.TotalQuantity.ToString());
                            table.Cell().Text(asset.AvailableQuantity.ToString());
                        }
                    });
                });
            });
        });

        return document.GeneratePdf();
    }
}
