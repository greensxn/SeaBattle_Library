using System;
using System.Threading.Tasks;

using SeaBattle.Args;

namespace SeaBattle
{
    public partial class Game
    {

        public Customization Settings { get; set; }
        public Moves History { get; set; }
        public ScoreArgs Score { get; set; }
        public bool[,] MyFlot { get; private set; }
        public bool[,] MyShot { get; private set; }
        public bool[,] MyMarkField { get; private set; }
        public bool[,] EnemyFlot { get; private set; }
        public bool[,] EnemyShot { get; private set; }
        public bool[,] EnemyMarkField { get; private set; }
        public bool IsEnemyHintUsed { get; private set; }
        public bool IsEnemyTurn { get; set; }
        public bool IsGameReady
        {
            get => isGameReady;
            set
            {
                isGameReady = value;
                OnGameReady?.Invoke(isGameReady);
            }
        }
        public bool IsGameOver
        {
            get => isGameOver;
            set
            {
                isGameOver = value;
                if (isGameOver)
                    OnGameOver?.Invoke(Score);
            }
        }
        public bool IsStartBattle
        {
            get => isStartBattle;
            set
            {
                isStartBattle = value;
                OnGameStarted?.Invoke();
            }
        }
        public event Action OnGameStarted;
        public event Action OnNewGame;
        public event Action OnHintShooted;
        public event Action<Coordinate> OnSetShip;
        public event Action<Coordinate> OnRemoveShip;
        public event Action<bool> OnGameReady;
        public event Action<Turn> OnTurn;
        public event Action<Ship> OnSetRandomShips;
        public event Action<ShipKillArgs> OnKillShip;
        public event Action<ShipHitArgs> OnHitShip;
        public event Action<ShipMissShotArgs> OnMissShotShip;
        public event Action<HintShotsArgs> OnHintShots;
        public event Action<MyShotArgs> OnMyShot;
        public event Action<EnemyShotArgs> OnEnemyShot;
        public event Action<ScoreArgs> OnGameOver;
        public event Action<ScoreArgs> OnScore;

        private bool isStartBattle { get; set; }
        private bool isGameOver { get; set; }
        private bool isGameReady { get; set; }
        private FinishOfTheShip EnemyHitShip { get; set; }
        private Derection[] moveList;

        public Game() {
            moveList = new Derection[] { Derection.Up, Derection.Right, Derection.Left, Derection.Down };
            Settings = new Customization();
            if (Settings.IsHistory)
                History = new Moves();
            OnSetShip += SeaWar_OnSetShip;
            OnRemoveShip += SeaWar_OnRemoveShip;
            OnHitShip += Game_OnHitShip;
            OnKillShip += Game_OnKillShip;
            OnMissShotShip += Game_OnMissShotShip;
            OnHintShots += Game_OnHintShot;
            OnTurn += Game_OnTurn;
        }

        public void NewGame() {
            Score = new ScoreArgs();
            MyFlot = new bool[10, 10];
            MyShot = new bool[10, 10];
            MyMarkField = new bool[10, 10];
            EnemyFlot = new bool[10, 10];
            EnemyShot = new bool[10, 10];
            EnemyMarkField = new bool[10, 10];
            EnemyHitShip = null;
            isGameReady = isStartBattle = IsEnemyHintUsed = isGameOver = false;
            OnNewGame?.Invoke();
        }

        public void StartBattle() {
            IsStartBattle = true;
            IsGameReady = false;
            IsEnemyTurn = false;
            OnTurn?.Invoke(IsEnemyTurn ? Turn.Enemy : Turn.My);
            OnScore?.Invoke(Score);
        }

        public void SetMyShip(Coordinate ShipCoordinate) {
            MyFlot[ShipCoordinate.X, ShipCoordinate.Y] = true;
            OnSetShip?.Invoke(ShipCoordinate);
        }

        public void RemoveMyShip(Coordinate ShipCoordinate) {
            MyFlot[ShipCoordinate.X, ShipCoordinate.Y] = false;
            OnRemoveShip?.Invoke(ShipCoordinate);
        }

        public bool SetMyShot(Coordinate ShotCoordinate) {
            if (!MyShot[ShotCoordinate.X, ShotCoordinate.Y]) {
                MyShot[ShotCoordinate.X, ShotCoordinate.Y] = true;
                if (EnemyFlot[ShotCoordinate.X, ShotCoordinate.Y]) {
                    IsEnemyTurn = false;
                    ShipKillArgs args = CheckShipKill(ShotCoordinate, true, false);
                    if (args.Ship.Length > 0) {
                        SetMarkAroundShip(ShotCoordinate, true);
                        OnKillShip?.Invoke(args);
                    }
                    else if (args.Ship.Length == 0)
                        OnHitShip?.Invoke(new ShipHitArgs(false, false, ShotCoordinate));
                    OnMyShot?.Invoke(new MyShotArgs(ShotCoordinate, false, true));
                }
                else {
                    IsEnemyTurn = true;
                    OnMyShot?.Invoke(new MyShotArgs(ShotCoordinate, false, false));
                    OnMissShotShip?.Invoke(new ShipMissShotArgs(false, false, ShotCoordinate));
                }
                CheckGameOver();
                OnTurn?.Invoke(IsEnemyTurn ? Turn.Enemy : Turn.My);
                return true;
            }
            else {
                OnTurn?.Invoke(Turn.My);
                return false;
            }
        }

        public Coordinate[] SetHintShot(Coordinate ShotCoordinate, bool IsEnemyShot) {
            bool[,] flot = IsEnemyShot ? MyFlot : EnemyFlot;
            bool[,] shot = IsEnemyShot ? EnemyShot : MyShot;
            bool IsKillShip = false;
            ShipKillArgs args = null;
            IsEnemyTurn = IsEnemyShot ? true : false;
            Coordinate[] area = GetHintArea(ShotCoordinate, IsEnemyShot);
            OnHintShots?.Invoke(new HintShotsArgs(IsEnemyShot, true, ShotCoordinate, GetHintArea(ShotCoordinate, IsEnemyShot)));
            for (int i = 0; i < area.Length; i++) {
                if (area[i] == null)
                    break;
                shot[area[i].X, area[i].Y] = true;
                if (flot[area[i].X, area[i].Y]) {
                    args = CheckShipKill(area[i], !IsEnemyShot, true);
                    if (!IsKillShip)
                        IsKillShip = args.Ship.Length > 0 ? true : false;
                    if (args.Ship.Length == 0) {
                        if (IsEnemyShot && EnemyHitShip == null) {
                            EnemyHitShip = new FinishOfTheShip(area[i], moveList);
                            IsEnemyTurn = false;
                        }
                        OnHitShip?.Invoke(new ShipHitArgs(IsEnemyShot, true, area[i]));
                    }
                    else if (args.Ship.Length > 0) {
                        if (IsEnemyShot) {
                            EnemyHitShip = null;
                            IsEnemyTurn = false;
                        }
                        SetMarkAroundShip(area[i], !IsEnemyShot);
                        OnKillShip?.Invoke(args);
                    }
                }
                else {
                    OnMissShotShip?.Invoke(new ShipMissShotArgs(IsEnemyShot, true, area[i]));
                }
            }
            IsEnemyTurn = IsEnemyShot ? IsKillShip : !IsKillShip;

            CheckGameOver();
            if (IsEnemyShot) {
                MyFlot = flot;
                EnemyShot = shot;
            }
            else {
                EnemyFlot = flot;
                MyShot = shot;
            }
            OnHintShooted?.Invoke();
            OnTurn?.Invoke(IsEnemyTurn ? Turn.Enemy : Turn.My);
            return area;
        }

        public async Task<Coordinate> SetEnemyShotAsync(int MinMS, int MaxMS) {
            Random r = new Random();
            await Task.Delay(r.Next(MinMS, MaxMS));
            return SetEnemyShot();
        }

        private Coordinate SetEnemyShot() {
            if (IsGameOver)
                return null;
            IsEnemyTurn = true;
            Random r = new Random();
            if (EnemyHitShip != null && Settings.IsSmartFinishShip) {
            ShotAgain:
                Coordinate enemyShot = FinishOffTheShip(EnemyHitShip, true);
                if (EnemyShot[enemyShot.X, enemyShot.Y] || MyMarkField[enemyShot.X, enemyShot.Y])
                    goto ShotAgain;
                EnemyShot[enemyShot.X, enemyShot.Y] = true;
                if (MyFlot[enemyShot.X, enemyShot.Y]) {
                    ShipKillArgs args = CheckShipKill(enemyShot, false, false);
                    if (args.Ship.Length > 0) {
                        EnemyHitShip = null;
                        SetMarkAroundShip(enemyShot, false);
                        OnKillShip?.Invoke(args);
                    }
                    else
                        OnHitShip?.Invoke(new ShipHitArgs(true, false, enemyShot));
                    OnEnemyShot?.Invoke(new EnemyShotArgs(enemyShot, true, false));
                    OnTurn?.Invoke(Turn.Enemy);
                }
                else {
                    IsEnemyTurn = false;
                    OnMissShotShip(new ShipMissShotArgs(true, false, enemyShot));
                    OnEnemyShot?.Invoke(new EnemyShotArgs(enemyShot, false, false));
                    OnTurn?.Invoke(Turn.My);
                }
                return enemyShot;
            }
            else
                while (CheckFreeCage(true)) {
                    int numY = r.Next(0, 10);
                    for (int i = numY; i < 10; i++) {
                        int numX = r.Next(0, 10);
                        int hint = r.Next(0, 8);
                        for (int j = numX; j < 10; j++) {
                            Coordinate enemyShot = new Coordinate(i, j);
                            if (!EnemyShot[i, j] && !MyMarkField[i, j]) {
                                if (!Settings.IsAI || Settings.IsAI && CheckAI(enemyShot)) {
                                    if (hint == 0 && !IsEnemyHintUsed && Settings.IsHint) {
                                        SetHintShot(enemyShot, true);
                                        IsEnemyHintUsed = true;
                                        IsEnemyTurn = false;
                                        OnTurn?.Invoke(Turn.My);
                                        return enemyShot;
                                    }
                                    EnemyShot[i, j] = true;
                                    if (MyFlot[i, j]) {
                                        IsEnemyTurn = true;
                                        ShipKillArgs args = CheckShipKill(enemyShot, false, false);
                                        if (args.Ship.Length > 0) {
                                            EnemyHitShip = null;
                                            SetMarkAroundShip(enemyShot, false);
                                            OnKillShip?.Invoke(args);
                                        }
                                        else {
                                            EnemyHitShip = new FinishOfTheShip(enemyShot, moveList);
                                            OnHitShip?.Invoke(new ShipHitArgs(true, false, enemyShot));
                                        }
                                        CheckGameOver();
                                        OnTurn?.Invoke(Turn.Enemy);
                                        return enemyShot;
                                    }
                                    else
                                        OnMissShotShip(new ShipMissShotArgs(true, false, enemyShot));
                                    IsEnemyTurn = false;
                                    OnTurn?.Invoke(Turn.My);
                                    return enemyShot;
                                }
                            }
                        }
                    }
                    r = new Random();
                }
            return null;
        }

        public bool CheckGameReady() => IsGameReady =
                Score.CountMyShips.Count1DeckShip == Settings.Default1DeckShip &&
                Score.CountMyShips.Count2DeckShip == Settings.Default2DeckShip &&
                Score.CountMyShips.Count3DeckShip == Settings.Default3DeckShip &&
                Score.CountMyShips.Count4DeckShip == Settings.Default4DeckShip;

        public bool CheckMyShip(int x, int y) => MyFlot[x, y];

        public bool CheckEnemyShip(int x, int y) => EnemyFlot[x, y];

        public void SetShips(bool IsEnemyShips) {
            bool[,] flot = IsEnemyShips ? EnemyFlot : MyFlot;
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (flot[i, j])
                        flot[i, j] = false;
            if (IsEnemyShips) {
                Score.CountEnemyShips.Reset();
                EnemyFlot = flot;
            }
            else {
                Score.CountMyShips.Reset();
                MyFlot = flot;
            }

            SetEnemyShip(4, Settings.Default4DeckShip, IsEnemyShips);
            SetEnemyShip(3, Settings.Default3DeckShip, IsEnemyShips);
            SetEnemyShip(2, Settings.Default2DeckShip, IsEnemyShips);
            SetEnemyShip(1, Settings.Default1DeckShip, IsEnemyShips);

            ClearMyMark();
            ClearEnemyMark();
        }

        public Coordinate[] GetHintArea(Coordinate ShipCoordinates, bool IsEnemyShot) {
            Coordinate[] ShipMarks = new Coordinate[9];
            GetAreaAroundPosition(ShipCoordinates).CopyTo(ShipMarks, 0);
            ShipMarks[ShipMarks.GetLengthWithoutNull()] = ShipCoordinates;
            for (int i = 0; i < ShipMarks.Length; i++) {
                if (ShipMarks[i] == null)
                    break;
                if (IsEnemyShot) {
                    if (EnemyShot[ShipMarks[i].X, ShipMarks[i].Y] || MyMarkField[ShipMarks[i].X, ShipMarks[i].Y])
                        ShipMarks[i] = null;
                }
                else {
                    if (MyShot[ShipMarks[i].X, ShipMarks[i].Y] || EnemyMarkField[ShipMarks[i].X, ShipMarks[i].Y])
                        ShipMarks[i] = null;
                }
            }
            return ShipMarks.ClearNull();
        }

        public bool CanSetMyShip(Coordinate coordinate) {
            bool cage1 = false;
            bool cage2 = false;
            bool cage3 = false;
            bool cage4 = false;

            int posX = coordinate.X;
            int posY = coordinate.Y;

            if (MyFlot[posX, posY])
                return true;

            if (posX + 1 < MyFlot.GetLength(0)) {
                if (posY + 1 < MyFlot.GetLength(1))
                    cage1 = MyFlot[posX + 1, posY + 1];
                if (posY - 1 >= 0)
                    cage2 = MyFlot[posX + 1, posY - 1];
            }

            if (posX - 1 >= 0) {
                if (posY + 1 < MyFlot.GetLength(1))
                    cage3 = MyFlot[posX - 1, posY + 1];
                if (posY - 1 >= 0)
                    cage4 = MyFlot[posX - 1, posY - 1];
            }

            return !(cage1 || cage2 || cage3 || cage4);
        }

        private Coordinate[] GetAreaAroundPosition(Coordinate ShipCoordinates) {
            Coordinate[] ShipMarks = new Coordinate[9];
            if (ShipCoordinates.X + 1 < 10)
                ShipMarks[0] = new Coordinate(ShipCoordinates.X + 1, ShipCoordinates.Y);
            if (ShipCoordinates.Y + 1 < 10)
                ShipMarks[1] = new Coordinate(ShipCoordinates.X, ShipCoordinates.Y + 1);
            if (ShipCoordinates.X - 1 >= 0)
                ShipMarks[2] = new Coordinate(ShipCoordinates.X - 1, ShipCoordinates.Y);
            if (ShipCoordinates.Y - 1 >= 0)
                ShipMarks[3] = new Coordinate(ShipCoordinates.X, ShipCoordinates.Y - 1);
            if (ShipCoordinates.X + 1 < 10) {
                if (ShipCoordinates.Y + 1 < 10)
                    ShipMarks[4] = new Coordinate(ShipCoordinates.X + 1, ShipCoordinates.Y + 1);
                if (ShipCoordinates.Y - 1 >= 0)
                    ShipMarks[5] = new Coordinate(ShipCoordinates.X + 1, ShipCoordinates.Y - 1);
            }
            if (ShipCoordinates.X - 1 >= 0) {
                if (ShipCoordinates.Y + 1 < 10)
                    ShipMarks[6] = new Coordinate(ShipCoordinates.X - 1, ShipCoordinates.Y + 1);
                if (ShipCoordinates.Y - 1 >= 0)
                    ShipMarks[7] = new Coordinate(ShipCoordinates.X - 1, ShipCoordinates.Y - 1);
            }
            return ShipMarks.ClearNull();
        }

        private bool CheckAI(Coordinate ShotCoordinate) {
            if (Score.CountMyShips.Count4DeckShip != 0)
                return CanShotAI(ShotCoordinate, 4);
            else if (Score.CountMyShips.Count3DeckShip != 0)
                return CanShotAI(ShotCoordinate, 3);
            else if (Score.CountMyShips.Count2DeckShip != 0)
                return CanShotAI(ShotCoordinate, 2);
            else
                return true;
        }

        private bool CheckFreeCage(bool IsMyFlot) {
            bool[,] mark = IsMyFlot ? MyMarkField : EnemyMarkField;
            bool[,] shot = IsMyFlot ? EnemyShot : MyShot;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (!(mark[i, j] || shot[i, j]))
                        return true;

            IsGameOver = true;
            return false;
        }

        private Coordinate FinishOffTheShip(FinishOfTheShip Ship, bool IsEnemyShot) {
            bool[,] flot = IsEnemyShot ? MyFlot : EnemyFlot;
            bool[,] shot = IsEnemyShot ? EnemyShot : MyShot;

            Coordinate Shot = Ship.GetRandomShot();

            if (flot[Shot.X, Shot.Y])
                Ship.AddCoordinate(Shot);
            else
                Ship.AddWrongShot(Shot);

            return Shot;
        }

        private ShipKillArgs CheckShipKill(Coordinate ShipCoordinate, bool IsMyShot, bool IsHintShot) {
            bool[,] flot = IsMyShot ? EnemyFlot : MyFlot;
            int shipLength = 0;
            int x = ShipCoordinate.X;
            int y = ShipCoordinate.Y;

            if (flot[x, y]) {
                Coordinate[] ShipCoordinates = GetShipCoordinates(ShipCoordinate, IsMyShot);
                if (ShipCoordinate == null)
                    return new ShipKillArgs(new Ship(0, IsMyShot, null), null, ShipCoordinate, !IsMyShot, IsHintShot);
                if (x + 1 < 10 && flot[x + 1, y] || x - 1 >= 0 && flot[x - 1, y])
                    shipLength = CheckShipKill(ShipCoordinates, false, IsMyShot) ? ShipCoordinates.GetLength(0) : 0;
                else if (y + 1 < 10 && flot[x, y + 1] || y - 1 >= 0 && flot[x, y - 1])
                    shipLength = CheckShipKill(ShipCoordinates, true, IsMyShot) ? ShipCoordinates.GetLength(0) : 0;
                else
                    shipLength = flot[x, y] ? 1 : 0;
            }
            if (shipLength != 0) {
                if (IsMyShot)
                    Score.CountEnemyShips.Remove(shipLength);
                else
                    Score.CountMyShips.Remove(shipLength);
                OnScore?.Invoke(Score);
                return new ShipKillArgs(new Ship(shipLength, IsMyShot, GetShipCoordinates(ShipCoordinate, IsMyShot)), GetMarkAroundShip(ShipCoordinate, IsMyShot), ShipCoordinate, !IsMyShot, IsHintShot);
            }
            return new ShipKillArgs(new Ship(0, IsMyShot, null), null, ShipCoordinate, !IsMyShot, IsHintShot);
        }

        private Coordinate[] GetShipCoordinates(Coordinate ShipCoordinate, bool IsEnemyShip) {
            bool[,] flot = IsEnemyShip ? EnemyFlot : MyFlot;
            int x = ShipCoordinate.X;
            int y = ShipCoordinate.Y;
            if (flot[x, y]) {
                Coordinate[] ShipCoordinates;
                if (x + 1 < 10 && flot[x + 1, y] || x - 1 >= 0 && flot[x - 1, y]) {
                    int minX = x;
                    int maxX = x;
                    while (minX - 1 >= 0 && flot[minX - 1, y])
                        minX--;
                    while (maxX + 1 < 10 && flot[maxX + 1, y])
                        maxX++;
                    ShipCoordinates = new Coordinate[maxX - minX + 1];
                    for (int i = 0; i < maxX - minX + 1; i++)
                        ShipCoordinates[i] = new Coordinate(minX + i, y);
                    return ShipCoordinates;
                }
                else if (y + 1 < 10 && flot[x, y + 1] || y - 1 >= 0 && flot[x, y - 1]) {
                    int minY = y;
                    int maxY = y;
                    while (minY - 1 >= 0 && flot[x, minY - 1])
                        minY--;
                    while (maxY + 1 < 10 && flot[x, maxY + 1])
                        maxY++;
                    ShipCoordinates = new Coordinate[maxY - minY + 1];
                    for (int i = 0; i < maxY - minY + 1; i++)
                        ShipCoordinates[i] = new Coordinate(x, minY + i);
                    return ShipCoordinates;
                }
                else
                    return new Coordinate[] { new Coordinate(x, y) };
            }
            else
                return null;
        }

        private bool CheckShipKill(Coordinate[] coordinates, bool IsHorisontal, bool IsEnemyShip) {
            for (int i = 0; i < coordinates.GetLength(0); i++)
                if (IsEnemyShip) {
                    if (!MyShot[coordinates[i].X, coordinates[i].Y])
                        return false;
                }
                else {
                    if (!EnemyShot[coordinates[i].X, coordinates[i].Y])
                        return false;
                }
            return true;
        }

        private void SeaWar_OnRemoveShip(Coordinate coor) => FindShips();

        private void SeaWar_OnSetShip(Coordinate coor) => FindShips();

        private void ClearEnemyMark() {
            for (int i = 0; i < EnemyMarkField.GetLength(0); i++)
                for (int j = 0; j < EnemyMarkField.GetLength(1); j++)
                    EnemyMarkField[i, j] = false;
        }

        private void ClearMyMark() {
            for (int i = 0; i < MyMarkField.GetLength(0); i++)
                for (int j = 0; j < MyMarkField.GetLength(1); j++)
                    MyMarkField[i, j] = false;
        }

        private void SetEnemyShip(int ShipLength, int Count, bool IsEnemyShips) {
            Random r = new Random();
            int shipCounter = 0;
            while (shipCounter < Count) {
                for (int j = 0; j < 4; j++) {
                    moveList.Shuffle();
                    int x = r.Next(0, 10);
                    int y = r.Next(0, 10);
                    Coordinate ShipCoordinate = new Coordinate(x, y);
                    Ship ship = CanSetShip(ShipCoordinate, ShipLength, moveList[0], IsEnemyShips, true);
                    if (ship != null) {
                        if (IsEnemyShips)
                            Score.CountEnemyShips.Add(ShipLength);
                        else
                            Score.CountMyShips.Add(ShipLength);

                        SetMarkAroundShip(ShipCoordinate, IsEnemyShips);
                        shipCounter++;
                        OnSetRandomShips?.Invoke(ship);
                        break;
                    }
                }
            }
        }

        private void SetMarkAroundShip(Coordinate ShipCoordinate, bool IsEnemyShip) {
            bool[,] flot = IsEnemyShip ? EnemyFlot : MyFlot;
            bool[,] mark = IsEnemyShip ? EnemyMarkField : MyMarkField;
            int x = ShipCoordinate.X;
            int y = ShipCoordinate.Y;

            Coordinate[] shipMark = GetMarkAroundShip(ShipCoordinate, IsEnemyShip);
            if (shipMark != null)
                for (int i = 0; i < shipMark.Length; i++)
                    mark[shipMark[i].X, shipMark[i].Y] = true;

            if (IsEnemyShip)
                EnemyMarkField = mark;
            else
                MyMarkField = mark;
        }

        private Coordinate[] GetMarkAroundShip(Coordinate ShipCoordinate, bool IsEnemyShip) {
            bool[,] flot = IsEnemyShip ? EnemyFlot : MyFlot;
            int x = ShipCoordinate.X;
            int y = ShipCoordinate.Y;

            if (flot[x, y]) {
                Coordinate[] ShipCoordinates = GetShipCoordinates(ShipCoordinate, IsEnemyShip);
                if (ShipCoordinate == null)
                    return null;
                int shipLength = ShipCoordinates.Length;
                Coordinate[] ShipMarks = new Coordinate[32];
                for (int i = 0; i < shipLength; i++) {
                    Coordinate[] cages = GetAreaAroundPosition(ShipCoordinates[i]);
                    for (int j = 0; j < cages.Length; j++)
                        if (!ShipMarks.Exist(cages[j]))
                            ShipMarks[ShipMarks.GetLengthWithoutNull()] = cages[j];

                    for (int j = 0; j < ShipCoordinates.Length; j++)
                        ShipMarks.Replace(ShipCoordinates[j], null);
                }
                return ShipMarks.ClearNull();
            }
            return null;
        }

        private void SetRandomEnemyShoot() {
            Random r = new Random();
            while (true) {
                int numY = r.Next(0, 10);
                for (int i = numY; i < 10; i++) {
                    int numX = r.Next(0, 10);
                    for (int j = numX; j < 10; j++) {
                        if (!EnemyShot[i, j]) {
                            EnemyShot[i, j] = true;
                            return;
                        }
                    }
                }
            }
        }

        private void FindShips() {
            Score.Reset();
            bool IsFindShips = false;
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    bool isShip = MyFlot[i, j];
                    if (isShip) {
                        IsFindShips = true;
                        int shipLength = 1;
                        for (int m = 0; m < 2; m++)
                            for (int k = 1; k < 10; k++) {
                                if (m == 0 && i + k < 10 ||
                                    m == 1 && j + k < 10) {
                                    if (MyFlot[m == 0 ? i + k : i, m == 1 ? j + k : j]) {
                                        shipLength++;
                                    }
                                    else
                                        break;
                                }
                            }
                        switch (shipLength) {
                            case 1:
                                Score.CountMyShips.Add(1);
                                break;
                            case 2:
                                Score.CountMyShips.Add(2);
                                Score.CountMyShips.Remove(1);
                                break;
                            case 3:
                                Score.CountMyShips.Add(3);
                                Score.CountMyShips.Remove(2);
                                break;
                            case 4:
                                Score.CountMyShips.Add(4);
                                Score.CountMyShips.Remove(3);
                                break;
                        }
                        OnScore(Score);
                    }
                }
            }
            if (!IsFindShips) {
                Score.CountMyShips.Reset();
                OnScore(Score);
            }
        }

        private void CheckGameOver() {
            IsGameOver =
               Score.CountMyShips.Count4DeckShip <= 0 &&
               Score.CountMyShips.Count3DeckShip <= 0 &&
               Score.CountMyShips.Count2DeckShip <= 0 &&
               Score.CountMyShips.Count1DeckShip <= 0 ||
               Score.CountEnemyShips.Count4DeckShip <= 0 &&
               Score.CountEnemyShips.Count3DeckShip <= 0 &&
               Score.CountEnemyShips.Count2DeckShip <= 0 &&
               Score.CountEnemyShips.Count1DeckShip <= 0;
        }

        private bool CanShotAI(Coordinate ShipCoordinate, int ShipLength) {
            int x = ShipCoordinate.X;
            int y = ShipCoordinate.Y;
            int HorisontalLength = 1;
            int VerticalLength = 1;

            for (int i = 1; i < 10; i++)
                if (x - i >= 0 && !EnemyShot[x - i, y] && !MyMarkField[x - i, y])
                    VerticalLength++;
                else
                    break;
            for (int i = 1; i < 10; i++)
                if (x + i < 10 && !EnemyShot[x + i, y] && !MyMarkField[x + i, y])
                    VerticalLength++;
                else
                    break;
            for (int i = 1; i < 10; i++)
                if (y - i >= 0 && !EnemyShot[x, y - i] && !MyMarkField[x, y - i])
                    HorisontalLength++;
                else
                    break;
            for (int i = 1; i < 10; i++)
                if (y + i < 10 && !EnemyShot[x, y + i] && !MyMarkField[x, y + i])
                    HorisontalLength++;
                else
                    break;

            return HorisontalLength >= ShipLength || VerticalLength >= ShipLength;
        }

        private Ship CanSetShip(Coordinate ShipCoordinate, int ShipLength, Derection Move, bool IsEnemyShip, bool IsSetShip = false) {
            bool CanSetShip = false;
            bool IsShipSet = false;
            int x = ShipCoordinate.X;
            int y = ShipCoordinate.Y;
            bool[,] mark = IsEnemyShip ? EnemyMarkField : MyMarkField;
            bool[,] flot = IsEnemyShip ? EnemyFlot : MyFlot;
            Ship ship = null;
            Coordinate[] shipCoordinates = new Coordinate[ShipLength];

        SetShip:
            for (int i = 0; i < ShipLength; i++) {
                if (Move == Derection.Up) {
                    if (x - i >= 0)
                        if (!flot[x - i, y] && !mark[x - i, y]) {
                            if (CanSetShip) {
                                shipCoordinates[i] = new Coordinate(x - i, y);
                                flot[x - i, y] = true;
                                IsShipSet = true;
                            }
                        }
                        else
                            return null;
                    else
                        return null;
                }
                else if (Move == Derection.Down) {
                    if (x + i < 10)
                        if (!flot[x + i, y] && !mark[x + i, y]) {
                            if (CanSetShip) {
                                shipCoordinates[i] = new Coordinate(x + i, y);
                                flot[x + i, y] = true;
                                IsShipSet = true;
                            }
                        }
                        else
                            return null;
                    else
                        return null;
                }
                else if (Move == Derection.Left) {
                    if (y - i >= 0)
                        if (!flot[x, y - i] && !mark[x, y - i]) {
                            if (CanSetShip) {
                                shipCoordinates[i] = new Coordinate(x, y - i);
                                flot[x, y - i] = true;
                                IsShipSet = true;
                            }
                        }
                        else
                            return null;
                    else
                        return null;
                }
                else {
                    if (y + i < 10)
                        if (!flot[x, y + i] && !mark[x, y + i]) {
                            if (CanSetShip) {
                                shipCoordinates[i] = new Coordinate(x, y + i);
                                flot[x, y + i] = true;
                                IsShipSet = true;
                            }
                        }
                        else
                            return null;
                    else
                        return null;
                }
            }
            CanSetShip = true;
            if (IsSetShip && !IsShipSet) {
                ship = new Ship(ShipLength, IsEnemyShip, shipCoordinates);
                goto SetShip;
            }
            if (IsEnemyShip) {
                EnemyMarkField = mark;
                EnemyFlot = flot;
            }
            else {
                MyMarkField = mark;
                MyFlot = flot;
            }

            return ship;
        }


        // EVENTS
        private void Game_OnMissShotShip(ShipMissShotArgs e) {
            if (Settings.IsHistory)
                History.AddMove(new Step(Shot.Miss, e.ShotCoordinate, e.IsEnemyShot, e.IsHintShot));
        }
        private void Game_OnKillShip(ShipKillArgs e) {
            if (Settings.IsHistory)
                History.AddMove(new Step(Shot.Kill, e.ShotCoordinate, e.IsEnemyShot, e.IsHintShot));
        }
        private void Game_OnHitShip(ShipHitArgs e) {
            if (Settings.IsHistory)
                History.AddMove(new Step(Shot.Hit, e.ShotCoordinate, e.IsEnemyShot, e.IsHintShot));
        }
        private async void Game_OnTurn(Turn turn) {
            if (turn == Turn.Enemy)
                await enemyShot();
        }
        private async Task enemyShot() {
            if (Settings.IsDelay)
                await SetEnemyShotAsync(350, 1100);
            else
                SetEnemyShot();
        }
        private void Game_OnHintShot(HintShotsArgs e) {
            if (Settings.IsHistory)
                History.AddMove(new Step(Shot.Hint, e.ShotCoordinate, e.IsEnemyShot, e.IsHintShot));
        }
    }
}
