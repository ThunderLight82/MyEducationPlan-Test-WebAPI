using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyEducationPlan.DataAccess;

namespace MyEducationPlan;

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
        services.AddDbContextPool<EducationPlanDbContext>(options => options
            .UseSqlServer(Configuration.GetConnectionString("SQLServer")));

        RegisterServices(services);

        services.AddMvc().AddNewtonsoftJson(options => 
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyEducationPlan", Version = "v1" });
        });
    }
    
    private void RegisterServices(IServiceCollection services)
    {
        // Future Service registration TODO
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
        
        // Future Endpoints for HomePage TODO
        app.UseEndpoints(endpoints =>
        {
            
        });
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniversityManagement.WebApi");
        });
    }
}