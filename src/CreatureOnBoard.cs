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
            string[] creature = data.split(" ");
            this.id = int.parse(creature[0]);
            this.baseId = int.parse(creature[1]);
            this.attack = int.parse(creature[2]);
            this.defense = int.parse(creature[3]);
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

            return string.join(", ", keywords);
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

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.id).Append(' ');
            sb.Append(this.baseId).Append(' ');
            sb.Append(this.attack).Append(' ');
            sb.Append(this.defense).Append(' ');
            sb.Append(this.keywords);
            return sb.ToString();
        }

        public string getAsInput(bool isOpponentBoard)
        {
            int position = isOpponentBoard ? -1 : 1;
            StringBuilder s = new StringBuilder();
            s.Append(baseId).Append(" ");
            s.Append(id).Append(" ");
            s.Append(position).Append(" ");
            s.Append(Type.CREATURE.ordinal()).Append(" ");
            s.Append(cost).Append(" ");
            s.Append(attack).Append(" ");
            s.Append(defense).Append(" ");
            s.Append(keywords).Append(" ");
            s.Append(myHealthChange).Append(" ");
            s.Append(oppHealthChange).Append(" ");
            s.Append(cardDraw).Append(" ");
            return s.ToString();
        }

        public string toTooltipText()
        {
            return baseCard.toTooltipText(this);
        }


    }
}