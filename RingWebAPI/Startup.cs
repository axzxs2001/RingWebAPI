using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
#if UseSwagger
using Microsoft.OpenApi.Models;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

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
            #region ������Է��ڻ����У�������ݿ��ѯ,Ȼ��ע�뵽������
            var permissions = new List<Permission> {
                              new Permission {  Url="/adminapi",  Credentials="admin",Method="GET"},
                              new Permission {  Url="/adminapi",  Credentials="admin",Method="GET"},
                              new Permission {  Url="/systemapi", Credentials="system",Method="GET"},
                              new Permission {  Url="/systemapi", Credentials="system",Method="GET"}
                          };
            services.AddSingleton(permissions);
            #endregion

            services.AddAuthorization();
#endif


#if UseSwagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RingWebAPI", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "RingWebAPI.xml");
                c.IncludeXmlComments(filePath);
            });
#endif
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
#if UseSwagger
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RingWebAPI v1"));
#endif
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

#if (!NoAuthenticate)
        /// <summary>
        /// ���Ȩ����֤
        /// </summary>
        /// <param name="services"></param>
        void AddAuthorization(IServiceCollection services)
        {
            //��ȡ�����ļ�
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,

            };
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            //�����������������ClaimTypes.Role�����漯�ϵ�ÿ��Ԫ�ص�NameΪ��ɫ���ƣ����ClaimTypes.Name�������漯�ϵ�ÿ��Ԫ�ص�NameΪ�û���
            var permissionRequirement = new PermissionRequirement(
                "/api/denied",
                ClaimTypes.Role,
                audienceConfig["Issuer"],
                audienceConfig["Audience"],
                signingCredentials,
                expiration: TimeSpan.FromSeconds(1000000)//����Token����ʱ��
                );

            services.AddAuthorization(options =>
            {
#if Policy
                options.AddPolicy("Permission", policy => policy.AddRequirements(permissionRequirement));
#endif
            }).
            AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                //��ʹ��https
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = tokenValidationParameters;
            });
#if Policy
            //ע����ȨHandler
#endif
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);
        }
#endif
        }
    }
