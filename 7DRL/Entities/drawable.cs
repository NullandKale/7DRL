namespace _7DRL.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Drawable
    {
        public char texture;
        public ConsoleColor color;
        public Transform pos;
        public List<Components.iComponent> components;
        public bool active = true;
        public string tag;

        public Drawable()
        {
            color = ConsoleColor.Green;
            components = new List<Components.iComponent>();
            pos = new Transform();
        }

        public void Update()
        {
            if (active)
            {
                if (tag == "Stairs")
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        components[i].Run(this);
                    }
                }
                else if (Game.doTick && tag != "Player")
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        components[i].Run(this);
                    }
                }
                else if (tag == "Player")
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        components[i].Run(this);
                    }
                }

                Draw();
            }
        }

        public void AddComponent(Components.iComponent c)
        {
            if (!components.Contains(c))
            {
                components.Add(c);
            }
        }

        public T GetComponent<T>() where T : Components.iComponent
        {
            if (components.Any(c => c.GetType() == typeof(T)))
            {
                return (T)components[components.IndexOf(components.Find(c => c.GetType() == typeof(T)))];
            }

            return default(T);
        }

        public void SetPos(int x, int y)
        {
            Game.g.world[pos.xPos, pos.yPos].Collideable = false;
            pos.xPos = x;
            pos.yPos = y;
        }

        public void SetPosRelative(int x, int y)
        {
            Game.g.world[pos.xPos, pos.yPos].Collideable = false;
            pos.xPos += x;
            pos.yPos += y;
        }

        private void Draw()
        {
            if (pos.xPos != -1 && pos.yPos != -1)
            {
                Game.g.world[pos.xPos, pos.yPos].Visual = texture;
                Game.g.world[pos.xPos, pos.yPos].Color = color;
                Game.g.world[pos.xPos, pos.yPos].Collideable = true;
            }
        }
    }
}
