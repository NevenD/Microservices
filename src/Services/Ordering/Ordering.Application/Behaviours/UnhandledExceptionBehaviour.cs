using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                // ne trebamo stavljati validaciju unutar svakog zasebnog handlea (commands or queries), nego wrapamo globalni handle sa try catch metodom
                return await next();
            }
            catch (Exception ex)
            {
                // dobivamo informacije o klasi
                // pohranjujemo naš exception unutar pipeline behviour
                var requestName = typeof(TRequest).Name;

                _logger.LogError(ex, "Application request: Unhandled exception for Request {Name} {@request}", requestName, request);
                throw;
            }
        }
    }
}
