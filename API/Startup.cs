using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Identity;
using FluentValidation.AspNetCore;
using Infrastructure.Security;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.Middleware;
using AutoMapper;
using Infrastructure.Photos;
using API.SignalR;
using System.Threading.Tasks;
using Application.Profiles;
using System;

namespace API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
      services.AddDbContext<DataContext>(opt =>
      {
        opt.UseLazyLoadingProxies();
        opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
      });

      ConfigureServices(services);
    }
    public void ConfigureProductionServices(IServiceCollection services)
    {
      services.AddDbContext<DataContext>(opt =>
      {
        opt.UseLazyLoadingProxies();
        opt.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
      });

      ConfigureServices(services);
    }
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(Options =>
      {
        Options.AddPolicy("CorsPolicy", policy =>
        {
          policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("WWW-Authenticate")
            .WithOrigins("http://localhost:3000")
            .AllowCredentials();
        });
      });
      services.AddMediatR(typeof(List.Handler).Assembly);
      services.AddAutoMapper(typeof(List.Handler));
      services.AddSignalR();
      services.AddMvc(opt =>
      {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        opt.Filters.Add(new AuthorizeFilter(policy));
      })
          .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Create>())
          .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      var builder = services.AddIdentityCore<AppUser>();
      var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
      identityBuilder.AddEntityFrameworkStores<DataContext>();
      identityBuilder.AddSignInManager<SignInManager<AppUser>>();

      services.AddAuthorization(opt =>
      {
        opt.AddPolicy("IsActivityHost", policy =>
              {
                policy.Requirements.Add(new IsHostRequirement());
              });
      });
      services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(opt =>
          {
            opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = key,
              ValidateAudience = false,
              ValidateIssuer = false,
              ValidateLifetime = true,
              ClockSkew = TimeSpan.Zero
            };
            opt.Events = new JwtBearerEvents
            {
              OnMessageReceived = context =>
              {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                {
                  context.Token = accessToken;
                }
                return Task.CompletedTask;
              }
            };
          });

      services.AddScoped<IJwtGenerator, JwtGenerator>();
      services.AddScoped<IUserAccessor, UserAccessor>();
      services.AddScoped<IPhotoAccessor, PhotoAccessor>();
      services.AddScoped<IProfileReader, ProfileReader>();
      services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseMiddleware<ErrorHandlingMiddleware>();
      if (env.IsDevelopment())
      {
        // app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        // app.UseHsts();
      }

      app.UseXContentTypeOptions();
      app.UseReferrerPolicy(opt => opt.NoReferrer());
      app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
      app.UseXfo(opt => opt.Deny());
      app.UseCsp(opt => opt
        .BlockAllMixedContent()
        .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com", "sha256-F4GpCPyRepgP5znjMD8sc7PEjzet5Eef4r09dEGPpTs="))
        .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
        .FormActions(s => s.Self())
        .FrameAncestors(s => s.Self())
        .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com", "blob:", "data:"))
        .ScriptSources(s => s.Self().CustomSources("sha256-ma5XxS1EBgt17N22Qq31rOxxRWRfzUTQS1KOtfYwuNo="))
      );

      // app.UseHttpsRedirection();
      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseAuthentication();
      app.UseCors("CorsPolicy");
      app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chat"); });
      app.UseMvc(routes => 
      {
        routes.MapSpaFallbackRoute(
          name: "spa-fallback",
          defaults: new {controller = "Fallback", action = "Index"}
        );
      });
    }
  }
}
