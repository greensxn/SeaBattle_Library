namespace SeaBattle.Args {

    public class ShipMissShotArgs : ShotArgs {
        public ShipMissShotArgs(bool IsEnemyShot, bool IsHintShot, Coordinate Coordinate) : base(IsEnemyShot, IsHintShot, Coordinate) { }
    }
}
