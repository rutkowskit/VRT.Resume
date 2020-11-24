using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;
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
                .RegisterMediator()
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
        private static void SetDependencyResolver(this IContainer container)
        {
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}