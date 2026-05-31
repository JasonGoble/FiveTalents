using FiveTalents.Domain.Giving;
namespace FiveTalents.Application.Giving.DTOs;

public record DonationDto(int Id, int? MemberId, string? MemberName, string FundName, decimal Amount, DateTime DonationDate, PaymentMethod PaymentMethod, DonationStatus Status);
public record DonationFundDto(int Id, string Name, string? Description, bool IsActive, bool IsDefault);
public record GivingSummaryDto(decimal TotalGiving, decimal TotalByFund, int DonorCount, DateTime PeriodStart, DateTime PeriodEnd);
