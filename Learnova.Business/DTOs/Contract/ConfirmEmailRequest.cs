namespace Learnova.Business.DTOs.Contract
{
    public record ConfirmEmailRequest(
    string UserId,
    string Code
);
}
