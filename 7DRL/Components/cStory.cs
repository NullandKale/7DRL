using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Components
{
    class cStory : iComponent
    {
        private string lastThought;

        private int currentSet;

        List<string> randomThoughts;
        List<string> startThoughts;
        List<List<string>> endThoughtSet;

        public cStory(int endLevel)
        {
            randomThoughts = new List<string>();
            startThoughts = new List<string>();
            endThoughtSet = new List<List<string>>();

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

            List<string> set1 = new List<string>
            {
                "\"I found you!\"",
                " P - \"You did.\"",
                " P - \"... and you will again.\"",
                "\"What do you mean?\"",
                "As you walk towards the princess,",
                "she fades away"
            };
            List<string> set2 = new List<string>
            {
                "\"I found you again!\"",
                " P - \"You did.\"",
                " P - \"... and you'll forget again.\"",
                "\"What do you mean?\"",
                "As you walk towards the princess,",
                "she fades away"
            };
            List<string> set3 = new List<string>
            {
                "\"I found you,\"" +
                "\" and I didn't forget!\"",
                " P - \"You did.\"",
                " P - \"... and you will again.\"",
                "\"What do you mean?\"",
                "As you walk towards the princess,",
                "she fades away"
            };

            endThoughtSet.Add(set1);
            endThoughtSet.Add(set2);
            endThoughtSet.Add(set3);
        }

        public void Run(Entities.drawable d)
        {
            if(Game.doTick)
            {
                if(Game.g.rng.Next(0,100) <= 5)
                {
                    var thought = randomThoughts[Game.g.rng.Next(randomThoughts.Count)];
                    if (lastThought != thought)
                    {
                        Game.g.LogCombat(thought);
                        lastThought = thought;
                    }
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
                                for(int k = 0; k < endThoughtSet[currentSet].Count; k++)
                                {
                                    Game.g.LogCombat(endThoughtSet[currentSet][k]);
                                }
                                Game.g.princess.active = false;
                                if (currentSet < endThoughtSet.Count)
                                {
                                    currentSet++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
