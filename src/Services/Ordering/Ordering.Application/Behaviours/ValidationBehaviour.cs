using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Ordering.Application.Exceptions.ValidationException;
namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        // validation handler je preprocessor behavior prokz prolazi svaki request i u slučaju grešaka, onda baci grešku
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {

            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                // koristimo task object, kad sve završimo vracamo sve validatore
                // odabiremo sve validatore
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                // provjeravamo ako ima errora
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }

            // moramo vratiti next kako bi nastavili request pipeline for the request handle method in MediatR
            // ako nema grešaka nastavlja dalje na handler
            return await next();
        }
    }
}
