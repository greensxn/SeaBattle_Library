using System;

namespace SeaBattle {
    public class FinishOfTheShip {
        public Coordinate[] Coordinates { get; }
        public Coordinate[] WrongShot { get; set; }
        public bool IsHorisontal { get; private set; }
        public Derection[] MoveList { get; }

        public FinishOfTheShip(Coordinate ShipFirstCoordinate, Derection[] MoveList) {
            Coordinates = new Coordinate[4];
            WrongShot = new Coordinate[3];
            Coordinates[0] = ShipFirstCoordinate;
            this.MoveList = MoveList;
        }

        public void AddCoordinate(Coordinate EnemyShot) {
            Coordinates[Coordinates.GetLengthWithoutNull()] = EnemyShot;

            IsHorisontal = Coordinates[1] != null && Coordinates[0].X == Coordinates[1].X;
        }

        public Coordinate GetRandomShot() {
            if (Coordinates[1] != null) {
                Coordinate lastShot = GetLastShot();
                Random r = new Random();
                bool IsChangeMove = false;
                Derection move;
                if (IsHorisontal) {
                    move = r.Next(0, 2) == 0 ? Derection.Left : Derection.Right;
                ChangeMoveHorisontal:
                    if (IsChangeMove)
                        move = move == Derection.Left ? Derection.Right : Derection.Left;
                    if (move == Derection.Left) {
                        if (Coordinates[0].Y - 1 >= 0) {
                            for (int i = Coordinates[0].Y - 1; i >= 0; i--) {
                                if (Exist(Coordinates, new Coordinate(Coordinates[0].X, i)))
                                    continue;
                                if (Exist(WrongShot, new Coordinate(Coordinates[0].X, i))) {
                                    IsChangeMove = true;
                                    goto ChangeMoveHorisontal;
                                }
                                return new Coordinate(Coordinates[0].X, i);
                            }
                        }
                        IsChangeMove = true;
                        goto ChangeMoveHorisontal;
                    }
                    else {
                        if (Coordinates[0].Y + 1 < 10) {
                            for (int i = Coordinates[0].Y + 1; i < 10; i++) {
                                if (Exist(Coordinates, new Coordinate(Coordinates[0].X, i)))
                                    continue;
                                if (Exist(WrongShot, new Coordinate(Coordinates[0].X, i))) {
                                    IsChangeMove = true;
                                    goto ChangeMoveHorisontal;
                                }
                                return new Coordinate(Coordinates[0].X, i);
                            }
                        }
                        IsChangeMove = true;
                        goto ChangeMoveHorisontal;
                    }
                }
                else {
                    move = r.Next(0, 2) == 0 ? Derection.Up : Derection.Down;
                ChangeMoveVertical:
                    if (IsChangeMove)
                        move = move == Derection.Up ? Derection.Down : Derection.Up;
                    if (move == Derection.Up) {
                        if (Coordinates[0].X - 1 >= 0) {
                            for (int i = Coordinates[0].X - 1; i >= 0; i--) {
                                if (Exist(Coordinates, new Coordinate(i, Coordinates[0].Y)))
                                    continue;
                                if (Exist(WrongShot, new Coordinate(i, Coordinates[0].Y))) {
                                    IsChangeMove = true;
                                    goto ChangeMoveVertical;
                                }
                                return new Coordinate(i, Coordinates[0].Y);
                            }
                        }
                        IsChangeMove = true;
                        goto ChangeMoveVertical;
                    }
                    else {
                        if (Coordinates[0].X + 1 < 10) {
                            for (int i = Coordinates[0].X + 1; i < 10; i++) {
                                if (Exist(Coordinates, new Coordinate(i, Coordinates[0].Y)))
                                    continue;
                                if (Exist(WrongShot, new Coordinate(i, Coordinates[0].Y))) {
                                    IsChangeMove = true;
                                    goto ChangeMoveVertical;
                                }
                                return new Coordinate(i, Coordinates[0].Y);
                            }
                        }
                        IsChangeMove = true;
                        goto ChangeMoveVertical;
                    }
                }
            }
            else {
                int lastShot = Coordinates.GetLengthWithoutNull() - 1;
                Coordinate coor = null;
            NewShot:
                MoveList.Shuffle();
                for (int i = 0; i < 4; i++) {
                    if (Coordinates[lastShot].X + 1 < 10 && MoveList[i] == Derection.Down) {
                        coor = new Coordinate(Coordinates[lastShot].X + 1, Coordinates[lastShot].Y);
                        break;
                    }
                    if (Coordinates[lastShot].Y - 1 >= 0 && MoveList[i] == Derection.Left) {
                        coor = new Coordinate(Coordinates[lastShot].X, Coordinates[lastShot].Y - 1);
                        break;
                    }
                    if (Coordinates[lastShot].X - 1 >= 0 && MoveList[i] == Derection.Up) {
                        coor = new Coordinate(Coordinates[lastShot].X - 1, Coordinates[lastShot].Y);
                        break;
                    }
                    if (Coordinates[lastShot].Y + 1 < 10 && MoveList[i] == Derection.Right) {
                        coor = new Coordinate(Coordinates[lastShot].X, Coordinates[lastShot].Y + 1);
                        break;
                    }
                }
                if (Exist(WrongShot, coor))
                    goto NewShot;
                return coor;
            }
        }

        public void AddWrongShot(Coordinate shot) => WrongShot[WrongShot.GetLengthWithoutNull()] = shot;

        private Coordinate GetLastShot() {
            for (int i = 0; i < Coordinates.Length; i++)
                if (Coordinates[i] == null)
                    return Coordinates[i - 1];
            return Coordinates[Coordinates.Length - 1];
        }

        private bool Exist(Coordinate[] array, Coordinate coordinate) {
            foreach (Coordinate item in array)
                if (item != null && item.X == coordinate.X && item.Y == coordinate.Y)
                    return true;
            return false;
        }
    }
}
