namespace InventoryManagement
{
    using System.Threading.Tasks;

    public interface IStore
    {
        Task<ItemInventory> process(InventoryEvent inventoryEvent);
    }
}