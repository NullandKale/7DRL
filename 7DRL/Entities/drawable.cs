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
                for(int i = 0; i < components.Count; i++)
                {
                    components[i].Run(this);
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
            pos.xPos = x;
            pos.yPos = y;
        }

        public void setPosRelative(int x, int y)
        {
            pos.xPos += x;
            pos.yPos += y;
        }

        private void draw()
        {
            Console.SetCursorPosition(pos.xPos - 1, pos.yPos);
            Console.Write(texture);
        }
    }

    public class transform
    {
        public int xPos;
        public int yPos;
    }
}
