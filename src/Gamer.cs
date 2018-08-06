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

  public List<int> runes = new List<int>() { 5,10,15,20,25};
  public List<Action> performedActions;
  public int handLimit;

  // todo rest



  public Gamer(int id, List<Card> deck)
  {
    this.id = id;
    this.hand = new List<>();
    this.deck = new List<>(deck);
    this.board = new List<>();
    this.graveyard = new List<>();
    this.performedActions = new List<>();
    this.health = Constants.INITIAL_HEALTH;
    this.maxMana = 0;
    this.currentMana = 0;
    this.nextTurnDraw = 1;

    handLimit = Constants.MAX_CARDS_IN_HAND + (id==0 ? 0 : Constants.SECOND_PLAYER_MAX_CARD_BONUS);
    DrawCards(INITIAL_HAND_SIZE + (id==0 ? 0 : Constants.SECOND_PLAYER_CARD_BONUS), 0);
  }

  private void suicideRunes()
  {
    if (!runes.isEmpty()) // first rune gone
    {
      Integer r = runes.remove(runes.Count - 1);
      health = r;
    }
    else // final run gone - suicide
    {
      health = 0;
    }
  }

  public void DrawCards(int n, int playerturn)
  {
    for (int i=0; i<n; i++)
    {
      if (deck.isEmpty() || playerturn>=Constants.PLAYER_TURNLIMIT)
      {
        suicideRunes();
        continue;
      }

      if (hand.Count==handLimit)
      {
        break; // additional draws are simply wasted
      }

      Card c = deck.remove(0);
      hand.add(c);
    }

  }


  public void ModifyHealth(int mod)
  {
    health += mod;

    if (mod >= 0)
      return;

    for (int r=runes.Count-1; r >=0; r--) // rune checking;
    {
      if (health <= runes.get(r))
      {
        nextTurnDraw += 1;
        runes.remove(r);
      }
    }
  }

  public int nextRune()
  {
    if (runes.isEmpty()) return 0;
    return runes.get(runes.Count-1);
  }

  public void removeFromBoard(int creatureIndex) {
      graveyard.add(board.remove(creatureIndex));
  }

  public string toDescriptiveString(bool reverse)
  {
    string line1 = string.Format("[Player %d] Health: %d %s     Mana: %d/%d", id, health, runes, currentMana, maxMana);
    string line2 = string.Format("Cards in hand: %d   In deck: %d   Next turn draw: %d", hand.Count, deck.Count, nextTurnDraw);

    ArrayList<string> inhand = new ArrayList<>();
    inhand.add("Hand:");
    for (Card c: hand)
      inhand.add((c.cost <= this.currentMana ? " * " : "   ") + c.toDescriptiveString());

    ArrayList<string> onboard = new ArrayList<>();
    onboard.add("Board:");
    for (CreatureOnBoard c: board)
      onboard.add((c.canAttack ? " * " : "   ") + c.toDescriptiveString());

    ArrayList<string> description = new ArrayList<>();
    description.add(line1);
    description.add(line2);
    description.add(string.join("\n",inhand));
    description.add(string.join("\n",onboard));
    if (reverse)
      Collections.reverse(description);

    return string.join("\n", description);
  }

  // todo
  public string ToString()
  {
    return super.ToString();
  }

  public string getPlayerInput() {
	  StringBuilder s = new StringBuilder();
	  s.Append(health).Append(" ");
	  s.Append(maxMana).Append(" ");
	  s.Append(deck.Count).Append(" ");
	  s.Append(nextRune());
	  return s.ToString();
  }
}
}