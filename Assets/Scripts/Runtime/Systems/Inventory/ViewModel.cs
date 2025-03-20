using Assets.Scripts.Runtime.Systems.Inventory.Helpers;

namespace Assets.Scripts.Runtime.Systems.Inventory
{
    public class ViewModel
    {
        public readonly int Capacity;
        public readonly BindableProperty<string> Coins;

        public ViewModel(InventoryModel model, int capacity)
        {
            Capacity = capacity;
            Coins = BindableProperty<string>.Bind(() => model.Coins.ToString());
        }
    }
}