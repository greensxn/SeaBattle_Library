using SeaBattle.Args;

namespace SeaBattle {
    public class HintShotsArgs : ShotArgs {

        public Coordinate[] Area { get; }

        public HintShotsArgs(bool IsEnemyShot, bool IsHintShot, Coordinate Coordinate, Coordinate[] Area) : base(IsEnemyShot, IsHintShot, Coordinate) {
            this.Area = Area;
        }

    }
}