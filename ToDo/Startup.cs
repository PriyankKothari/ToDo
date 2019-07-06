using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using ToDo.Constraints;
using ToDo.Models;
using ToDo.Persistent.DbContexts;
using ToDo.Persistent.DbServices;
using ToDo.ServiceBus.MessageSenders;

namespace ToDo
{
    public class Startup
    {
        private readonly string _swaggerDocsPath = "api-docs";
        private readonly string _swaggerDocsTitle = "ToDo API Swagger Docs";
        private readonly string _swaggerDocsVersion = "v1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            // injecting AuthorisationDbContext with Identity
            services.AddDbContext<AuthorisationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AuthorisationConnectionString")))
                .AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthorisationDbContext>();

           // injecting ToDoDbContext
           services.AddDbContext<ToDoDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ToDoConnectionString"));
            });

            //dependency injection for Services
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<IToDoDbContext, ToDoDbContext>();
            services.AddScoped<IToDoService, ToDoService>();
            services.AddScoped<IMessageSender>(provider =>
                new MessageSender(Configuration["MicrosoftAzure:ServiceBus:ConnectionString"].ToString(),
                    Configuration["MicrosoftAzure:ServiceBus:Queue:ToDo:Name"].ToString()));

            services.Configure<RouteOptions>(options =>
                options.ConstraintMap.Add("ItemStatus", typeof(ItemStatusEnumConstraint)));

            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                // The name of the file is located in project properties -> build tab
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    "ToDo.Api.xml");
                options.IncludeXmlComments(filePath);
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