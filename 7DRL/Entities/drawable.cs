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

        private transform lastPos;
        private char coveredTexture;

        public drawable()
        {
            components = new List<Components.iComponent>();
            pos = new transform();
            lastPos = pos;
            coveredTexture = texture;
        }

        public void update()
        {
            if(active)
            {
                for(int i = 0; i < components.Count; i++)
                {
                    Console.SetCursorPosition(0, 30);
                    Console.Write("Running Components...");
                    components[i].Run(this);
                    Console.SetCursorPosition(0, 30);
                    Console.Write("                       Finished");
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
            lastPos.xPos = pos.xPos;
            lastPos.xPos = pos.yPos;
            pos.xPos = x;
            pos.yPos = y;
        }

        public void setPosRelative(int x, int y)
        {
            lastPos.xPos = pos.xPos;
            lastPos.xPos = pos.yPos;
            pos.xPos += x;
            pos.yPos += y;
        }

        private void draw()
        {
            Console.SetCursorPosition(lastPos.xPos, lastPos.yPos);
            Console.Write(coveredTexture);

            coveredTexture = Game.g.world[pos.xPos, pos.yPos];

            Console.SetCursorPosition(pos.xPos, pos.yPos);
            Console.Write(texture);
        }
    }

    public class transform
    {
        public int xPos;
        public int yPos;
    }
}
