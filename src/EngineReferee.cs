using System;
using System.Collections.Generic;

namespace LOCAM
{

    public class EngineReferee
    {
        //private MultiplayerGameManager<Player> gameManager; // @Inject ?

        public DraftPhase draft;
        public GameState state = null;

        public int gamePlayer = 0;
        public int gameTurn = 0;
        public List<Action> actionsToHandle = new List<Action>();

        private bool showStart = true;

        const int ILLEGAL_ACTION_SUMMARY_LIMIT = 3;

        public void refereeInit(MultiplayerGameManager<Player> gameManager)
        {
            if (Constants.VERBOSE_LEVEL > 1) Console.WriteLine("New game");

            RefereeParams _params = new RefereeParams(gameManager);

            DraftPhase.Difficulty difficulty;
            switch (gameManager.getLeagueLevel())
            {
                case 1:
                    difficulty = DraftPhase.Difficulty.VERY_EASY;
                    break;
                case 2:
                    difficulty = DraftPhase.Difficulty.EASY;
                    break;
                case 3:
                    difficulty = DraftPhase.Difficulty.LESS_EASY;
                    break;
                default:
                    difficulty = DraftPhase.Difficulty.NORMAL;
                    break;
            }


            Constants.LoadCardlist("cardlist.txt");
            if (Constants.VERBOSE_LEVEL > 1) Console.WriteLine("   CARDSET with " + Constants.CARDSET.Count + " cards loaded.");
            if (Constants.VERBOSE_LEVEL > 1) Console.WriteLine("   Difficulty is set to: " + difficulty.ToString() + ".");

            draft = new DraftPhase(difficulty, _params);
            draft.PrepareChoices();

            if (Constants.VERBOSE_LEVEL > 1) Console.WriteLine("   Draw Phase Prepared. " + draft.allowedCards.Count + " cards allowed. ");
            if (Constants.VERBOSE_LEVEL > 1) Console.WriteLine("   " + draft.draftingCards.Count + " cards selected to the draft.");

            gameManager.setMaxTurns(Constants.MAX_TURNS_HARDLIMIT); // should be never reached, not handled on the referee's side
        }


        public bool refereeGameTurn(MultiplayerGameManager<Player> gameManager, int turn)
        {
            if (showStart && gameTurn == Constants.CARDS_IN_DECK)
            {
                showStart = false;
                //gameManager.addTooltip(gameManager.getPlayer(0), "Battle start!");

            }
            if (gameTurn < Constants.CARDS_IN_DECK)
            {
                DraftTurn(gameManager);
                return false;
            }
            else
            {
                return GameTurn(gameManager);
            }
        }

        private void DraftTurn(MultiplayerGameManager<Player> gameManager)
        {
            if (Constants.VERBOSE_LEVEL > 1 && gameTurn == 0) Console.WriteLine("   Draft phase");
            if (Constants.VERBOSE_LEVEL > 2) Console.WriteLine("      Draft turn " + gameTurn + "/" + Constants.CARDS_IN_DECK);

            gameManager.setTurnMaxTime(gameTurn == 0 ? Constants.TIMELIMIT_FIRSTDRAFTTURN : Constants.TIMELIMIT_DRAFTTURN);

            for (int player = 0; player < 2; player++)
            {
                Player sdkplayer = gameManager.getPlayer(player);
                foreach (string line in draft.getMockPlayersInput())
                {
                    sdkplayer.sendInputLine(line);
                }

                for (int card = 0; card < 3; card++)
                    sdkplayer.sendInputLine(draft.draft[gameTurn, card].getAsInput());
                sdkplayer.execute();
            }

            for (int player = 0; player < 2; player++)
            {
                Player sdkplayer = gameManager.getPlayer(player);
                try
                {
                    string output = sdkplayer.getOutputs()[0];
                    DraftPhase.ChoiceResultPair choice = draft.PlayerChoice(gameTurn, output, player);
                    draft.text[player] = choice.text;
                    gameManager.addToGameSummary(
                        string.Format("Player %s chose %s", sdkplayer.getNicknameToken(), choice.card.toDescriptiveString())
                    );
                }
                catch (InvalidActionHard e)
                {
                    HandleError(gameManager, sdkplayer, sdkplayer.getNicknameToken() + ": " + e.Message);
                    return;
                }
                catch (TimeoutException e)
                {
                    HandleError(gameManager, sdkplayer, sdkplayer.getNicknameToken() + " timeout!");
                    return;
                }
            }

            gameTurn++;
        }

        private bool GameTurn(MultiplayerGameManager<Player> gameManager)
        {
            Player sdkplayer = gameManager.getPlayer(gamePlayer);
            gameManager.setFrameDuration(Constants.FRAME_DURATION_BATTLE);

            if (state == null) // frame-only turn for showing the initial state
            {
                draft.ShuffleDecks();
                if (Constants.VERBOSE_LEVEL > 1) Console.WriteLine("   Decks shuffled.");
                if (Constants.VERBOSE_LEVEL > 1) Console.WriteLine("   Game phase");
                state = new GameState(draft);

                gameManager.setTurnMaxTime(1); // weird try but works ^^
                sdkplayer.execute();

                return false;
            }

            if (actionsToHandle.Count > 0) // there is a legal action on top of the list
            {
                gameManager.setTurnMaxTime(1); // weird try but works ^^
                sdkplayer.execute();

                Action a = actionsToHandle[0];
                actionsToHandle.RemoveAt(0);
                gameManager.addToGameSummary("Player " + sdkplayer.getNicknameToken() + " performed action: " + a.toStringNoText());

                state.AdvanceState(a);
                if (a.type == Action.Type.SUMMON)
                {
                    gameManager.setFrameDuration(Constants.FRAME_DURATION_SUMMON);
                }
            }
            else // it's time to actually call a player
            {
                if (Constants.VERBOSE_LEVEL > 2) Console.Write("      Game turn " + (gameTurn - Constants.CARDS_IN_DECK) + ", player " + gamePlayer);

                gameManager.setTurnMaxTime(gameTurn <= Constants.CARDS_IN_DECK + 1 ? Constants.TIMELIMIT_FIRSTGAMETURN : Constants.TIMELIMIT_GAMETURN);

                state.AdvanceState();

                foreach (string line in state.getPlayersInput())
                    sdkplayer.sendInputLine(line);
                foreach (string line in state.getCardsInput())
                    sdkplayer.sendInputLine(line);
                sdkplayer.execute();

                try
                {
                    string output = sdkplayer.getOutputs()[0];
                    actionsToHandle = Action.parseSequence(output);
                    if (Constants.VERBOSE_LEVEL > 2) Console.WriteLine(" (returned " + actionsToHandle.Count + " actions)");
                }
                catch (InvalidActionHard e)
                {
                    HandleError(gameManager, sdkplayer, sdkplayer.getNicknameToken() + ": " + e.Message);
                }
                catch (TimeoutException e)
                {
                    HandleError(gameManager, sdkplayer, sdkplayer.getNicknameToken() + " timeout!");
                }
            }

            // now we roll-out actions until next legal is found
            List<Action> legals = state.computeLegalActions(); //Console.WriteLine(gameTurn + " "+ state.players[state.currentPlayer].currentMana +"/"+state.players[state.currentPlayer].maxMana + "->"+legals);
            int illegalActions = 0;

            while (actionsToHandle.Count > 0)
            {
                Action a = actionsToHandle[0];
                if (a.type == Action.Type.PASS)
                {
                    actionsToHandle.RemoveAt(0); // pop
                    continue;
                }
                if (legals.Contains(a))
                    break;
                actionsToHandle.RemoveAt(0); // pop
                illegalActions++;
                if (illegalActions <= ILLEGAL_ACTION_SUMMARY_LIMIT)
                {
                    gameManager.addToGameSummary("[Warning] " + sdkplayer.getNicknameToken() + " Action is not legal: " + a.ToString());
                }
            }
            if (illegalActions > ILLEGAL_ACTION_SUMMARY_LIMIT)
            {
                gameManager.addToGameSummary("[Warning] " + sdkplayer.getNicknameToken() + " Performed another " + (illegalActions - ILLEGAL_ACTION_SUMMARY_LIMIT) + " illegalActions");
            }

            if (CheckAndHandleEndgame(gameManager, state))
                return true;

            if (actionsToHandle.Count == 0) // player change
            {
                gameTurn++;
                gamePlayer = (gamePlayer + 1) % 2;
            }

            return false;
        }

        private void HandleError(MultiplayerGameManager<Player> gameManager, Player sdkplayer, string errmsg)
        {
            gameManager.addToGameSummary(errmsg);
            sdkplayer.deactivate(errmsg);
            sdkplayer.setScore(-1);
            gameManager.endGame();
        }

        // returns true if the game ends
        private bool CheckAndHandleEndgame(MultiplayerGameManager<Player> gameManager, GameState state)
        {
            if (state.winner == -1)
                return false;

            //gameManager.addToGameSummary("!\n" + state.ToString());

            if (Constants.VERBOSE_LEVEL > 1) Console.WriteLine("   Game finished in turn " + (gameTurn - Constants.CARDS_IN_DECK) + ".");
            if (Constants.VERBOSE_LEVEL > 1) Console.Write("   Scores: ");
            if (Constants.VERBOSE_LEVEL > 0) Console.WriteLine((state.winner == 0 ? "1" : "0") + " " + (state.winner == 1 ? "1" : "0"));

            gameManager.addToGameSummary(gameManager.getPlayer(state.winner).getNicknameToken() + " won!");
            gameManager.getPlayer(state.winner).setScore(1);
            gameManager.endGame();
            return true;
        }
    }
}