using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ManyInOneAPI.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
