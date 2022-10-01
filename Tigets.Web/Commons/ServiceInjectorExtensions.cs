﻿using Tigets.Core.Models;
using Tigets.Core.Repositories;
using Tigets.Core.Services;
using Tigets.Infrastructure.Repositories;

namespace Tigets.Web.Commons
{
    public static class ServiceInjectorExtensions
    {
        public static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IAccountService, AccountService>();
        }
    }
}
