using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ManyInOneAPI.Health
{
    public class GenAIHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // TODO : To implement
            throw new NotImplementedException();
        }
    }
}
