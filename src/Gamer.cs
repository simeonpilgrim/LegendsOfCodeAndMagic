using System.Collections.Generic;
using System.Text;

namespace LOCAM
{

    /**
     * Created by aCat on 2018-03-24.
     */
    public class Gamer
    {
        int id;
        //public bool isSecond;
        public List<Card> hand;
        public List<Card> deck;
        public List<CreatureOnBoard> board;
        public List<CreatureOnBoard> graveyard;
        public int health;
        public int maxMana;
        public int currentMana;
        public int nextTurnDraw;

        public List<int> runes = new List<int>() { 5, 10, 15, 20, 25 };
        public List<Action> performedActions;
        public int handLimit;

        // todo rest



        public Gamer(int id, List<Card> deck)
        {
            this.id = id;
            this.hand = new List<Card>();
            this.deck = new List<Card>(deck);
            this.board = new List<CreatureOnBoard>();
            this.graveyard = new List<CreatureOnBoard>();
            this.performedActions = new List<Action>();
            this.health = Constants.INITIAL_HEALTH;
            this.maxMana = 0;
            this.currentMana = 0;
            this.nextTurnDraw = 1;

            handLimit = Constants.MAX_CARDS_IN_HAND + (id == 0 ? 0 : Constants.SECOND_PLAYER_MAX_CARD_BONUS);
            DrawCards(Constants.INITIAL_HAND_SIZE + (id == 0 ? 0 : Constants.SECOND_PLAYER_CARD_BONUS), 0);
        }

        private void suicideRunes()
        {
            if (runes.Count > 0) // first rune gone
            {
                int l = runes.Count - 1;
                int r = runes[l];
                runes.RemoveAt(l);
                health = r;
            }
            else // final run gone - suicide
            {
                health = 0;
            }
        }

        public void DrawCards(int n, int playerturn)
        {
            for (int i = 0; i < n; i++)
            {
                if (deck.Count == 0 || playerturn >= Constants.PLAYER_TURNLIMIT)
                {
                    suicideRunes();
                    continue;
                }

                if (hand.Count == handLimit)
                {
                    break; // additional draws are simply wasted
                }

                Card c = deck[0];
                deck.RemoveAt(0);
                hand.Add(c);
            }
        }


        public void ModifyHealth(int mod)
        {
            health += mod;

            if (mod >= 0)
                return;

            for (int r = runes.Count - 1; r >= 0; r--) // rune checking;
            {
                if (health <= runes[r])
                {
                    nextTurnDraw += 1;
                    runes.Remove(r);
                }
            }
        }

        public int nextRune()
        {
            if (runes.Count == 0)
                return 0;
            return runes[runes.Count - 1];
        }

        public void removeFromBoard(int creatureIndex)
        {
            graveyard.Add(board[creatureIndex]);
            board.RemoveAt(creatureIndex);
        }

        public string toDescriptiveString(bool reverse)
        {
            string line1 = string.Format("[Player %d] Health: %d %s     Mana: %d/%d", id, health, runes, currentMana, maxMana);
            string line2 = string.Format("Cards in hand: %d   In deck: %d   Next turn draw: %d", hand.Count, deck.Count, nextTurnDraw);

            List<string> inhand = new List<string>();
            inhand.Add("Hand:");
            foreach (Card c in hand)
                inhand.Add((c.cost <= this.currentMana ? " * " : "   ") + c.toDescriptiveString());

            List<string> onboard = new List<string>();
            onboard.Add("Board:");
            foreach (CreatureOnBoard c in board)
                onboard.Add((c.canAttack ? " * " : "   ") + c.toDescriptiveString());

            List<string> description = new List<string>();
            description.Add(line1);
            description.Add(line2);
            description.Add(string.Join("\n", inhand));
            description.Add(string.Join("\n", onboard));
            if (reverse)
                description.Reverse();

            return string.Join("\n", description);
        }

        override public string ToString()
        {
            return base.ToString();
        }

        public string getPlayerInput()
        {
            StringBuilder s = new StringBuilder();
            s.Append(health).Append(" ");
            s.Append(maxMana).Append(" ");
            s.Append(deck.Count).Append(" ");
            s.Append(nextRune());
            return s.ToString();
        }
    }
}