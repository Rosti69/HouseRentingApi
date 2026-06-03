
using HouseRentingSystemApi.Data;
using HouseRentingSystemApi.Middleware;
using HouseRentingSystemApi.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HouseRentingSystemApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();

			builder.Services.AddCors(options => options.AddPolicy("FrontendPolicy", policy =>
			{
				policy
					.WithOrigins("http://localhost:5230")
					.AllowAnyHeader()
					.AllowAnyMethod();
			}
			));
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddHouseRentingData(builder.Configuration);

			//---NEW SECTION---
			var jwtSection = builder.Configuration.GetSection("Jwt");
			var key = jwtSection["Key"];

			builder.Services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,

						ValidIssuer = jwtSection["Issuer"],
						ValidAudience = jwtSection["Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
					};
				});
			builder.Services.AddAuthorization();

            var app = builder.Build();
			RolesSeeder.SeedAsync(app.Services).GetAwaiter().GetResult();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseCors("FrontendPolicy");
			app.UseHttpsRedirection();

			app.UseMiddleware<RequestTimingMiddleware>();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
