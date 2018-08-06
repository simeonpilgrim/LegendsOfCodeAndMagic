using System;
using System.Collections.Generic;
using System.Linq;

namespace LOCAM
{

    /**
     * Created by aCat on 2018-03-24.
     */
    public class DraftPhase
    {
        public enum Difficulty { NORMAL, LESS_EASY, EASY, VERY_EASY };

        public Difficulty difficulty;
        public List<Card> allowedCards;
        //TODO List should be used everywhere, apart from creation
        public List<Card> draftingCards;
        public Card[,] draft;
        //TODO we shouldn't mix arrays and collections, List<List<Card>> would be better
        public List<Card>[] chosenCards;
        public List<Card>[] decks; // after shuffle and assigning unique id's

        public string[] text = new string[2];

        private Random choicesRNG;
        private Random[] shufflesRNG;
        private RefereeParams _params;

        // todo - add function and field documentation

        public DraftPhase(Difficulty difficulty, RefereeParams _params)
        {
            this.difficulty = difficulty;
            this._params = _params;

            chosenCards = new List<Card>[] { new List<Card>(), new List<Card>() };
            decks = new List<Card>[] { new List<Card>(), new List<Card>() };

            choicesRNG = _params.draftChoicesRNG;
            shufflesRNG = new Random[] { _params.shufflePlayer0RNG, _params.shufflePlayer1RNG };
        }

        private bool isVeryEasyCard(Card card)
        {
            return card.type == Card.Type.CREATURE
                    && !card.keywords.hasAnyKeyword()
                    && card.myHealthChange == 0 && card.oppHealthChange == 0 && card.cardDraw == 0;
        }

        private void prepareAllowedCards()
        {
            var cardBase = Constants.CARDSET.Values;

            if (difficulty == Difficulty.NORMAL)
            {
                allowedCards = new List<Card>(cardBase);
            }
            else if (difficulty == Difficulty.LESS_EASY)
            {
                allowedCards = cardBase
                    .Where(card => !card.keywords.hasDrain && !card.keywords.hasLethal && !card.keywords.hasWard)
                    .ToList();
            }
            else if (difficulty == Difficulty.EASY)
            {
                allowedCards = cardBase
                    .Where(card => card.type == Card.Type.CREATURE)
                    .Where(card => !card.keywords.hasDrain && !card.keywords.hasLethal && !card.keywords.hasWard)
                    .ToList();
            }
            else
            {
                allowedCards = cardBase
                    .Where(card => isVeryEasyCard(card))
                    .ToList();
            }
        }

        public void PrepareChoices()
        {
            prepareAllowedCards();

            if (_params.predefinedDraftIds != null) // parameter-forced draft choices
            {
                draftingCards = new List<Card>(); // 0 size is ok here? in hand-made draft this is meaningless variable
                draft = new Card[Constants.CARDS_IN_DECK,3];
                for (int pick = 0; pick < Constants.CARDS_IN_DECK; pick++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        draft[pick, i] = Constants.CARDSET[_params.predefinedDraftIds[pick, i]];
                    }
                }

                return;
            }

            List<int> drafting = new List<int>();
            for (int pick = 0; pick < Math.Min(Constants.CARDS_IN_DRAFT, allowedCards.Count); pick++)
            {
                int i = -1;
                do
                {
                    i = choicesRNG.Next(allowedCards.Count);
                } while (drafting.Contains(i));
                drafting.Add(i);
            }

            //assert (drafting.Count>=3);

            draftingCards = new List<Card>();
            foreach (int i in drafting)
                draftingCards.Add(allowedCards[i]);


            draft = new Card[Constants.CARDS_IN_DECK,3];
            for (int pick = 0; pick < Constants.CARDS_IN_DECK; pick++)
            {
                int choice1 = drafting[choicesRNG.Next(drafting.Count)];
                int choice2;
                do
                {
                    choice2 = drafting[choicesRNG.Next(drafting.Count)];
                } while (choice2 == choice1);
                int choice3;
                do
                {
                    choice3 = drafting[choicesRNG.Next(drafting.Count)];
                } while (choice3 == choice1 || choice3 == choice2);

                draft[pick,0] = allowedCards[choice1];
                draft[pick,1] = allowedCards[choice2];
                draft[pick,2] = allowedCards[choice3];
            }

            //return draftCards;
        }

        public ChoiceResultPair PlayerChoice(int pickNumber, string action, int player)
        {
            Card choice = null;
            string text = "";
            try
            {
                string[] command = action.Split(" ", 3);
                text = command.Length < 3 ? "" : command[2].Trim();

                if (command[0] != "PICK" && command[0] != "CHOOSE" && command[0] != "PASS")
                    throw new Exception();
                if (command[0] == "PASS") {
                    choice = draft[pickNumber,0];
                }
                else if (command[0] == "PICK")
                {
                    int value = int.Parse(command[1]);
                    if (value < 0 || value > 2)
                        throw new InvalidActionHard("Invalid action format. \"PICK\" argument should be 0, 1 or 2.", null);
                    choice = draft[pickNumber,value];
                }
                else // "CHOOSE"
                {
                    HashSet<int> ids = new HashSet<int>();
                    ids.Add(draft[pickNumber,0].baseId);
                    ids.Add(draft[pickNumber,1].baseId);
                    ids.Add(draft[pickNumber,2].baseId);
                    int value = int.Parse(command[1]);

                    if (!ids.Contains(value))
                        throw new InvalidActionHard("Invalid action format. \"CHOOSE\" argument should be valid card's base id " + ids + ".", null);
                    choice = Constants.CARDSET[value];
                }
            }
            catch (InvalidActionHard e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new InvalidActionHard("Invalid action. Expected  \"PICK [0,1,2]\" or \"PASS\".", e);
            }

            chosenCards[player].Add(choice);
            return new ChoiceResultPair(choice, text);
        }

        public void ShuffleDecks()
        {
            for (int player = 0; player < 2; player++)
            {
                foreach (Card c in chosenCards[player])
                {
                    decks[player].Add(new Card(c));
                }
                decks[player].Shuffle(shufflesRNG[player]);
                for (int i = 0; i < decks[player].Count; i++)
                {
                    decks[player][i].id = 2 * i + player + 1;
                }
            }
        }



        public class ChoiceResultPair
        {
            public Card card;
            public string text;

            public ChoiceResultPair(Card card, string text)
            {
                this.card = card;
                this.text = text;
            }
        }

        public string[] getMockPlayersInput() {
            List<string> lines = new List<string>();
            string s = $"{Constants.INITIAL_HEALTH} 0 {decks[0].Count} 25";
            lines.Add(s);
            lines.Add(s);
            lines.Add("0");
            lines.Add("3");

            return lines.ToArray();
        }
    }

    static class MyExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}