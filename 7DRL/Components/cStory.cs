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

        List<string> genericRandomToughts;
        List<List<string>> randomThoughtSet;
        List<string> startThoughts;
        List<List<string>> endThoughtSet;

        public cStory(int endLevel)
        {
            
            randomThoughtSet = new List<List<string>>();
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

            genericRandomToughts = new List<string>
            {
                "You stub your toe.",
                "\"Just one more floor.\"",
                "\"When was the last time I ate?\"",
                "You hear the beating of giant wings.",
                "\"Have I been here before?\"",
                "\"These walls are familiar.\"",
                "\"How long have I been searching?\"",
                "You hear a faint call... " + Game.g.PName + "!",
                "\"Why am I doing this?\"",
                "\"What is this place?\"",
                "\"How did I get here?\""
            };
            List<string> randomset1 = new List<string>
            {
                "\"I must save her.\""
            };
            List<string> randomset2 = new List<string>
            {
                "\"What did she mean?\"",
                "\"Who was that?\"",
                "\"Where did she go?\""
            };
            List<string> randomset3 = new List<string>
            {
                "\"Where did she go?\"",
                "\"I must not forget.\"",
                "\"How do I save her?\"",
                "\"Why does she fade?\""
            };
            List<string> randomset4 = new List<string>
            {
                "\"Where did she go?\""
            };
            randomThoughtSet.Add(randomset1);
            randomThoughtSet.Add(randomset2);
            randomThoughtSet.Add(randomset3);
            randomThoughtSet.Add(randomset4);

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
            List<string> set4 = new List<string>
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
            endThoughtSet.Add(set4);
        }

        public void Run(Entities.drawable d)
        {
            if(Game.doTick)
            {
                if(Game.g.rng.Next(0,100) <= 5)
                {
                    List<string> set = new List<string>();
                    set.AddRange(randomThoughtSet[currentSet]);
                    set.AddRange(genericRandomToughts);

                    var thought = set[Game.g.rng.Next(set.Count)];
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
