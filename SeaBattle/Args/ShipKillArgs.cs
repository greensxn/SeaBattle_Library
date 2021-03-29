namespace SeaBattle.Args {
    public class ShipKillArgs : ShotArgs {
        public Ship Ship { get; }
        public Coordinate[] Marks { get; }

        public ShipKillArgs(Ship Ship, Coordinate[] Marks, Coordinate ShotCoordinate, bool IsEnemyShot, bool IsHintShot) : base(IsEnemyShot, IsHintShot, ShotCoordinate) {
            this.Ship = Ship;
            this.Marks = Marks;
        }
    }
}
