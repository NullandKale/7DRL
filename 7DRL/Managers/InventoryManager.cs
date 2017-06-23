using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Managers
{
    public class InventoryManager
    {

    }

    public class Inventory
    {
        List<Item> items;

        public int currentGoldAmount;

        float currentWeight;
        float MaxWeight;

        public Inventory()
        {
            items = new List<Item>();
            currentGoldAmount = 0;

            updateWeight();
        }

        public void addItem(Item item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if(items[i].name == item.name)
                {
                    if(items[i].currentStackSize + item.currentStackSize <= items[i].maxStackSize)
                    {
                        items[i].currentStackSize += item.currentStackSize;
                        updateWeight();
                        return;
                    }
                    else
                    {
                        int itemsOver = items[i].currentStackSize + item.currentStackSize - items[i].maxStackSize;
                        items[i].currentStackSize = items[i].maxStackSize;
                        item.currentStackSize = itemsOver;
                        items.Add(item);
                        updateWeight();
                        return;
                    }
                }
            }

            items.Add(item);
            updateWeight();
        }

        public void removeItem(int loc, int amount)
        {
            if(items[loc].currentStackSize - amount < 0)
            {
                items[loc].currentStackSize -= amount;
            }
            else
            {
                items.RemoveAt(loc);
            }

            updateWeight();
        }

        public void CleanInv()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if(items[i].currentStackSize <= 0)
                {
                    removeItem(i, 10000);
                }
            }
        }

        private void updateWeight()
        {
            MaxWeight = Game.g.pcStats.carryWeight;

            currentWeight = 0;
            for(int i = 0; i < items.Count; i++)
            {
                currentWeight += items[i].weight * items[i].currentStackSize;
            }

            if(currentWeight > MaxWeight)
            {
                Game.g.pcStats.isEncumbered = true;
            }
            else
            {
                Game.g.pcStats.isEncumbered = false;
            }

        }
    }

    public class Item
    {
        public char texture;
        public bool isOnGround;
        public int xPos;
        public int yPos;

        public int inventoryPos;
        public string name;
        public string description;
        public float weight;
        public int value;

        public int maxStackSize;
        public int currentStackSize;

        public override string ToString()
        {
            return (texture + " " + name + " " + weight + " lbs. " + value + " Gold");
        }
    }
}
