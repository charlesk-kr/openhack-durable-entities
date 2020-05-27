using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace InventoryManagement
{
    public class ShipmentEvent
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("divisionId")]
        public string DivisionId { get; set; }
        [JsonProperty("storeId")]
        public string StoreId { get; set; }
        [JsonProperty("distributionId")]
        public string DistributionId { get; set; }
        [JsonProperty("items")]
        public List<ShipmentItem> Items { get; set; }
    }

    public class ShipmentItem
    {
        [JsonProperty("upc")]
        public string Upc { get; set; }
        [JsonProperty("shipmentAmount")]
        public int ShipmentAmount { get; set; }
    }
}