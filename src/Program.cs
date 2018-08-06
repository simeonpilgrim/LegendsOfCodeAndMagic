using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LegendsOfCodeAndMagic
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

    public class GameManager<T> where T : AbstractPlayer
    {
        public GameManager()
        {
            //Const.InitTheThingsInConstThatAreNotConstant();
        }

        internal List<T> players = new List<T>();
        int league = 7;
        internal int maxTurns = 0;
        internal bool game_ended = false;

        // API's used by BotG
        public int getLeagueLevel() { return league; }
        public void setFrameDuration(int value) { }
        public void setMaxTurns(int value) { maxTurns = value; }
        public List<T> getActivePlayers() { return players.Where(p => p.active).ToList(); }
        public List<T> getPlayers() { return players; }
        public void endGame() { game_ended = true; }
        public void addToGameSummary(string value) { System.Diagnostics.Debug.WriteLine($"SUM: {value}"); }

        // API's used to run game
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
