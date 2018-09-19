using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Inkton.Nester;
using Websock.WebSocketManager;
using Websock.Database;
using Websock.Model;

namespace Websock
{   
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddApiVersioning(options => 
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1,0);
                    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                });
            services.AddNester(QueueMode.Server);
            services.AddNesterMySQL<MessageContext>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddWebSocketManager(); 
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);
            app.MapWebSocketManager("/LiveChat", serviceProvider.GetService<MessageHandler>());
            app.UseStaticFiles();
            app.UseMvc();

            DBinitialize.EnsureCreated(app.ApplicationServices);
        }
    }
}
