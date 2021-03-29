using System;

namespace SeaBattle {
    public class Moves {
        public event Action<Step> OnHistoryAdded;
        public event Action<Step> OnHistoryRemoved;
        public Step[] History { get; private set; }

        public Moves() => Clear();

        public void AddMove(Step Step) {
            if (GetLastStep() != null && GetLastStep().IsHintShot && Step.IsHintShot)
                return;
            Add(Step);
        }

        public void RemoveMove(Step Step) {
            Remove(Step);
            OnHistoryRemoved?.Invoke(Step);
        }

        public Step GetLastStep() {
            CheckLength();
            int length = History.GetLengthWithoutNull();
            if (length - 1 >= 0)
                return History[length - 1];
            else
                return null;
        }

        private void Add(Step Step) {
            CheckLength();
            History[History.GetLengthWithoutNull()] = Step;
            OnHistoryAdded?.Invoke(Step);
        }

        private void CheckLength() {
            int length = History.GetLengthWithoutNull();
            if (length + 1 >= History.Length) {
                Step[] newHistory = new Step[(int)(History.Length * 1.5)];
                History.CopyTo(newHistory, 0);
                History = new Step[newHistory.Length];
                newHistory.CopyTo(History, 0);
            }
        }

        private void Remove(Step Step) {
            for (int i = 0; i < History.Length; i++)
                if (History[i] == Step)
                    History[i] = null;
        }

        public Step[] GetHistory() {
            int historyLength = 0;
            Step[] steps = new Step[0];
            foreach (Step item in History)
                if (item != null)
                    historyLength++;
            if (historyLength > 0) {
                steps = new Step[historyLength];
                for (int i = 0; i < historyLength; i++) 
                    steps[i] = History[i];
            }
            return steps;
        }

        public void Clear() => History = new Step[2];
    }
}
