using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace InventoryManagement
{
    public static class InventoryOrchestration
    {
        [FunctionName("InventoryOrchestration")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            InventoryEvent data = context.GetInput<InventoryEvent>();

            var entityId = new EntityId("Store", data.StoreId);
            var proxy = context.CreateEntityProxy<IStore>(entityId);

            var itemInventory = await proxy.process(data);

            await context.CallActivityAsync<string>("MDSWrite", itemInventory);
        }
    }
}