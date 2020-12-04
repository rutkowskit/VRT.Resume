using System;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VRT.Resume.Application;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Behaviours;
using VRT.Resume.Persistence.Data;
using VRT.Resume.Web.MappingProfiles;
using VRT.Resume.Web.Services;

namespace VRT.Resume.Web
{
    public static class Dependencies
    {
        public static void Register()
        {
            new ContainerBuilder()
                .RegisterLogger()
                .RegisterControllers()
                .RegisterDbContext()
                .RegisterMappings()
                .RegisterMediator()
                .RegisterMediatorPipelineBehaviours()
                .RegisterCurrentUserService()
                .RegisterServiceFactory()
                .RegisterImplementadInterfaces()
                .Build()
                .SetDependencyResolver()
                .CreateDbIfNotExists();
        }

        private static ContainerBuilder RegisterLogger(this ContainerBuilder builder)
        {
            builder.RegisterType<LoggingService>()
                .As<ILogger>()
                .SingleInstance();
            return builder;
        }

        private static ContainerBuilder RegisterControllers(this ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(Dependencies).Assembly);
            return builder;
        }

        private static ContainerBuilder RegisterDbContext(this ContainerBuilder builder)
        {
            builder.Register(ctx =>
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information);
                });
                var conString = ConfigurationManager.ConnectionStrings["AppDbContext:MSSQL"];
                var opt = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(conString.ConnectionString);
                //.UseLoggerFactory(loggerFactory);
                return opt.Options;
            }).SingleInstance();

            builder.RegisterType<AppDbContext>()
                    .InstancePerRequest();
            return builder;
        }

        private static ContainerBuilder RegisterImplementadInterfaces(this ContainerBuilder builder)
        {
            builder
                 .RegisterAssemblyTypes(typeof(SkillTypes).Assembly)
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
            builder.Register(ctx => new HttpContextWrapper(HttpContext.Current))
               .As<HttpContextBase>()
               .InstancePerRequest();

            builder.RegisterType<CurrentUserService>()
                .As<ICurrentUserService>()
                .InstancePerRequest();
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

        private static ContainerBuilder RegisterMappings(this ContainerBuilder builder)
        {
            builder.Register(context => new MapperConfiguration(cfg =>
            {
                //Register Mapper Profile
                cfg.AddProfile<AutoMapperProfile>();
            }
            )).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
            .As<IMapper>()
            .InstancePerLifetimeScope();
            return builder;
        }

        private static IContainer SetDependencyResolver(this IContainer container)
        {
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            return container;
        }

        private static void CreateDbIfNotExists(this IContainer container)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var options = scope.Resolve<DbContextOptions<AppDbContext>>();
                var ctx = new AppDbContext(options);
                if (ctx.Database.EnsureCreated())
                {
                    ctx.SeedSkillTypes();
                    ctx.SaveChanges();
                }
            }
        }

        private static AppDbContext SeedSkillTypes(this AppDbContext ctx)
        {
            ctx.SkillType.AddRange(Enum.GetNames(typeof(SkillTypes))
                .Select(skillName => AsSkillType(skillName))
                .Where(w => w != null)                
            );
            return ctx;
        }

        private static Domain.Entities.SkillType AsSkillType(string skillName)
        {
            return Enum.TryParse<SkillTypes>(skillName, out var skillType)
                ? new Domain.Entities.SkillType()
                {
                    SkillTypeId = (byte)skillType,
                    Name = skillName
                }                          
                : null;
        }
    }
}