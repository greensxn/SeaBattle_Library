namespace SeaBattle.Args {
    public class ShipHitArgs : ShotArgs {
        public ShipHitArgs(bool IsEnemyShot, bool IsHintShot, Coordinate Coordinate) : base(IsEnemyShot, IsHintShot, Coordinate) { }
    }
}
