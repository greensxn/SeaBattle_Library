namespace SeaBattle.Args {
    public class MyShotArgs : ShotArgs {
        public bool IsShot { get; }
        public MyShotArgs(Coordinate ShotCoordinate, bool IsShot, bool IsHintShot) : base(false, IsHintShot, ShotCoordinate) {
            this.IsShot = IsShot;
        }
    }
}