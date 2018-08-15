using System;
using System.Collections.Generic;

namespace LOCAM
{

    /**
     * Created by aCat on 2018-03-22.
     */
    public class Action
    {
        public enum Type { SUMMON, ATTACK, USE, PASS };

        public Type type;
        public int arg1;
        public int arg2;
        public Card.Type cardType; // for items
        public string text; // for say
        public ActionResult result;

        public Action()
        {
        }


        public static List<Action> parseSequence(string data)
        {
            List<Action> actions = new List<Action>();

            foreach (string str in data.Split(";"))
            {
                var tstr = str.Trim();
                if (tstr != "")
                    actions.Add(Action.parse(tstr));
            }

            return actions;
        }

        // todo copy constructor?


        public static Action parse(string data)
        {
            string[] str = data.Split(" ", 2);

            Type type;
            switch (str[0].Trim())
            {
                case "SUMMON": type = Type.SUMMON; break;
                case "ATTACK": type = Type.ATTACK; break;
                case "USE": type = Type.USE; break;
                case "PASS": type = Type.PASS; break;
                default: throw new InvalidActionHard("Invalid action name. Should be SUMMON, ATTACK, or USE.", null);
            }

            if (type == Type.SUMMON)
            {
                try
                {
                    string[] args = str[1].Split(" ", 2);
                    int arg1;
                    arg1 = int.Parse(args[0]);
                    string text = args.Length < 2 ? "" : args[1].Trim();
                    return Action.newSummon(arg1, text);
                }
                catch (Exception e)
                {
                    throw new InvalidActionHard("Invalid SUMMON argument. Expected integer (card id).", e);
                }
            }
            else if (type == Type.PASS)
            {
                return Action.newPass();
            }
            else
            {
                try
                {
                    string[] args = str[1].Split(" ", 3);

                    int arg1;
                    int arg2;
                    arg1 = int.Parse(args[0]);
                    arg2 = int.Parse(args[1]);
                    string text = args.Length < 3 ? "" : args[2].Trim();
                    return type == Type.ATTACK ? Action.newAttack(arg1, arg2, text) : Action.newUse(arg1, arg2, text);
                }
                catch (Exception e)
                {
                    throw new InvalidActionHard("Invalid " + type.ToString() + " arguments. Expected two integers (card id and target id).", e);
                }
            }
        }

        public static Action newSummon(int arg1)
        {
            return newSummon(arg1, "");
        }

        public static Action newSummon(int arg1, string text) // todo private?
        {
            Action a = new Action();
            a.type = Type.SUMMON;
            a.cardType = Card.Type.CREATURE;
            a.arg1 = arg1;
            a.text = text;
            return a;
        }

        public static Action newAttack(int arg1, int arg2)
        {
            return newAttack(arg1, arg2, "");
        }

        public static Action newAttack(int arg1, int arg2, string text) // todo private?
        {
            Action a = new Action();
            a.type = Type.ATTACK;
            a.cardType = Card.Type.CREATURE;
            a.arg1 = arg1;
            a.arg2 = arg2;
            a.text = text;
            return a;
        }

        public static Action newPass()
        {
            Action a = new Action();
            a.type = Type.PASS;
            a.text = "";
            return a;
        }

        public static Action newUse(int arg1, int arg2)
        {
            return newUse(arg1, arg2, "");
        }



        // todo - it's just a shell now
        public static Action newUse(int arg1, int arg2, string text) // todo private?
        {
            Action a = new Action();
            a.type = Type.USE;
            //a.cardType = Card.Type.CREATURE; // todo here
            a.arg1 = arg1;
            a.arg2 = arg2;
            a.text = text;
            return a;
        }

        public string toStringNoText()
        {
            switch (type)
            {
            case Type.SUMMON: return $"SUMMON {arg1}";
            case Type.ATTACK: return $"ATTACK {arg1} {arg2}";
            case Type.USE: return $"USE {arg1} {arg2}";
            case Type.PASS: return "PASS";
            }
            return base.ToString();
        }

        override
        public string ToString()
        {
            switch (type)
            {
            case Type.SUMMON: return $"SUMMON {arg1} {text}";
            case Type.ATTACK: return $"ATTACK {arg1} {arg2} {text}";
            case Type.USE: return $"USE {arg1} {arg2} {text}";
            case Type.PASS: return "PASS";
            }
            return base.ToString();
        }

        override public bool Equals(Object other)
        {
            if (other == null) return false;
            if (other == this) return true;
            if (!(other is Action)) return false;
            Action a = (Action)other;
            return this.toStringNoText().Equals(a.toStringNoText());
        }

        public override int GetHashCode()
        {
            return toStringNoText().GetHashCode();
        }
    }
}