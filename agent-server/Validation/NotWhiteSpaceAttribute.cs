using System.ComponentModel.DataAnnotations;

namespace AgentServer.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NotWhiteSpaceAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string text)
        {
            return true;
        }

        return string.IsNullOrWhiteSpace(text) == false;
    }
}