using System.Collections;

namespace SeaBattle {
    public class Coordinate  {
        public int X { get; }
        public int Y { get; }

        public Coordinate(int X, int Y) {
            this.X = X;
            this.Y = Y;
        }

        public Coordinate(Coordinate Coordinate) {
            X = Coordinate.X;
            Y = Coordinate.Y;
        }

        public override string ToString() => $"X - {X} Y - {Y}";

        public static bool operator ==(Coordinate left, Coordinate right) {
            if (ReferenceEquals(left, null)) {
                if (ReferenceEquals(right, null))
                    return true;
                return false;
            }
            if (right != null && left.X == right.X && left.Y == right.Y)
                return true;
            return false;
        }
        public static bool operator !=(Coordinate left, Coordinate right) {
            return !(left == right);
        }
    }
}
