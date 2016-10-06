using Moneta.Domain.ValueObjects;

namespace Moneta.Domain.Interfaces.Validation
{
    public interface ISelfValidator
    {
        ValidationResult ResultadoValidacao { get; }
        bool IsValid(); 
    }
}