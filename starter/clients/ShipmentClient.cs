using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace InventoryManagement
{
    public static class ShipmentClient
    {
        [FunctionName("ShipmentCosmosTrigger")]
        public static void processShipments(
            [CosmosDBTrigger(
                databaseName: "inventory",
                collectionName: "shipments",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "ShipmentLeases",
                CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> shipmentEvents,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string firstId = shipmentEvents[0].Id;
            int count = shipmentEvents.Count;

            log.LogInformation($"Processing '{count}' shipment events - start id: '{firstId}'.");
        }
    }
}