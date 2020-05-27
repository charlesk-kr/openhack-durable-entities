using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InventoryManagement
{
    public static class ShipmentClient
    {
        [FunctionName("ShipmentCosmosTrigger")]
        public static async Task processShipments(
            [CosmosDBTrigger(
                databaseName: "inventory",
                collectionName: "shipments",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "ShipmentLeases",
                CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> documents,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string firstId = documents[0].Id;
            int count = documents.Count;

            log.LogInformation($"Processing '{count}' shipment events - start id: '{firstId}'.");

            foreach (Document document in documents)
            {
                ShipmentEvent shipmentEvent = JsonConvert.DeserializeObject<ShipmentEvent>(document.ToString());

                foreach (ShipmentItem item in shipmentEvent.Items)
                {
                    var inventoryEvent = new InventoryEvent();
                    inventoryEvent.Id = shipmentEvent.Id;
                    inventoryEvent.DivisionId = shipmentEvent.DivisionId;
                    inventoryEvent.StoreId = shipmentEvent.StoreId;
                    inventoryEvent.CountAdjustment = item.ShipmentAmount;
                    inventoryEvent.Upc = item.Upc;
                    inventoryEvent.LastShipmentTimestamp = shipmentEvent.LastShipmentTimestamp;

                    await starter.StartNewAsync<InventoryEvent>("InventoryOrchestration", null, inventoryEvent).ConfigureAwait(false);
                }
            }
        }
    }
}