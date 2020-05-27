using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace InventoryManagement
{
    public class InventoryDbActivity
    {
        private static Container container;

        [FunctionName("MDSWrite")]
        public static async Task<string> MdsWrite([ActivityTrigger] IDurableActivityContext context)
        {
            var data = context.GetInput<ItemInventory>();

            Container cosmosContainer = GetCosmosContainer();

            await cosmosContainer.UpsertItemAsync(data);
            return data.Id;
        }

        private static Container GetCosmosContainer() {
            if ( container == null ) {
                var cosmosEndpoint =  Environment.GetEnvironmentVariable("CosmosEndpoint", EnvironmentVariableTarget.Process);
                var cosmosKey =  Environment.GetEnvironmentVariable("CosmosKey", EnvironmentVariableTarget.Process);
                var database = Environment.GetEnvironmentVariable("Database", EnvironmentVariableTarget.Process);
                var containerId = Environment.GetEnvironmentVariable("Container", EnvironmentVariableTarget.Process);
                var cosmosDb = new CosmosClient(cosmosEndpoint, cosmosKey);
                container = cosmosDb.GetContainer(database, containerId);
            }
            return container;
        }
    }
}