using System.Collections.Generic;

namespace IdMatrixSoapLib.Models
{
    public class IdMatrixResponse
    {
        public string MessageId { get; set; } = string.Empty;
        public string ClientReference { get; set; } = string.Empty;
        public string OverallOutcome { get; set; } = string.Empty;
        public string VerificationOutcome { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalPoints { get; set; }
        public List<VerificationResult> VerificationResults { get; set; } = new();
        public List<ErrorDetail> Errors { get; set; } = new();
    }

    public class VerificationResult
    {
        public string Type { get; set; } = string.Empty;
        public string Outcome { get; set; } = string.Empty;
        public decimal Points { get; set; }
        public string Details { get; set; } = string.Empty;
    }

    public class ErrorDetail
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
    }
}
