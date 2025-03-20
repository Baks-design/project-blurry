using System;
using System.Collections.Generic;

namespace Assets.Scripts.Runtime.Systems.Inventory
{
    public class Builder
    {
        int capacity = 20;
        readonly InventoryView view;
        IEnumerable<ItemDetails> itemDetails;

        public Builder(InventoryView view) => this.view = view;

        public Builder WithStartingItems(IEnumerable<ItemDetails> itemDetails)
        {
            this.itemDetails = itemDetails;
            return this;
        }

        public Builder WithCapacity(int capacity)
        {
            this.capacity = capacity;
            return this;
        }

        public InventoryController Build()
        {
            var model = itemDetails != null
                ? new InventoryModel(itemDetails, capacity)
                : new InventoryModel(Array.Empty<ItemDetails>(), capacity);

            return new InventoryController(view, model, capacity);
        }
    }
}