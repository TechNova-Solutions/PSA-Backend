using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PSA_Business_Logic.DbContextProvider;
using PSA_Business_Logic.Dtos;
using PSA_Business_Logic.Model;
using System.Text;

namespace PSA_Business_Logic.Service_Collection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomeServiceProviders(this IServiceCollection services, IConfiguration configuration)
        {
            //add database connection
            services.AddDbContext<PropertySellingDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PropertySellingConnectionString"));
            });

            //connect with angular project
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins(configuration.GetSection("ApplicationSettings:ClientUrl").Value)
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        );
            });

            //For Identity
            services.AddIdentityCore<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PropertySellingDbContext>()
                .AddDefaultTokenProviders();

            //configure Email services
            //var emailConfig = configuration
            //    .GetSection("EmailConfiguration")
            //    .Get<EmailConfiguration>();
            //services.AddSingleton(emailConfig);
            //services.AddScoped<IEmailService, EmailService>();

            //add config for Required Email
            services.Configure<IdentityOptions>(opt =>
               opt.SignIn.RequireConfirmedEmail = true);

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(3));




            //Change Password and username Policy
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            }
            );

            //inject appSettings
            services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));

            //JWT Authentication

            var key = Encoding.UTF8.GetBytes(configuration.GetSection("ApplicationSettings:JWT_Secret").Value);

            //Adding Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromMinutes(5),

                };
            });
        }
    }
}
