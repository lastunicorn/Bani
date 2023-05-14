// Bani
// Copyright (C) 2022 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.IO;
using System.Reflection;
using Autofac;
using DustInTheWind.Bani.Cli.Application.PresentIssuers;
using DustInTheWind.Bani.Cli.Presentation;
using DustInTheWind.Bani.DataAccess;
using DustInTheWind.Bani.DataAccess.Port;
using DustInTheWind.ConsoleTools.Commando.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Microsoft.Extensions.Configuration;

namespace DustInTheWind.Bani;

internal class SetupServices
{
    public static IContainer BuildContainer()
    {
        ContainerBuilder containerBuilder = new();
        ConfigureServices(containerBuilder);

        return containerBuilder.Build();
    }

    private static void ConfigureServices(ContainerBuilder containerBuilder)
    {
        containerBuilder
            .Register(builder =>
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                return configuration;
            })
            .AsSelf()
            .SingleInstance();

        Assembly applicationAssembly = typeof(PresentIssuersRequest).Assembly;
        MediatRConfiguration mediatRConfiguration = MediatRConfigurationBuilder.Create(applicationAssembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithRegistrationScope(RegistrationScope.Scoped) // currently only supported values are `Transient` and `Scoped`
            .Build();
        containerBuilder.RegisterMediatR(mediatRConfiguration);

        containerBuilder.RegisterType<BaniDbContext>().AsSelf();
        containerBuilder
            .Register(builder =>
            {
                IConfiguration configuration = builder.Resolve<IConfiguration>();
                string databasePath = configuration["DatabasePath"];
                return new BaniDbContext(databasePath);
            })
            .AsSelf();
        containerBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        Assembly presentationAssembly = typeof(PresentIssuersCommand).Assembly;
        containerBuilder.RegisterCommando(presentationAssembly);
    }
}