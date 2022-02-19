using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Errors.Validation;

[Serializable]
[Immutable]
public class ValidationErrorDetails
{
    public ValidationErrorDetails(params ValidationFailure[] failures)
    {
        if (failures == null)
        {
            throw new ArgumentNullException(nameof(failures));
        }

        Failures = failures.ToList();
    }

    public List<ValidationFailure> Failures { get; }

    public override string ToString()
    {
        return string.Join(", ", Failures.Select(x => $"{x.Key}: {x.Rule}"));
    }
}