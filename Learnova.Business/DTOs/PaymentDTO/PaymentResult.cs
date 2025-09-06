namespace Learnova.Business.DTOs.PaymentDTO
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? TransactionId { get; set; }

        public static PaymentResult Success(string transactionId) => new()
        {
            IsSuccess = true,
            TransactionId = transactionId
        };

        public static PaymentResult Failure(string errorMessage) => new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
