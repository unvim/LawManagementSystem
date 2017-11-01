using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ittechub.Saas.IDP
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddIdentityServer()
		.AddInMemoryClients(Clients.Get())
		.AddInMemoryIdentityResources(Resources.GetIdentityResources())
		.AddInMemoryApiResources(Resources.GetApiResources())
		.AddTestUsers(Users.Get())
		.AddDeveloperSigningCredential();

			services.AddMvc();
			
		}

	

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseIdentityServer();

			app.UseStaticFiles();
			app.UseMvcWithDefaultRoute();

			//app.Run(async (context) =>
			//{
			//	await context.Response.WriteAsync("Hello World!");
			//});
		}
	}

	internal class Clients
	{
		public static IEnumerable<Client> Get()
		{
			return new List<Client> {
			new Client {
				ClientId = "oauthClient",
				ClientName = "Example Client Credentials Client Application",
				AllowedGrantTypes = GrantTypes.ClientCredentials,
				ClientSecrets = new List<Secret> {
					new Secret("superSecretPassword".Sha256())},
				AllowedScopes = new List<string> {"customAPI.read"}
			}
		};
		}
	}

	internal class Resources
	{
		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource> {
			new IdentityResources.OpenId(),
			new IdentityResources.Profile(),
			new IdentityResources.Email(),
			new IdentityResource {
				Name = "role",
				UserClaims = new List<string> {"role"}
			}
		};
		}

		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource> {
			new ApiResource {
				Name = "customAPI",
				DisplayName = "Custom API",
				Description = "Custom API Access",
				UserClaims = new List<string> {"role"},
				ApiSecrets = new List<Secret> {new Secret("scopeSecret".Sha256())},
				Scopes = new List<Scope> {
					new Scope("customAPI.read"),
					new Scope("customAPI.write")
				}
			}
		};
		}

		
	}

	internal class Users
	{
		public static List<TestUser> Get()
		{
			return new List<TestUser> {
			new TestUser {
				SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
				Username = "scott",
				Password = "password",
				Claims = new List<Claim> {
					new Claim(JwtClaimTypes.Email, "scott@scottbrady91.com"),
					new Claim(JwtClaimTypes.Role, "admin")
				}
			}
		};
		}
	}
}
