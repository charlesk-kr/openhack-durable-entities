using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InventoryManagement
{
    public static class OnHandClient
    {
        [FunctionName("OnHandCosmosTrigger")]
        public static async Task processOnHand(
            [CosmosDBTrigger(
                databaseName: "inventory",
                collectionName: "onHand",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "OnHandLeases",
                CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> documents,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string firstId = documents[0].Id;
            int count = documents.Count;

            log.LogInformation($"Processing '{count}' onHand events - start id: '{firstId}'.");

            foreach (Document document in documents) 
            {
                OnHandEvent onHandEvent = JsonConvert.DeserializeObject<OnHandEvent>(document.ToString());

                var inventoryEvent = new InventoryEvent();
                inventoryEvent.Id = onHandEvent.Id;
                inventoryEvent.DivisionId = onHandEvent.DivisionId;
                inventoryEvent.StoreId = onHandEvent.StoreId;
                inventoryEvent.CountAdjustment = onHandEvent.InventoryCount;
                inventoryEvent.Upc = onHandEvent.Upc;
                inventoryEvent.Description = onHandEvent.Description;
                inventoryEvent.ProductName = onHandEvent.ProductName;
                inventoryEvent.LastUpdateTimestamp = onHandEvent.LastUpdateTimestamp;

                await starter.StartNewAsync<InventoryEvent>("InventoryOrchestration", null, inventoryEvent).ConfigureAwait(false);
            }
        }
    }
}