namespace SeaBattle {
    public partial class Customization {
        public int Default4DeckShip { get; set; }
        public int Default3DeckShip { get; set; }
        public int Default2DeckShip { get; set; }
        public int Default1DeckShip { get; set; }

        public bool IsAI { get; set; }
        public bool IsSmartFinishShip { get; set; }
        public bool IsHint { get; set; }
        public bool IsHistory { get; set; }
        public bool IsDelay { get; set; }

        public Customization() : this(4, 3, 2, 1) { }

        public Customization(int default1DeckShip, int default2DeckShip, int default3DeckShip, int default4DeckShip) {
            Default1DeckShip = default1DeckShip;
            Default2DeckShip = default2DeckShip;
            Default3DeckShip = default3DeckShip;
            Default4DeckShip = default4DeckShip;

            IsAI = true;
            IsSmartFinishShip = true;
            IsHint = true;
            IsHistory = true;
            IsDelay = true;
        }

        public int GetTotalShips() => Default1DeckShip + Default2DeckShip + Default3DeckShip + Default4DeckShip;
    }
}
