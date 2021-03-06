using System;
using Newtonsoft.Json;

namespace InventoryManagement
{
    public class InventoryEvent
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("divisionId")]
        public string DivisionId { get; set; }
        [JsonProperty("storeId")]
        public string StoreId { get; set; }
        [JsonProperty("upc")]
        public string Upc { get; set; }
        [JsonProperty("countAdjustment")]
        public int CountAdjustment { get; set; }
        [JsonProperty("productName")]
        public string ProductName { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("lastUpdateTimestamp")]
        public DateTime? LastUpdateTimestamp { get; set; }
        [JsonProperty("lastShipmentTimestamp")]
        public DateTime? LastShipmentTimestamp { get; set; }
    }
}