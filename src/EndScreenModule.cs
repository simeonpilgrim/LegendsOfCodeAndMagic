namespace LOCAM
{
    public class EndScreenModule : Module {

    private GameManager<AbstractPlayer> gameManager;
    private int[] scores;

    EndScreenModule(GameManager<AbstractPlayer> gameManager) {
        this.gameManager = gameManager;
        gameManager.registerModule(this);
    }

    public void setScores(int[] scores) {
        this.scores = scores;
    }

    override public void onGameInit() {
    }
    override public void onAfterGameTurn() {
    }

    override public void onAfterOnEnd() {
        gameManager.setViewData("endScreen", scores);
    }

}
}
