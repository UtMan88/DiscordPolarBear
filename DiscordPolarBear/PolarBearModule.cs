using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordPolarBear
{
    public class PolarBearModule : ModuleBase
    {
        private Random rand = new Random();
        private readonly string[] riddles = new string[]
        {
            "Polar Bears sit around Holes In The Ice, like Petals On A Flower.",
            "Polar Ice sit around Holes In The Flower, like Petals On A Bears.",
            "**Did You Know?** That Polar Bears sit around Holes In The Ice, like Petals On A Flower.",
            "Ya know, these Polar Bears, man, they, just, like, sit around Holes In The Ice, ya know? Like, Petals On A Flower or something...",
            "Polar Bears. Holes. Ice. Petals. Flower.",
            "I'm tired of these monkey-fighting Polar Bears, 'round this muck-flinging Ice, like Petals on a Monday-to-Friday Flower!"
        };

        static Dictionary<IMessageChannel, int[]> rolls =  new Dictionary<IMessageChannel, int[]>();

        [Command("hello")]
        public async Task HelloCommand()
        {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            if(rand.Next() % 2 == 0 && rand.Next() % 5 == 0)
            {
                sb.AppendLine("rawr.");
            }
            else if(rand.Next() % 5 == 0)
            {
                sb.AppendLine("blub.");
            }
            else
            {
                sb.AppendLine("*[Plankton sounds intensify]*");
            }

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        [Command("roll")]
        public async Task RollCommand()
        {
            var sb = new StringBuilder();

            var channel = Context.Channel;

            int riddle = rand.Next(0, riddles.Length);
            sb.AppendLine(riddles[riddle]);

            int[] dice = new int[5];
            for(int i = 0; i < 5; ++i)
            {
                dice[i] = rand.Next(1, 7);
            }

            rolls[channel] = dice;
            GenerateRollString(dice, sb);

            sb.AppendLine("How many **Polar Bears**, how many **Fish**, and how many **Plankton**?");
            sb.AppendLine();
            sb.AppendLine("Find out by replying `howmany [bears|fish|plankton]`, or take a guess with `thereare x [bears|fish|plankton]`!");

            await ReplyAsync(sb.ToString());
        }

        [Command("howmany")]
        public async Task HowManyCommand(string howmany)
        {
            var sb = new StringBuilder();
            if (rolls.ContainsKey(Context.Channel))
            {
                sb.AppendLine("lets see...");
                GenerateRollString(rolls[Context.Channel], sb);
                switch (howmany.ToLower())
                {
                    case "bear":
                    case "bears":
                        int bears = CalculateBears(rolls[Context.Channel]);
                        sb.AppendLine($"There are {bears} {(bears == 1 ? "bear" : "bears")}!");
                        break;
                    case "fish":
                        int fish = CalculateFish(rolls[Context.Channel]);
                        sb.AppendLine($"There are {fish} fish!");
                        break;
                    case "plankton":
                        int plankton = CalculatePlankton(rolls[Context.Channel]);
                        sb.AppendLine($"There are {plankton} plankton!");
                        break;
                    default:
                        sb.AppendLine($"Uhh... There are {rand.Next(0, 1000)} {howmany}, I guess...");
                        sb.AppendLine("... What does that have to do with **bears**, **fish**, or **plankton**?");
                        break;
                }
            }
            else
            {
                sb.AppendLine("You're standing in a field of flowers. Try `~roll`ing first!");
            }
            await ReplyAsync(sb.ToString());
        }

        [Command("thereare")]
        public async Task ThereAreCommand(int num, string howmany)
        {
            var sb = new StringBuilder();
            if (rolls.ContainsKey(Context.Channel))
            {
                switch (howmany.ToLower())
                {
                    case "bear":
                    case "bears":
                        int bears = CalculateBears(rolls[Context.Channel]);
                        if (num == bears)
                            sb.AppendLine("**Correct!**");
                        else
                            sb.AppendLine("**Nope!**");
                        break;
                    case "fish":
                        int fish = CalculateFish(rolls[Context.Channel]);
                        if (num == fish)
                            sb.AppendLine("**Correct!**");
                        else
                            sb.AppendLine("**Nope!**");
                        break;
                    case "plankton":
                        int plankton = CalculatePlankton(rolls[Context.Channel]);
                        if (num == plankton)
                            sb.AppendLine("**Correct!**");
                        else
                            sb.AppendLine("**Nope!**");
                        break;
                    default:
                        sb.AppendLine("Uhhh... what does that have to do with **bears**, **fish**, or **plankton**?");
                        break;
                }
            }
            else
            {
                sb.AppendLine("Wrong, because you're standing in a field of flowers. Try `~roll`ing first!");
            }
            await ReplyAsync(sb.ToString());
        }

        private string GenerateRollString(int[] dice, StringBuilder sb)
        {
            if(sb == null)
            {
                sb = new StringBuilder();
            }

            for (int i = 0; i < 5; ++i)
            {
                switch (dice[i])
                {
                    case 1:
                        sb.Append("⚀");
                        break;
                    case 2:
                        sb.Append("⚁");
                        break;
                    case 3:
                        sb.Append("⚂");
                        break;
                    case 4:
                        sb.Append("⚃");
                        break;
                    case 5:
                        sb.Append("⚄");
                        break;
                    case 6:
                        sb.Append("⚅");
                        break;
                    default:
                        sb.Append("☒");
                        break;
                }
                sb.Append(" ");
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private int CalculateBears(int[] dice)
        {
            int ret = 0;
            foreach (int d in dice)
            {
                switch(d)
                {
                    default:
                    case 1:
                    case 2:
                    case 4:
                    case 6:
                        break;
                    case 3:
                        ret += 2;
                        break;
                    case 5:
                        ret += 4;
                        break;
                }
            }

            return ret;
        }

        private int CalculateFish(int[] dice)
        {
            int ret = 0;
            foreach (int d in dice)
            {
                switch (d)
                {
                    default:
                    case 2:
                    case 4:
                    case 6:
                        break;
                    case 1:
                        ret += 6;
                        break;
                    case 3:
                        ret += 4;
                        break;
                    case 5:
                        ret += 2;
                        break;
                }
            }

            return ret;
        }

        private int CalculatePlankton(int[] dice)
        {
            int sumEven = 0;
            int sumOdd = 0;
            foreach(int d in dice)
            {
                if (d % 2 == 0)
                {
                    switch (d)
                    {
                        default:
                            //wut.
                            continue;
                        case 2:
                            sumOdd += 5;
                            break;
                        case 4:
                            sumOdd += 3;
                            break;
                        case 6:
                            sumOdd += 1;
                            break;
                    }
                    sumEven += d;
                }
            }
            return (sumEven * 7) + sumOdd;
        }
    }
}
