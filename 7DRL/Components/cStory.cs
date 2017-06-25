using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Components
{
    class cStory : iComponent
    {
        List<string> randomThoughts;
        List<string> startThoughts;
        List<string> endThoughts;

        public cStory(int endLevel)
        {
            randomThoughts = new List<string>();
            startThoughts = new List<string>();
            endThoughts = new List<string>();

            startThoughts.Add("You awaken in a dungeon.");
            startThoughts.Add("You feel a beckoning down,");
            startThoughts.Add("deaper into the dungeon.");
            startThoughts.Add("You hear a faint call... " + Game.g.PName + "!");
            startThoughts.Add("You must go farther down.");

            for(int i = 0; i < startThoughts.Count; i++)
            {
                Game.g.LogCombat(startThoughts[i]);
            }

            randomThoughts.Add("You stub your toe.");
            randomThoughts.Add("You hear the beating of giant wings.");
            randomThoughts.Add("\"Have I been here before?\"");
            randomThoughts.Add("\"I must save her.\"");
            randomThoughts.Add("\"How long have I been searching?\"");
            randomThoughts.Add("\"Just one more floor.\"");
            randomThoughts.Add("\"When was the last time I ate?\"");
            randomThoughts.Add("\"Why am I doing this?\"");

            endThoughts.Add("\"I found you!\"");
            endThoughts.Add(" P - \"You did.\"");
            endThoughts.Add(" P - \"... and you will again.\"");
            endThoughts.Add("\"What do you mean?\"");
            endThoughts.Add("As you walk towards the princess,");
            endThoughts.Add("she fades away");
        }

        public void Run(Entities.drawable d)
        {
            if(Game.doTick)
            {
                if(Game.g.rng.Next(0,100) <= 5)
                {
                    Game.g.LogCombat(randomThoughts[Game.g.rng.Next(randomThoughts.Count)]);
                }

                if(Game.g.floor % 10 == 0 && Game.g.floor != 0)
                {
                    for(int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int xPos = d.pos.xPos + i;
                            int yPos = d.pos.yPos + j;

                            if(Game.g.world[xPos, yPos].Visual == 'P')
                            {
                                for(int k = 0; k < endThoughts.Count; k++)
                                {
                                    Game.g.LogCombat(endThoughts[k]);
                                }
                                Game.g.princess.active = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
