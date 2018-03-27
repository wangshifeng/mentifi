using System;
using System.Linq;
using Hub3c.ApiMessage;
using Hub3c.Mentify.AccessInternalApi;
using Hub3c.Mentify.AccessInternalApi.Implementations;
using Hub3c.Mentify.API.Filters;
using Hub3c.Mentify.Core.RestApiClient;
using Hub3c.Mentify.Core.Serialization.Deserializers;
using Hub3c.Mentify.Messaging.Handler;
using Hub3c.Mentify.Messaging.Messages.Commands;
using Hub3c.Mentify.MongoRepo;
using Hub3c.Mentify.MongoRepo.Model;
using Hub3c.Mentify.MongoRepo.Repository;
using Hub3c.Mentify.Repository;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Implementations;
using Hub3c.Messaging.Message;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using MongoDB.Driver;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Hub3c.Mentify.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(Configuration)
              .CreateLogger();

            Log.Information("============================ Application Started ============================");
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddDbContext<MentifiContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("Hub3cConnection"))).AddUnitOfWork<MentifiContext>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["AuthUrl"];
                    options.RequireHttpsMetadata = false;
                });

            services.AddScoped<IDeserializer, JsonDeserializer>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IApiClient, ApiClient>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<ISystemUserDeviceService, SystemUserDeviceService>();
            services.AddScoped<IHub3cFirebaseApi, Hub3CFirebaseApi>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailApi, EmailApi>();
            services.AddScoped<IBulletinService, BulletinService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IBusInstance, BusInstance>();
            services.AddScoped<IGoalService, GoalService>();
            services.AddScoped<IGoalProgressService, GoalProgressService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IMessageBoardService, MessageBoardService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectTeamService, ProjectTeamService>();
            services.AddScoped<IAdminInvitationService, AdminInvitationService>();
            services.AddScoped<IInvitationLinkService, InvitationLinkService>();
            services.AddScoped<IExperienceService, ExperienceService>();
            services.AddScoped<IInformalExperienceService, InformalExperienceService>();
            services.AddScoped<ISubjectExperienceService, SubjectExperienceService>();
            services.AddScoped<ISubjectPreferenceService, SubjectPreferenceService>();
            services.AddScoped<IAdditionalActivityService, AdditionalActivityService>();
            services.AddScoped<IUserAddressService, UserAddressService>();
            services.AddScoped<IEducationService, EducationService>();

            var mongoOpt = Configuration.GetSection("MongoDbConfig");
            services.AddSingleton<IMongoClient>(new MongoClient(mongoOpt.GetSection("Host").Value));
            services.AddSingleton<IMongoRepository<Resource>, MongoRepository<Resource>>();

            // For Rabbit Subscriber
            var rabbitUsername = Configuration.GetSection("MessagingConfig")["RabbitUsername"];
            var rabbitPassword = Configuration.GetSection("MessagingConfig")["RabbitPassword"];
            var rabbitHost = Configuration.GetSection("MessagingConfig")["RabbitServer"];
            var rabbitVHost = Configuration.GetSection("MessagingConfig")["RabbitVHost"];

            var subscriber = new RabbitSubscriber(rabbitUsername, rabbitPassword, rabbitHost, rabbitVHost);
            var rabbitBus = new RabbitBus(rabbitHost, rabbitUsername, rabbitPassword, rabbitVHost);
            var busInstance = new BusInstance(rabbitBus);
            services.AddSingleton(subscriber);
            services.AddSingleton(rabbitBus);
            services.AddSingleton(busInstance);
            services.AddMvc(op =>
            {
                op.Filters.Add(new ValidateModelStateAttribute());
            });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Mentify API", Version = "v1" });
                c.SwaggerDoc("v2", new Info { Title = "Mentify API", Version = "v2" });

                c.DocInclusionPredicate((version, apiDesc) =>
                {
                    var versions = apiDesc.ControllerAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var values = apiDesc.RelativePath
                        .Split('/');
                    values[1] = version;
                    apiDesc.RelativePath = string.Join("/", values);

                    var versionParameter = apiDesc.ParameterDescriptions
                        .SingleOrDefault(p => p.Name == "version");

                    if (versionParameter != null)
                        apiDesc.ParameterDescriptions.Remove(versionParameter);

                    //return true;

                    return versions.Any(v => $"v{v.ToString()}" == version);

                });
                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please insert JWT with Bearer into field",
                        Name = "Authorization",
                        Type = "apiKey",
                    });

                c.IncludeXmlComments(GetXmlCommentsPath());
                c.DescribeAllEnumsAsStrings();
                c.DescribeAllParametersInCamelCase();
            });
            services.AddCors();
            services.AddApiVersioning(
                o =>
                {
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = new ApiVersion(new DateTime(2018, 2, 6), 1, 0);
                });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));


            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                // rabbit subscriber
                var subscriber = (RabbitSubscriber)scope.ServiceProvider.GetService(typeof(RabbitSubscriber));
                var busInstance = (BusInstance)scope.ServiceProvider.GetService(typeof(BusInstance));

                busInstance.Declare(typeof(ResourceData));
                busInstance.Declare(typeof(MobileAppNotification));

                subscriber.Subscribe<ResourceData>(action =>
                {
                    var mongo =
                        (IMongoRepository<Resource>)app.ApplicationServices.GetService(typeof(IMongoRepository<Resource>));
                    var handler = new ResourceHandler(mongo);
                    handler.SaveAllResource(action);
                });

                subscriber.Subscribe<MobileAppNotification>(action =>
                {
                    scope.ServiceProvider.GetService<INotificationService>().SendNotificationFromWeb(action);
                });
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mentifi V1"); c.SwaggerEndpoint("/swagger/v2/swagger.json", "Mentifi V2"); });

            app.UseCors(a => a.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseMvc();
        }

        private string GetXmlCommentsPath()
        {
            var app = PlatformServices.Default.Application;
            return System.IO.Path.Combine(app.ApplicationBasePath, app.ApplicationName + ".xml");
        }
    }
}