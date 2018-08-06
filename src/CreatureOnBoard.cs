using System.Collections.Generic;
using System.Text;

namespace LOCAM
{

    /**
     * Creature that is not a card anymore but it is placed on board.
     */
    public class CreatureOnBoard
    {
        public int id;
        public int baseId;
        public int attack;
        public int defense;
        public int cost;
        public int myHealthChange;
        public int oppHealthChange;
        public int cardDraw;
        public Keywords keywords;

        public bool canAttack;
        public bool hasAttacked;
        public int lastTurnDefense;

        public Card baseCard;

        public CreatureOnBoard(CreatureOnBoard creature)
        {
            this.id = creature.id;
            this.baseId = creature.baseId;
            this.cost = creature.cost;
            this.attack = creature.attack;
            this.defense = creature.defense;
            this.keywords = new Keywords(creature.keywords);
            this.lastTurnDefense = creature.lastTurnDefense;
            baseCard = creature.baseCard;
            this.canAttack = creature.canAttack;
            this.hasAttacked = creature.hasAttacked;
        }

        /**
         * @param data "id baseId attack defense keywords"
         */
        public CreatureOnBoard(string data)
        {
            string[] creature = data.Split(" ");
            this.id = int.Parse(creature[0]);
            this.baseId = int.Parse(creature[1]);
            this.attack = int.Parse(creature[2]);
            this.defense = int.Parse(creature[3]);
            this.keywords = new Keywords(creature[4]);
            this.canAttack = this.keywords.hasCharge;
            this.lastTurnDefense = this.defense;
        }

        public CreatureOnBoard(Card card)
        {
            this.id = card.id;
            this.baseId = card.baseId;
            this.attack = card.attack;
            this.defense = card.defense;
            this.keywords = new Keywords(card.keywords);
            this.canAttack = this.keywords.hasCharge;
            this.lastTurnDefense = card.defense;
            this.cost = card.cost;
            this.myHealthChange = card.myHealthChange;
            this.oppHealthChange = card.oppHealthChange;
            this.cardDraw = card.cardDraw;
            baseCard = card;
        }

        public string generateText()
        {
            List<string> keywords = this.keywords.getListOfKeywords();

            return string.Join(", ", keywords);
        }

        public string toDescriptiveString()
        {
            StringBuilder sb = new StringBuilder();
            if (id >= 0) sb.Append("id:").Append(this.id).Append(' ');
            sb.Append("(").Append(this.baseId).Append(")").Append(' ');

            sb.Append("ATT:").Append(this.attack).Append(' ');
            sb.Append("DEF:").Append(this.defense).Append(' ');
            sb.Append(generateText());

            return sb.ToString();
        }

        override public string ToString()
        {
            return $"{id} {baseId} {attack} {defense} {keywords}";
        }

        public string getAsInput(bool isOpponentBoard)
        {
            int position = isOpponentBoard ? -1 : 1;
            return $"{baseId} {id} {position} {Card.Type.CREATURE} {cost} {attack} {defense} {keywords} {myHealthChange} {oppHealthChange} {cardDraw} ";
        }

        public string toTooltipText()
        {
            return baseCard.toTooltipText(this);
        }
    }
}