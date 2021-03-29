namespace SeaBattle.Args {
    public class EnemyShotArgs : ShotArgs {
        public bool IsShot { get; }
        public EnemyShotArgs(Coordinate ShotCoordinate, bool IsShot, bool IsHintShot) : base(true, IsHintShot, ShotCoordinate) {
            this.IsShot = IsShot;
        }

        public EnemyShotArgs(EnemyShotArgs e) : base(e) {
            IsShot = e.IsShot;
        }
    }
}