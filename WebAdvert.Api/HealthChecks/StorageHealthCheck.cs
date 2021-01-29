using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebAdvert.Api.Services;

namespace WebAdvert.Api.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        public readonly IAdvertStorageService _advertStorageService;

        public StorageHealthCheck(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
        }

        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default)
        {
            var isStorageOk = await _advertStorageService.CheckHealthAsync();
            return isStorageOk ? HealthCheckResult.Healthy("") : HealthCheckResult.Unhealthy("");
        }
    }
}
