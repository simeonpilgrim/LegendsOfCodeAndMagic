using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LOCAM
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }


    public class Properties
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        public string getProperty(string name, string defaultValue)
        {
            string val;
            if (values.TryGetValue(name, out val))
                return val;
            return defaultValue;
        }

        public void Add(string name, string value)
        {
            values[name] = value;
        }
    }


    public class MultiplayerGameManager<T> where T : AbstractPlayer
    {
        public MultiplayerGameManager()
        {
            //Const.InitTheThingsInConstThatAreNotConstant();
        }

        internal List<T> players = new List<T>();
        readonly int league = 1;
        internal int maxTurns = 0;
        internal bool game_ended = false;
        internal int seed = 0;
        internal Properties _params;

        // API's used by LOCAM
        public int getSeed() { return seed; }
        public Properties getGameParameters() { return _params; }

        public int getLeagueLevel() { return league; }
        public void setFrameDuration(int value) { }
        public void setMaxTurns(int value) { maxTurns = value; }
        //public List<T> getActivePlayers() { return players.Where(p => p.active).ToList(); }
        public List<T> getPlayers() { return players; }
        public T getPlayer(int index) { return players[index]; }

        public void setTurnMaxTime(int ms) { }


        public void endGame() { game_ended = true; }
        public void addToGameSummary(string value) { System.Diagnostics.Debug.WriteLine($"SUM: {value}"); }
    }

    public class AbstractMultiplayerPlayer : AbstractPlayer
    {
        virtual public int getExpectedOutputLines() { return 0; }
    }

    public class AbstractPlayer
    {
        int score = 0;
        internal bool active = true;
        // BotG API's
        public string getNicknameToken() { return "Player"; }
        public void setScore(int value) { score = value; }
        public void deactivate(string message) { active = false; }
        public void sendInputLine(string message)
        {
            // Send "input" to the player pipe
            pro.StandardInput.WriteLine(message);
            if (player_id == 0)
                Debug.WriteLine(message);

        }
        public void execute() { }

        public string[] getOutputs()
        {
            return getOutputs(1);
        }

        public string[] getOutputs(int lines_count)
        {
            // Read "output from player pipe
            var input = new List<string>();
            while (input.Count < lines_count)
            {
                string line = pro.StandardOutput.ReadLine();
                if (line != null && line.Length > 0)
                    input.Add(line);
            }
            return input.ToArray();
        }
        public int getIndex() { return player_id; }
        public int getScore() { return score; }

        // My API's
        internal Process pro;
        internal int player_id;
        internal string code_file_name;
    }
}
