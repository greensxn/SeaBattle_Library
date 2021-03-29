namespace SeaBattle {
    public class Step {

        public Shot Shot { get; }
        public bool IsEnemyStep { get; }
        public bool IsHintShot { get; }
        public Coordinate Coordinate { get; }

        public Step(Shot Shot, Coordinate Coordinate, bool IsEnemyStep, bool IsHintShot) {
            this.Shot = Shot;
            this.Coordinate = Coordinate;
            this.IsEnemyStep = IsEnemyStep;
            this.IsHintShot = IsHintShot;
        }

        public override string ToString() => $"Coordinate: X{Coordinate.X} Y{Coordinate.Y}, Enemy: {IsEnemyStep}, Shot: {Shot}, Hint: {IsHintShot}";
    }
}
