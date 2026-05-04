using System.ComponentModel.DataAnnotations;

namespace Assessment.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NotWhitespaceAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
        => value is not string s || !string.IsNullOrWhiteSpace(s);

    public override string FormatErrorMessage(string name)
        => $"{name} mag niet leeg of alleen witruimte zijn.";
}
