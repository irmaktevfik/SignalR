using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using bidmobileservicesService.Models;
using System.Data.Entity;
using Microsoft.WindowsAzure.Mobile.Service;

namespace bidmobileservicesService.Hubs
{
    public class BidHub : Hub
    {
        public ApiServices Services { get; set; }
        public async void PushBid(string bidItemId)
        {
            using (bidmobileservicesContext context = new bidmobileservicesContext())
            {
                //Get Selected Bid Item
                var data = context.BidItems.Where(p => p.Id == bidItemId).First();
                //Increment the bid Amount by 10
                data.BidAmount += 10;
                //save changes
                context.SaveChanges();

                //lets Push our notification
                string wnsToast = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><toast><visual><binding template=\"" + data.ItemName.ToString() 
                                                + "\"><text id=\"1\">{0}</text></binding></visual></toast>", data.BidAmount.ToString());
                WindowsPushMessage message = new WindowsPushMessage();
                message.XmlPayload = wnsToast;
                await Services.Push.SendAsync(message);


                //get all bids
                GetBids(string.Empty);
            }
        }

        public string GetBids(string test)
        {
            using (bidmobileservicesContext context = new bidmobileservicesContext())
            {
                string jsonData = string.Empty;
                //get all items list
                var data = context.BidItems.ToList();
                //convert out list to json 
                jsonData = JsonConvert.SerializeObject(data);
                //send data to all clients on event "message"
                Clients.All.Message(jsonData);
                return jsonData;
            }
        }
    }
}