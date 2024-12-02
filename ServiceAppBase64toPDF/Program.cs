using ServiceAppBase64toPDF;
using ServiceAppBase64toPDF.Application;
using ServiceAppBase64toPDF.Infrastructure;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        // Register application services and repositories
        services.AddTransient<IimagetoPDFService, ImagetoPDFService>();
        services.AddTransient<IScannedSIService, ScannedSIService>();

        services.AddTransient<IQF_Scanned_SIRepository, QF_Scanned_SIRepository>();

        services.AddHostedService<Worker>();

        // Check if it's in development or production
        var env = context.HostingEnvironment;
        if (env.IsDevelopment())
        {
            // Development-specific services or configurations
        }
        else if (env.IsProduction())
        {
            // Production-specific services or configurations
        }
    })
    .Build();

await host.RunAsync();
