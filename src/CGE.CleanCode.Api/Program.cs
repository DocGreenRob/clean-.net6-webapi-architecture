using AutoMapper;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Cge.Core.Logging;
using FluentAssertions.Common;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using CGE.CleanCode.Api.Middlewear;
using CGE.CleanCode.Api.Models.Authorization;
using CGE.CleanCode.Api.Models.Validators;
using CGE.CleanCode.Common.Models;
using CGE.CleanCode.Service;
using CGE.CleanCode.Service.Interfaces;
using CGE.CleanCode.Service.Mappings;
using CGE.CleanCode.Service.Services;
using CGE.CleanCode.ServiceBus;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

[assembly: System.Reflection.AssemblyVersion("1.0.*")]
namespace CGE.CleanCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            var configuration = builder.Configuration;

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Host.ConfigureAppConfiguration((context, config) =>
            {
                var settings = config.Build();
                var keyVaultURL = settings["KeyVault:Vault"];
                var keyVaultClientId = settings["KeyVault:ClientId"];
                var keyVaultClientSecret = settings["KeyVault:ClientSecret"];
                config.AddAzureKeyVault(keyVaultURL, keyVaultClientId, keyVaultClientSecret, new DefaultKeyVaultSecretManager());
            });


            string serviceBusDbConn = builder.Configuration.GetSection("ServiceBusConnectionString").Value;

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddAutoMapper(typeof(Program));
            var mapperConfiguration = new MapperConfiguration((x) =>
            {
                x.AddProfile<EntityToDtoProfiles>();
            });

            builder.Services.AddSingleton<IMapper>(x => mapperConfiguration.CreateMapper());

            builder.Services.AddSingleton<ITelemetryClient, TelemetryClientWrap>();
            builder.Services.AddSingleton<IMongoDbWrapper>(x => new MongoDbWrapper<Dummy>(x.GetRequiredService<IConfiguration>(),
                                                                                        x.GetRequiredService<ITelemetryClient>(),
                                                                                        "Dummy"));
            builder.Services.AddSingleton<IRoleService, RoleService>();
            builder.Services.AddSingleton<IRolePermissionService, RolePermissionService>();
            builder.Services.AddSingleton<IRouteService, RouteService>();
            builder.Services.AddSingleton<IUserService, UserService>();

            builder.Services.AddTransient<IMessagePublisher>(s => new MessagePublisher(configuration[serviceBusDbConn]));
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddSingleton<ICacheConfigruationMangementService, CacheConfigruationMangementService<Dummy>>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CGE.CleanCode.Api", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
                c.ExampleFilters();
                c.CustomSchemaIds(type => type.ToString());
            });

            builder.Services.AddApiVersioning(options =>
                                                {
                                                    options.ReportApiVersions = true;
                                                    options.AssumeDefaultVersionWhenUnspecified = true;
                                                    options.DefaultApiVersion = new ApiVersion(1, 0);
                                                });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddSwaggerGenNewtonsoftSupport();

            builder.Services.AddMvc().AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
                    x.JsonSerializerOptions.PropertyNamingPolicy = null;
                }).AddFluentValidation(x =>
                            {
                                //x.RegisterValidatorsFromAssemblyContaining<ExerciseValidator>();
                                x.DisableDataAnnotationsValidation = true;
                                x.ImplicitlyValidateChildProperties = true;
                            }).AddNewtonsoftJson(x =>
                            {
                                x.UseMemberCasing();
                                x.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                            });

            //builder.Services.AddMvc().AddJsonOptions(x =>
            //	{
            //		x.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
            //		x.JsonSerializerOptions.PropertyNamingPolicy = ;
            //	});

            //builder.Services.AddFluentValidationAutoValidation(x => { 
            //	x.Register.AddFluentValidationClientsideAdapters()
            //});
            //builder.Services.AddFluentValidationClientsideAdapters(x => { 

            //});

            builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

            //builder.Services.AddAuthentication()
            //                                .AddMicrosoftAccount(o =>
            //                                {
            //                                    o.ClientId = "";
            //                                    o.ClientSecret = "";
            //                                })
            //                                .AddGoogle(o =>
            //                                {

            //                                })
            //                                .AddTwitter(o => { })
            //                                .AddFacebook(o => { });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // JWT Bearer authentication options
            })
            .AddGoogle(options =>
            {
                options.ClientId = "318379590121-hc38q5ddni530ro728ee6sk96nqod69b.apps.googleusercontent.com";
                options.ClientSecret = "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";
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });

            // Replace the default authorization policy provider with our own
            // custom provider which can return authorization policies for given
            // policy names (instead of using the default policy provider)
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, ApiPolicyProvider>();
            //// As always, handlers must be provided for the requirements of the authorization policies
            builder.Services.AddSingleton<IAuthorizationHandler, ApiAuthorizationHandler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseSwagger();
                //app.UseSwaggerUI();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            var telemetryClient = app.Services.GetService<ITelemetryClient>();
            // https://auth0.com/docs/quickstart/webapp/aspnet-core/02-user-profile
            //app.UseMiddleware<UserMiddleware>(applicationContext);
            app.UseMiddleware<ExceptionMiddleware>(telemetryClient);
            app.UseMiddleware<AntiXssMiddleware>(telemetryClient);
            //app.UseMiddleware<ApiKeyMiddleware>();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }

}