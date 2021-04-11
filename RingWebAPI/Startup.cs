using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#if (UseSwagger)
using System.IO;
using Microsoft.OpenApi.Models;
#endif
#if (!NoAuthenticate)
using RingWebAPI.Models;
using RingWebAPI.Services;
#endif
#if (Policy)
using System.Collections.Generic;
#endif
namespace RingWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
#if (Policy)
#region 这里可以放在缓存中，或从数据库查询,然后注入到容器中
            var permissions = new List<Permission> {
                              new Permission {  Url="/adminapi",  Credentials="admin",Method="GET"},
                              new Permission {  Url="/adminapi",  Credentials="admin",Method="POST"},
                              new Permission {  Url="/systemapi", Credentials="system",Method="GET"},
                              new Permission {  Url="/systemapi", Credentials="system",Method="POST"}
                          };
            services.AddSingleton(permissions);
#endregion
#endif
#if (!NoAuthenticate)
            var audience = new AudienceModel();
            Configuration.GetSection("AudienceModel").Bind(audience);
          
            services.AddAuthorization(audience);
            services.AddScoped<IUserService, UserService>();
            services.AddAuthorization();
#endif
#if (UseSwagger)
           services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RingWebAPI", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "RingWebAPI.xml");
                c.IncludeXmlComments(filePath);
#if (!NoAuthenticate)
                var schemeName = "Bearer";
                //如果用Token验证，会在Swagger界面上有验证              
                c.AddSecurityDefinition(schemeName, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "请输入不带有Bearer的Token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = schemeName.ToLowerInvariant(),
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = schemeName
                            }
                        },
                        new string[0]
                    }
                });
#endif
            });
#endif
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
#if (UseSwagger)
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RingWebAPI v1"));
#endif
            }
#if (!NoAuthenticate)
            app.UseAuthentication();
#endif
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
