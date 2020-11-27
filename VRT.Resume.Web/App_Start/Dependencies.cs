using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Behaviours;
using VRT.Resume.Application.Persons.Queries.GetPersonEducation;
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
                .RegisterControllers()                       
                .RegisterDbContext()
                .RegisterMappings()
                .RegisterMediator()
                .RegisterMediatorPipelineBehaviours()
                .RegisterCurrentUserService()
                .RegisterServiceFactory()
                .RegisterImplementadInterfaces()
                .Build()
                .SetDependencyResolver();             
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
                var opt = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(@"Data Source=.\sql2; Initial Catalog=ResumeData; Integrated Security=True;MultipleActiveResultSets=true;");
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

        
        private static void SetDependencyResolver(this IContainer container)
        {
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}