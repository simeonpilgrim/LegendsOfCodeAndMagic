namespace LOCAM
{

    /**
     * Created by aCat on 2018-03-21.
     */
    public class ActionResult
    {
        public bool isValid;
        public CreatureOnBoard attacker;
        public CreatureOnBoard defender;
        public int attackerHealthChange;
        public int defenderHealthChange;
        public int attackerAttackChange;
        public int defenderAttackChange;
        public int attackerDefenseChange;
        public int defenderDefenseChange;
        public bool attackerDied;
        public bool defenderDied;

        public ActionResult(bool isValid)
        {
            this.isValid = isValid;
            this.attacker = null;
            this.defender = null;
            this.attackerHealthChange = 0;
            this.defenderHealthChange = 0;
        }

        public ActionResult(CreatureOnBoard attacker, CreatureOnBoard defender, bool attackerDied, bool defenderDied, int attackerHealthChange, int defenderHealthChange)
        {
            this.isValid = true;
            this.attackerDied = attackerDied;
            this.defenderDied = defenderDied;
            this.attacker = attacker;
            this.defender = defender;
            this.attackerHealthChange = attackerHealthChange;
            this.defenderHealthChange = defenderHealthChange;
        }
    }
}
