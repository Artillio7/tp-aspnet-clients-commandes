using FluentValidation;
using TpClientsCommandes.Models;

namespace TpClientsCommandes.Validators
{
    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator()
        {
            RuleFor(c => c.Nom)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(c => c.Prenom)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(c => c.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(c => c.Telephone)
                .MaximumLength(30);

            RuleFor(c => c.Adresse)
                .MaximumLength(300);

            RuleFor(c => c.DateCreation)
                .LessThanOrEqualTo(DateTime.UtcNow);
        }
    }
}