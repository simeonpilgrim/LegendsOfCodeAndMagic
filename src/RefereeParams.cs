using System;
using System.Text;

namespace LOCAM
{
    public class RefereeParams
    {
        public Random draftChoicesRNG;
        public Random shufflePlayer0RNG;
        public Random shufflePlayer1RNG;
        public int[,] predefinedDraftIds = null;
        private Properties _params;

        public RefereeParams(long draftChoicesSeed, long shufflePlayer0Seed, long shufflePlayer1Seed)
        {
            draftChoicesRNG = new Random((int)draftChoicesSeed);
            shufflePlayer0RNG = new Random((int)shufflePlayer0Seed);
            shufflePlayer1RNG = new Random((int)shufflePlayer1Seed);
        }

        public RefereeParams(MultiplayerGameManager<Player> gameManager)
        {
            // pure initialization if seed set by the manager
            long mainSeed = gameManager.getSeed();
            _params = gameManager.getGameParameters();

            Random RNG = new Random((int)mainSeed);

            if (long.TryParse(_params.getProperty("seed", ((int)DateTime.Now.Ticks).ToString()), out long val)) // overriding when seed given as parameter
            {
                mainSeed = val;
                RNG = new Random((int)mainSeed);
            }
            long draftChoicesSeed = RNG.Next();
            long shufflePlayer0Seed = RNG.Next();
            long shufflePlayer1Seed = RNG.Next();
            // overriding remaining seeds

            if (long.TryParse(_params.getProperty("draftChoicesSeed", ""), out val))
                draftChoicesSeed = val;
            if (long.TryParse(_params.getProperty("shufflePlayer0Seed", ""), out val))
                shufflePlayer0Seed = val;
            if (long.TryParse(_params.getProperty("shufflePlayer1Seed", ""), out val))
                shufflePlayer1Seed = val;

            if (_params.getProperty("predefinedDraftIds", "") != "")
            {
                predefinedDraftIds = new int[Constants.CARDS_IN_DECK, 3];
                string[] picks = _params.getProperty("predefinedDraftIds", "").Split(",");

                //assert(picks.Length >= Constants.CARDS_IN_DECK);

                for (int pick = 0; pick < Constants.CARDS_IN_DECK; pick++)
                {
                    string[] choice = picks[pick].Trim().Split("\\s+");
                    for (int i = 0; i < 3; i++)
                    {
                        predefinedDraftIds[pick, i] = int.Parse(choice[i].Trim());
                    }
                }
            }

            // update params values
            // we can't update predefinedDraftIds if there were not set by the user...
            _params.Add("seed", mainSeed.ToString());
            _params.Add("draftChoicesSeed", draftChoicesSeed.ToString());
            _params.Add("shufflePlayer0Seed", shufflePlayer0Seed.ToString());
            _params.Add("shufflePlayer1Seed", shufflePlayer1Seed.ToString());

            // set RNG's
            draftChoicesRNG = new Random((int)draftChoicesSeed);
            shufflePlayer0RNG = new Random((int)shufflePlayer0Seed);
            shufflePlayer1RNG = new Random((int)shufflePlayer1Seed);

            //Console.WriteLine(ToString());
        }

        override public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("draftChoicesSeed").Append("=").Append(_params.getProperty("draftChoicesSeed", "")).Append("\n");
            sb.Append("shufflePlayer0Seed").Append("=").Append(_params.getProperty("shufflePlayer0Seed", "")).Append("\n");
            sb.Append("shufflePlayer1Seed").Append("=").Append(_params.getProperty("shufflePlayer1Seed", "")).Append("\n");
            //sb.Append("predefinedDraftIds").Append("=").Append(params.getProperty("predefinedDraftIds","")).Append("\n");
            return sb.ToString();
        }
    }
}