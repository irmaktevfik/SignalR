using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bidmobileservicesService.DataObjects
{
    public class BidItems : EntityData
    {
        public string Id { get; set; }

        public string ItemName { get; set; }

        public int BidAmount { get; set; }
    }
}