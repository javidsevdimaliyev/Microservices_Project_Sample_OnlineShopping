﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OrderService.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));

        return services;
    }
}
