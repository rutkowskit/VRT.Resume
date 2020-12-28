using Autofac;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Behaviours;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Fakes
{
    internal static class FakeDependencies
    {
        internal static IContainer Register()
        {
            return new ContainerBuilder()
                .RegisterImplementadInterfaces()
                .RegisterDbContext()                
                .RegisterMediator()
                .RegisterMediatorPipelineBehaviours()
                .RegisterCurrentUserService()
                .RegisterServiceFactory()                
                .RegisterDateTimeService()
                .Build();                
        }

        private static ContainerBuilder RegisterDateTimeService(this ContainerBuilder builder)
        {
            builder
                 .RegisterType<FakeDateTimeService>()
                 .AsImplementedInterfaces();
            return builder;
        }

        private static ContainerBuilder RegisterDbContext(this ContainerBuilder builder)
        {
            builder.Register(ctx =>
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                return options;                
            }).InstancePerLifetimeScope();

            builder.RegisterType<AppDbContext>()
                    .InstancePerLifetimeScope();
            return builder;
        }

        private static ContainerBuilder RegisterImplementadInterfaces(this ContainerBuilder builder)
        {
            builder
                 .RegisterAssemblyTypes(typeof(IDateTimeService).Assembly)
                 .AsImplementedInterfaces();
            return builder;
        }

        private static ContainerBuilder RegisterServiceFactory(this ContainerBuilder builder)
        {
            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            }).InstancePerLifetimeScope();
            return builder;
        }

        private static ContainerBuilder RegisterCurrentUserService(this ContainerBuilder builder)
        {
            builder.RegisterType<FakeCurrentUserService>()
                .As<ICurrentUserService>()
                .InstancePerDependency();
            return builder;
        }

        private static ContainerBuilder RegisterMediator(this ContainerBuilder builder)
        {
            builder
              .RegisterType<Mediator>()
              .As<IMediator>()
              .InstancePerLifetimeScope();
            return builder;
        }

        private static ContainerBuilder RegisterMediatorPipelineBehaviours(this ContainerBuilder builder)
        {
            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IRequestExceptionHandler<,,>),
                typeof(IRequestExceptionAction<,>),
                typeof(INotificationHandler<>),
            };

            var appAssembly = typeof(ValidationBehaviour<,>).Assembly;
            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(appAssembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            // It appears Autofac returns the last registered types first
            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestExceptionActionProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestExceptionProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidationBehaviour<,>)).As(typeof(IPipelineBehavior<,>));

            return builder;
        }              
    }
}
