using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace InventoryManagement
{
    using System.Threading.Tasks;

    [JsonObject(MemberSerialization.OptIn)]
    public class Store
    {
        [JsonProperty("divisionId")]
        string DivisionId { get; set; }
        [JsonProperty("storeId")]
        string StoreId { get; set; }
        [JsonProperty("items")]
        Dictionary<string, StoreItem> Items { get; set; } = new Dictionary<string, StoreItem>();


        [FunctionName(nameof(Store))]
        public static Task store([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Store>();

        Task<ItemInventory> process(InventoryEvent inventoryEvent)
        {
            if ( DivisionId == null ) 
            {
                DivisionId = inventoryEvent.DivisionId;
                StoreId = inventoryEvent.StoreId;
            }

            if ( !Items.TryGetValue(inventoryEvent.Upc, out var currentItem) )
            {
                this.Items[inventoryEvent.Upc] = new StoreItem(){
                    Upc = inventoryEvent.Upc,
                    InventoryCount = inventoryEvent.CountAdjustment,
                    ProductName = inventoryEvent.ProductName,
                    Description = inventoryEvent.Description,
                    LastUpdateTimestamp = inventoryEvent.LastShipmentTimestamp,
                    LastShipmentTimestamp = inventoryEvent.LastShipmentTimestamp
                };
            }
            else
            {
                if ( inventoryEvent.LastShipmentTimestamp == null )
                {
                    currentItem.InventoryCount = inventoryEvent.CountAdjustment;
                }
                else 
                {
                    currentItem.InventoryCount += inventoryEvent.CountAdjustment;
                }
            }

            currentItem = Items[inventoryEvent.Upc];
            ItemInventory itemInventory = new ItemInventory();
            itemInventory.Id = BuildItemId(DivisionId, StoreId, currentItem.Upc);
            itemInventory.DivisionId = DivisionId;
            itemInventory.StoreId = StoreId;
            itemInventory.Upc = currentItem.Upc;
            itemInventory.ProductName = itemInventory.ProductName;
            itemInventory.Description = currentItem.Description;
            itemInventory.LastShipmentTimestamp = itemInventory.LastShipmentTimestamp;
            itemInventory.LastUpdateTimestamp = itemInventory.LastUpdateTimestamp;

            Console.WriteLine($"[{DivisionId}/{StoreId}] Inventory event '{inventoryEvent.Id}");
            return Task.FromResult(itemInventory);
        }

        private string BuildItemId(string division, string store, string upc) 
        {
            return division + ":" + store + ":" + upc;
        }
    }
}