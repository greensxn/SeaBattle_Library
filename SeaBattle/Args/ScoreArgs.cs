namespace SeaBattle.Args {
    public class ScoreArgs {
        public PlayerScoreArgs CountMyShips { get; }
        public PlayerScoreArgs CountEnemyShips { get; }

        public ScoreArgs() {
            CountMyShips = new PlayerScoreArgs();
            CountEnemyShips = new PlayerScoreArgs();
        }

        public void Reset() {
            CountMyShips.Reset();
            CountEnemyShips.Reset();
        }

    }
}
