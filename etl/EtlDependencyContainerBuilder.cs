using Autofac;
using chase;
using core.Configuration;
using core.Database;
using core.Db;
using core.Dependencies;
using discord;
using firefly_iii;
using Microsoft.Extensions.Logging;
using ses;
using sqs;

namespace core;

public class EtlDependencyContainerBuilder : IDependencyContainerBuilder
{
	public static void RegisterContainer()
	{
		var builder = new ContainerBuilder();
		
		new EtlDependencyContainerBuilder().RegisterDependencies(builder);
		new SqsDependencyBuilder().RegisterDependencies(builder);
		new ChaseDependencyBuilder().RegisterDependencies(builder);
		new SesDependencyBuilder().RegisterDependencies(builder);
		new FireflyDependencyBuilder().RegisterDependencies(builder);
		new DiscordDependencyBuilder().RegisterDependencies(builder);

		DependencyContainer.Instance =  builder.Build();
	}

	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<DatabaseContext>().As<DatabaseContext>().InstancePerLifetimeScope();
		builder.RegisterType<Database.Db>().As<IDb>().InstancePerLifetimeScope();
		builder.RegisterType<EnvironmentVariableConfiguration>().As<IEnvironmentVariableConfiguration>();
		builder.RegisterType<EventLoop>().As<EventLoop>().SingleInstance();
		
		builder.RegisterInstance(LoggerFactory.Create(
				logBuilder =>
				{
					logBuilder.AddConsole();
				}))
			.As<ILoggerFactory>();

		builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>));
	}
}