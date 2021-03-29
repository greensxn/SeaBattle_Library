using System;

namespace SeaBattle {
    public class Ship {
        public int Length { get; }
        public bool IsEnemyShip { get; }
        public Coordinate[] Coordinates { get; }

        public Ship(int Length, bool IsEnemyShip, Coordinate[] Coordinates) {
            this.Length = Length;
            this.IsEnemyShip = IsEnemyShip;
            this.Coordinates = Coordinates;
        }

        public Ship(bool IsEnemyShip) => this.IsEnemyShip = IsEnemyShip;

        public override string ToString() {
            return $"Enemy: {IsEnemyShip}; Length: {Length}; Coordinates: {String.Join(", ", (object[])Coordinates)}";
        }
    }
}
