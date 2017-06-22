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
        public int weight;
        public int value;

        public override string ToString()
        {
            return (texture + " " + name + " " + weight + " lbs. " + value + " Gold");
        }
    }
}
