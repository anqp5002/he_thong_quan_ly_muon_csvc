using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

public interface IReportService
{
    Task<byte[]> GenerateCheckoutPdfAsync(int checkoutId);
    Task<byte[]> GenerateReturnPdfAsync(int returnId);
    Task<byte[]> GenerateDamageReportPdfAsync(int reportId);
    Task<byte[]> GenerateInventoryReportPdfAsync();
}
