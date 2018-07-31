namespace LOCAM
{
    public class Referee : AbstractReferee
    {
        public MultiplayerGameManager<Player> gameManager;

        private EngineReferee engine = new EngineReferee();

        public static int turn = 0;

        override public void init()
        {
            // Engine
            engine.refereeInit(gameManager);
        }

        override public void gameTurn(int turn)
        {
            this.turn = turn;
            // Engine
            bool end = engine.refereeGameTurn(gameManager, turn);
        }

        override public void onEnd()
        {
            endScreenModule.setScores(gameManager.getPlayers().stream().mapToInt(p->p.getScore()).toArray());
        }
    }
}
