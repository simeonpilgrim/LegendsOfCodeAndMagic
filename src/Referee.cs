namespace LOCAM
{
    public class Referee
    {
        public MultiplayerGameManager<Player> gameManager;

        private EngineReferee engine = new EngineReferee();

        public int turn = 0;

        public void init()
        {
            // Engine
            engine.refereeInit(gameManager);
        }

        public void gameTurn(int turn)
        {
            this.turn = turn;
            // Engine
            bool end = engine.refereeGameTurn(gameManager, turn);
        }

        public void onEnd()
        {
            //endScreenModule.setScores(gameManager.getPlayers()..mapToInt(p->p.getScore()).toArray());
        }
    }
}
