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

        }

        public void Run(Entities.drawable r)
        {
            if(Game.doTick)
            {
                if(Game.g.rng.Next(0,100) <= 5)
                {
                    Game.g.LogCombat(randomThoughts[Game.g.rng.Next(randomThoughts.Count)]);
                }
            }
        }
    }
}
