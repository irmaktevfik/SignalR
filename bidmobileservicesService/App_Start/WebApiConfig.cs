using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using bidmobileservicesService.DataObjects;
using bidmobileservicesService.Models;
using Microsoft.WindowsAzure.Mobile.Service.Config;

namespace bidmobileservicesService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            //Register SignalR
            SignalRExtensionConfig.Initialize();

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            Database.SetInitializer(new bidmobileservicesInitializer());
        }
    }

    public class bidmobileservicesInitializer : ClearDatabaseSchemaIfModelChanges<bidmobileservicesContext>
    {
        protected override void Seed(bidmobileservicesContext context)
        {
            List<BidItems> bidItems = new List<BidItems>
            {
                new BidItems { Id = "1", ItemName="Car", BidAmount = 0},
                new BidItems { Id = "2", ItemName="House", BidAmount = 0}
            };

            foreach (BidItems Item in bidItems)
            {
                context.Set<BidItems>().Add(Item);
            }

            base.Seed(context);
        }
    }
}

