using FluentValidation;
using TpClientsCommandes.Models;

namespace TpClientsCommandes.Validators
{
    public class CommandeValidator : AbstractValidator<Commande>
    {
        public CommandeValidator()
        {
            RuleFor(c => c.NumeroCommande)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(c => c.DateCommande)
                .LessThanOrEqualTo(DateTime.UtcNow);

            RuleFor(c => c.MontantTotal)
                .GreaterThan(0);

            RuleFor(c => c.Statut)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(c => c.ClientId)
                .GreaterThan(0);
        }
    }
}