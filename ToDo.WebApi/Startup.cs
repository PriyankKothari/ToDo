using System.IO;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using ToDo.WebApi.Constraints;
using ToDo.WebApi.Filters;
using ToDo.Persistent.DbContexts;
using ToDo.Persistent.DbObjects;
using ToDo.Persistent.DbServices;
using ToDo.ServiceBus.MessageSenders;
using ToDo.WebApi.Validators;

namespace ToDo.WebApi
{
    public class Startup
    {
        private readonly string _swaggerDocsTitle = "ToDo API Swagger Docs";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddMvcCore().AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddMvc();
            services.AddApiVersioning(o => o.ReportApiVersions = true);
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // registering fluent validation, action filter & constraint for Enum
            services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
            services.AddMvc(options => options.Filters.Add(typeof(ValidatorActionFilter)));
            services.Configure<RouteOptions>(options =>
                options.ConstraintMap.Add("ItemStatus", typeof(ItemStatusEnumConstraint)));

            // registering AuthorisationDbContext with Identity
            services.AddDbContext<AuthorisationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AuthorisationConnectionString")))
                .AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<AuthorisationDbContext>();

            // registering Authentication
            services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = bool.Parse(Configuration["IdentityServer:RequireHttpsMetadata"]);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["IdentityServer:SecretKey"]))
                };
                options.Configuration = new OpenIdConnectConfiguration();
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy =
                    new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
            });

            // registering ToDoDbContext
            services.AddDbContext<ToDoDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ToDoConnectionString"));
            });

            // registering Services & validators
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<AuthorisationDbContext>();
            services.AddScoped<IToDoDbContext, ToDoDbContext>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IToDoService, ToDoService>();
            services.AddScoped<IEventStoreService, EventStoreService>();
            services.AddScoped<IMessageSender>(provider =>
                new MessageSender(Configuration["MicrosoftAzure:ServiceBus:ConnectionString"].ToString(),
                    Configuration["MicrosoftAzure:ServiceBus:Queue:ToDo:Name"].ToString()));
            services.AddTransient<IValidator<ToDoItem>, ToDoItemValidator>();

            // registering swagger
            services.AddSwaggerGen(options =>
            {
                foreach (var description in services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                // The name of the file is located in project properties -> build tab
                options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    "ToDo.Api.xml"));
            });
        }

        /// <summary>
        /// This method creates information for the current API version.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"{_swaggerDocsTitle} {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}