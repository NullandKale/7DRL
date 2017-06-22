using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Entities
{
    public class drawable
    {
        public char texture;
        public transform pos;
        public List<Components.iComponent> components;
        public bool active = true;
        public string tag;

        public drawable()
        {
            components = new List<Components.iComponent>();
            pos = new transform();
        }

        public void update()
        {
            if(active)
            {
                if(Game.doTick && tag != "Player")
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        components[i].Run(this);
                    }
                }
                else if(tag == "Player")
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        components[i].Run(this);
                    }
                }

                draw();
            }
        }

        public void AddComponent(Components.iComponent c)
        {
            if(!components.Contains(c))
            {
                components.Add(c);
            }
        }

        public void setPos(int x, int y)
        {
            Game.g.world[pos.xPos, pos.yPos].collideable = false;
            pos.xPos = x;
            pos.yPos = y;
        }

        public void setPosRelative(int x, int y)
        {
            Game.g.world[pos.xPos, pos.yPos].collideable = false;
            pos.xPos += x;
            pos.yPos += y;
        }

        private void draw()
        {
            if (pos.xPos != -1 && pos.yPos != -1)
            {
                Game.g.world[pos.xPos, pos.yPos].Visual = texture;
                Game.g.world[pos.xPos, pos.yPos].collideable = true;
            }
        }
    }

    public class transform
    {
        public int xPos;
        public int yPos;
    }
}
