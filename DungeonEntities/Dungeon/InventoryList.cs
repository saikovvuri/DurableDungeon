using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.WebJobs;

namespace DungeonEntities.Dungeon
{
    public class InventoryList : BaseHasInventory, IInventoryListOperations
    {
        public void New(Inventory[] inventory)
        {
            InventoryList = (from i in inventory select i.Name).ToList();
            SaveLists();
        }

        [FunctionName(nameof(InventoryList))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<InventoryList>();
    }
}
