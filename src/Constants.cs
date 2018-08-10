using System;
using System.Collections.Generic;
using System.IO;

namespace LOCAM
{
    /**
     * Created by aCat on 2018-03-22.
     */
    public class Constants
    {
        public static int VERBOSE_LEVEL = 3; // 3 - full, 2 - without turn details, 1 - results only, 0 - silent

        public const int CARDS_IN_DECK = 30; // 30;
        public const int CARDS_IN_DRAFT = 60; // 60;

        public const int INITIAL_HAND_SIZE = 4;
        public const int MAX_CARDS_IN_HAND = 8; // was 10
        public const int SECOND_PLAYER_CARD_BONUS = 1;
        public const int SECOND_PLAYER_MAX_CARD_BONUS = 0;
        //public const int EMPTY_DECK_DAMAGE = 5;

        public const int MAX_MANA = 12;
        public const int INITIAL_HEALTH = 30;

        public const int MAX_CREATURES_IN_LINE = 6; // was 8

        public const int TIMELIMIT_FIRSTDRAFTTURN = 1000;
        public const int TIMELIMIT_DRAFTTURN = 100;
        public const int TIMELIMIT_FIRSTGAMETURN = 1000;
        public const int TIMELIMIT_GAMETURN = 100;

        public const int PLAYER_TURNLIMIT = 50;
        public const int MAX_TURNS_HARDLIMIT = (2 * CARDS_IN_DECK + 2 * CARDS_IN_DECK + 2 * 10) * 10;

        public static readonly Dictionary<int, Card> CARDSET = new Dictionary<int, Card>();

        public const int FRAME_DURATION_DRAFT = 500;
        public const int FRAME_DURATION_BATTLE = 750;
        public const int FRAME_DURATION_SUMMON = 600;

        public static void LoadCardlist(string cardsetPath)
        {
            using (var sr = new StreamReader(cardsetPath))
            {
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine().Replace("//.*", "").Trim();
                    if (line.Length > 0)
                    {
                        Card c = new Card(line.Split(";"));
                        CARDSET[c.baseId] = c;
                    }
                }
            }
        }
    }
}