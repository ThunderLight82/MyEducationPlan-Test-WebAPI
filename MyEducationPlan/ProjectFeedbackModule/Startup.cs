using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProjectFeedbackModule.Application.Services;
using ProjectFeedbackModule.Application.Services.Interfaces;
using ProjectFeedbackModule.DataAccess;
using ProjectFeedbackModule.Domain;

namespace ProjectFeedbackModule;

public class Startup
{
    private IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextPool<ProjectModuleDbContext>(options => options
            .UseSqlServer(Configuration.GetConnectionString("SQLServer")));

        RegisterServices(services);

        services.AddMvc().AddNewtonsoftJson(options => 
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectFeedbackModule", Version = "v1" });
        });
    }
    
    private void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ICsvService, CsvService>();
        
        services.Configure<FeedbackSettings>(Configuration.GetSection("FeedbackSettings"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=GetProjects}/{projectId?}"
            );

            endpoints.MapControllerRoute(
                name: "Feedbacks",
                pattern: "Home/ProjectFeedbacksList/{projectId}",
                defaults: new { controller = "Home", action = "GetProjects" }
            );
        });
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectFeedbackModule v1");
        });
    }
}