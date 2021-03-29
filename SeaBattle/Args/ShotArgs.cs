namespace SeaBattle.Args {
    public class ShotArgs {
        public bool IsEnemyShot { get; }
        public bool IsHintShot { get; }
        public Coordinate ShotCoordinate { get; }

        public ShotArgs(bool IsEnemyShot, bool IsHintShot, Coordinate ShotCoordinate) {
            this.IsEnemyShot = IsEnemyShot;
            this.IsHintShot = IsHintShot;
            this.ShotCoordinate = ShotCoordinate;
        }

        public ShotArgs(ShotArgs e) {
            ShotCoordinate = e.ShotCoordinate;
            IsEnemyShot = e.IsEnemyShot;
            IsHintShot = e.IsHintShot;
        }

    }
}
