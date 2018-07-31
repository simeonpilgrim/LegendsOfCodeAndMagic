using System;
using System.Collections.Generic;
using System.Text;

namespace LOCAM
{

    /**
     * Created by Kot on 2018-03-20.
     */
    public class Card
    {

        public enum Type
        {
            CREATURE,
            ITEM_GREEN,
            ITEM_RED,
            ITEM_BLUE
        }

        //CREATURE("creature"),
        //ITEM_GREEN("itemGreen"),
        //ITEM_RED("itemRed"),
        //ITEM_BLUE("itemBlue");

        public Type TypeFromDescription(string description)
        {
            switch (description)
            {
                case "creature": return Type.CREATURE;
                case "itemGreen": return Type.ITEM_GREEN;
                case "itemRed": return Type.ITEM_RED;
                case "itemBlue": return Type.ITEM_BLUE;
            }
            throw new System.InvalidOperationException();
        }



        public int id;
        public int baseId;
        public Type type;
        public int cost;
        public int attack;
        public int defense;
        public Keywords keywords;
        //TODO maybe myHealthChange, oppHealthChange, cardDraw should be moved into Summon class?
        public int myHealthChange;
        public int oppHealthChange;
        public int cardDraw;
        public string name;
        public string text;
        private string tooltipTextBase;
        public string comment;



        // todo copy constructor with id; ?
        // todo constructor with text (id-based)

        // copy constructor
        public Card(Card card)
        {
            this.id = card.id;
            this.baseId = card.baseId;
            this.name = card.name;
            this.type = card.type;
            this.cost = card.cost;
            this.attack = card.attack;
            this.defense = card.defense;
            this.keywords = new Keywords(card.keywords);
            this.myHealthChange = card.myHealthChange;
            this.oppHealthChange = card.oppHealthChange;
            this.cardDraw = card.cardDraw;
            this.comment = card.comment;
            generateText();
            this.tooltipTextBase = card.tooltipTextBase;
        }

        // data = {baseId, name, type, cost, attack, defense, keywords, myHealthChange, oppHealthChange, cardDraw, comment}
        public Card(string[] data)
        {
            this.id = -1;
            this.baseId = int.Parse(data[0]);
            this.name = data[1];
            this.type = TypeFromDescription(data[2]);
            this.cost = int.Parse(data[3]);
            this.attack = int.Parse(data[4]);
            this.defense = int.Parse(data[5]);
            this.keywords = new Keywords(data[6]);
            this.myHealthChange = int.Parse(data[7]);
            this.oppHealthChange = int.Parse(data[8]);
            this.cardDraw = int.Parse(data[9]);

            this.comment = ""; //data[11]; // comments deprecated as we are far from TESL in many cards
            generateText();
            try
            {
                this.tooltipTextBase = data[10];
            }
            catch (java.lang.ArrayIndexOutOfBoundsException e)
            {
                this.tooltipTextBase = "";
            }
        }

        public void generateText()
        {
            StringBuilder sb = new StringBuilder();

            List<string> keywords = this.keywords.getListOfKeywords();

            ArrayList<string> summon = new ArrayList<>();

            if (myHealthChange > 0)
                summon.add("gain " + myHealthChange + " health");
            if (myHealthChange < 0)
                summon.add("deal " + Math.abs(myHealthChange) + " damage to yourself");
            if (oppHealthChange < 0)
                summon.add("deal " + Math.abs(oppHealthChange) + " damage to your opponent");
            if (cardDraw > 0)
                summon.add("draw " + (cardDraw == 1 ? "a card" : (cardDraw + " cards")));

            if (type == Type.CREATURE)
            {
                sb.Append(string.join(", ", keywords));
                if (myHealthChange != 0 || oppHealthChange != 0 || cardDraw != 0)
                {
                    sb.Append(keywords.isEmpty() ? "" : "; ").Append("Summon: ");
                    sb.Append(string.join(", ", summon)).Append(".");
                }
            }
            else if (type == Type.ITEM_GREEN)
            {
                if (!keywords.isEmpty())
                    sb.Append("Give ").Append(string.join(", ", keywords));
                if (myHealthChange != 0 || oppHealthChange != 0 || cardDraw != 0)
                {
                    sb.Append(keywords.isEmpty() ? "" : "; ");
                    sb.Append(string.join(", ", summon)).Append(".");
                }
            }
            else if (type == Type.ITEM_RED)
            {
                if (!keywords.isEmpty())
                    sb.Append("Remove ").Append(keywords.size() == 7 ? "all keywords" : string.join(", ", keywords));
                if (myHealthChange != 0 || oppHealthChange != 0 || cardDraw != 0)
                {
                    sb.Append(keywords.isEmpty() ? "" : "; ");
                    sb.Append(string.join(", ", summon)).Append(".");
                }
            }
            else
            {
                sb.Append(string.join(", ", summon)).Append(".");
            }

            this.text = sb.ToString();
        }

        private string toTextDescription()
        {
            StringBuilder sb = new StringBuilder();
            NumberFormat nf = new DecimalFormat("+#;-#");

            if (type == Type.CREATURE)
            {
                sb.Append(attack).Append(" / ").Append(defense).Append(" creature. ").Append(text);
            }
            else if (type == Type.ITEM_GREEN)
            {
                sb.Append(text);
                if (attack != 0 || defense != 0)
                    sb.Append(" Give friendly creature ").Append(nf.format(attack)).Append(" / ").Append(nf.format(defense));
            }
            else if (type == Type.ITEM_RED)
            {
                sb.Append(text);
                if (attack != 0 || defense != 0)
                    sb.Append(" Give enemy creature ").Append(attack).Append(" / ").Append(defense);
            }
            else
            {
                sb.Append(defense != 0 ? ("Deal " + Math.Abs(defense) + " damage. ") : "").Append(text.Length() > 1 ? text : "");
            }

            return sb.ToString();
        }

        private string toTooltipInnerText()
        {
            return toTooltipInnerText(null);
        }

        private string toTooltipInnerText(CreatureOnBoard creatureOnBoard)
        {
            StringBuilder sb = new StringBuilder();

            if (type == Type.CREATURE)
            {
                Keywords keywords;
                if (creatureOnBoard != null)
                {
                    keywords = creatureOnBoard.keywords;
                    int aDiff = creatureOnBoard.attack - attack;
                    int dDiff = creatureOnBoard.defense - defense;
                    if (aDiff != 0 || dDiff != 0)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(attack).Append(" / ").Append(defense).Append(" Creature");
                    if (aDiff != 0 || dDiff != 0)
                    {
                        sb.Append("\n");
                        if (aDiff > 0)
                        {
                            sb.Append("+");
                        }
                        else if (aDiff == 0)
                        {
                            sb.Append(" ");
                        }
                        sb.Append(aDiff == 0 ? " " : aDiff).Append("  ");
                        if (dDiff > 0)
                        {
                            sb.Append("+");
                        }
                        else if (dDiff == 0)
                        {
                            sb.Append(" ");
                        }
                        sb.Append(dDiff == 0 ? " " : dDiff).Append(" ");
                    }
                }
                else
                {
                    keywords = this.keywords;
                    sb.Append(attack).Append(" / ").Append(defense).Append(" Creature");
                }
                List<string> keywordsList = keywords.getListOfKeywords();

                if (!keywordsList.isEmpty())
                    sb.Append("\\n").Append(string.join(", ", keywordsList));
                string tt = tooltipTextBase.trim();
                if (tt.length() > 0)
                    sb.Append("\\n").Append(tt);
                return sb.ToString();
            }

            if (type == Type.ITEM_GREEN)
                sb.Append("Green Item\\n");
            if (type == Type.ITEM_RED)
                sb.Append("Red Item\\n");
            if (type == Type.ITEM_BLUE)
                sb.Append("Blue Item\\n");
            sb.Append(tooltipTextBase.trim());

            return sb.ToString();
        }

        public string toTooltipText(CreatureOnBoard creatureOnBoard)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.name + " (#" + baseId + ")").Append("\n\n");

            if (id >= 0) sb.Append("instanceId: ").Append(this.id).Append("\n");
            sb.Append("cost: ").Append(this.cost).Append("\n");
            sb.Append("\n");

            sb.Append(toTooltipInnerText(creatureOnBoard).replace("\\n", "\n"));

            return sb.ToString();
        }

        public string toHTMLString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<tr>");
            sb.Append("<td>").Append(baseId).Append("</td>");
            //string portraitpath = string.Format("src=\"../assets/card_images/%03d.png\"", baseId);
            string portraitimage = string.Format("src=\"portraits/%03d.png\"", baseId);
            string cardimage = string.Format("src=\"cards/%03d.png\"", baseId);
            //sb.Append("<td>").Append("<a><img "+portraitimage+" width=\"50\"><img class=\"enlarge\" "+cardimage+" ></a>").Append("</td>"); // width="300" -- deprecated for now
            sb.Append("<td>").Append("<a><img " + portraitimage + " width=\"50\"><img class=\"enlarge\" " + portraitimage + " ></a>").Append("</td>"); // width="300" -- deprecated for now

            sb.Append("<td><b><a>" + name + "<img class=\"enlarge\" " + cardimage + " ></a></b></td>");

            sb.Append("<td>").Append(type.getDescription()).Append("</td>");

            sb.Append("<td>").Append(cost).Append("</td>");
            sb.Append("<td>").Append(attack).Append("</td>");
            sb.Append("<td>").Append(defense).Append("</td>");

            sb.Append("<td><div class=\"keywords\">");
            sb.Append(this.keywords);
            sb.Append("</div></td>");

            sb.Append("<td>").Append(myHealthChange).Append("</td>");
            sb.Append("<td>").Append(oppHealthChange).Append("</td>");
            sb.Append("<td>").Append(cardDraw).Append("</td>");

            Pattern tesllink = Pattern.compile("@(\\d+)=(['\\w-]+)");
            string htmlcomment = comment;
            Matcher m = tesllink.matcher(htmlcomment);
            while (m.find())
            {
                string lowname = m.group(2).replace("-", "").toLowerCase();
                string fullname = m.group(2).replace("-", " ");
                string link = string.Format("<a href=\"https://www.legends-decks.com/card/%s/%s\">%s<img class=\"enlarge\" src=\"https://www.legends-decks.com/img_cards/%s.png\"></a>",
                        m.group(1), lowname, fullname, lowname);
                htmlcomment = htmlcomment.replace(m.group(0), link);
            }

            string tt = toTooltipInnerText();
            if (tt.length() > 0)
                sb.Append("<td><i>").Append(tt.replace("\\n", "<br/>")).Append("</i></td>");
            else
                sb.Append("<td><i>").Append(toTextDescription()).Append("</i></td>");

            sb.Append("<!--<td>").Append(htmlcomment).Append("</td>-->");


            // card = "<tr><td>" + card.replace(";", "</td><td>") + "</td></tr>\n";


            sb.Append("</tr>");
            // todo
            return sb.ToString();
        }

        public string toDescriptiveString()
        {
            StringBuilder sb = new StringBuilder();
            if (id >= 0) sb.Append("id:").Append(this.id).Append(' ');
            if (name!="?") sb.Append(this.name).Append(' ');
            sb.Append("(#").Append(this.baseId).Append(")").Append(' ');
            sb.Append(type.getDescription()).Append(' ');

            sb.Append("COST:").Append(this.cost).Append(' ');
            if (this.type == Type.CREATURE)
            {
                sb.Append("ATT:").Append(this.attack).Append(' ');
                sb.Append("DEF:").Append(this.defense).Append(' ');
            }
            else // items
            {
                sb.Append("ATT:").Append(string.Format("%+d", this.attack)).Append(' ');
                sb.Append("DEF:").Append(string.Format("%+d", this.defense)).Append(' ');
            }

            sb.Append(" ").Append(this.text);

            return sb.ToString();
        }

        public string toStringWithoutId()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.baseId).Append(' ');
            //sb.Append(this.type.getDescription()).Append(' ');
            sb.Append((int)this.type).Append(' '); // todo test is it ok? 0, 1, 2, 3
            sb.Append(this.cost).Append(' ');
            sb.Append(this.attack).Append(' ');
            sb.Append(this.defense).Append(' ');
            sb.Append(this.keywords);
            sb.Append(' ');
            sb.Append(this.myHealthChange).Append(' ');
            sb.Append(this.oppHealthChange).Append(' ');
            sb.Append(this.cardDraw).Append(' ');
            return sb.ToString();
        }

        override public string ToString()
        {
            return this.id + " " + toStringWithoutId();
        }

        public string getAsInput()
        {
            StringBuilder s = new StringBuilder();

            s.Append(baseId).Append(" ");
            s.Append(id).Append(" ");
            s.Append(0).Append(" ");
            s.Append((int)type).Append(" ");
            s.Append(cost).Append(" ");
            s.Append(attack).Append(" ");
            s.Append(defense).Append(" ");
            s.Append(keywords).Append(" ");
            s.Append(myHealthChange).Append(" ");
            s.Append(oppHealthChange).Append(" ");
            s.Append(cardDraw).Append(" ");
            return s.ToString();
        }
    }
}
