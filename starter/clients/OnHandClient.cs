using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace InventoryManagement
{
    public static class OnHandClient
    {
        [FunctionName("OnHandCosmosTrigger")]
        public static void processOnHand(
            [CosmosDBTrigger(
                databaseName: "inventory",
                collectionName: "onHand",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "OnHandLeases",
                CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> onHandEvents,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string firstId = onHandEvents[0].Id;
            int count = onHandEvents.Count;

            log.LogInformation($"Processing '{count}' onHand events - start id: '{firstId}'.");
        }
    }
}