namespace SeaBattle.Args {
    public class PlayerScoreArgs {
        public int Count1DeckShip { get; private set; }
        public int Count2DeckShip { get; private set; }
        public int Count3DeckShip { get; private set; }
        public int Count4DeckShip { get; private set; }

        public PlayerScoreArgs(int Count1DeckShip, int Count2DeckShip, int Count3DeckShip, int Count4DeckShip) {
            this.Count1DeckShip = Count1DeckShip;
            this.Count2DeckShip = Count2DeckShip;
            this.Count3DeckShip = Count3DeckShip;
            this.Count4DeckShip = Count4DeckShip;
        }

        public PlayerScoreArgs() : this(0, 0, 0, 0) { }

        public void Add(int ShipLength) {
            switch (ShipLength) {
                case 1:
                    Count1DeckShip++;
                    break;
                case 2:
                    Count2DeckShip++;
                    break;
                case 3:
                    Count3DeckShip++;
                    break;
                case 4:
                    Count4DeckShip++;
                    break;
                default:
                    break;
            }
        }

        public void Remove(int Length) {
            switch (Length) {
                case 1:
                    Count1DeckShip--;
                    break;
                case 2:
                    Count2DeckShip--;
                    break;
                case 3:
                    Count3DeckShip--;
                    break;
                case 4:
                    Count4DeckShip--;
                    break;
                default:
                    break;
            }
        }

        public int GetTotal() => Count1DeckShip + Count2DeckShip + Count3DeckShip + Count4DeckShip;

        public void Reset() {
            Count1DeckShip = 0;
            Count2DeckShip = 0;
            Count3DeckShip = 0;
            Count4DeckShip = 0;
        }

        public override string ToString() => $"{Count1DeckShip + Count2DeckShip + Count3DeckShip + Count4DeckShip}";
    }
}
