using System;
//using System.Collections.Generic;

namespace SeaBattle {
    static class Extension {

        public static void Shuffle<T>(this T[] array) {
            Random r = new Random();
            int n = array.Length;
            while (n > 1) {
                int k = r.Next(n);
                n--;
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static int GetLengthWithoutNull<T>(this T[] array) {
            int Counter = 0;
            while (Counter < array.Length && array[Counter] != null)
                Counter++;
            return Counter;
        }

        public static Coordinate[] ClearNull(this Coordinate[] array) {
            int counter = 0;
            for (int i = 0; i < array.Length; i++)
                if (array[i] != null)
                    counter++;
            Coordinate[] ClearedShipMarks = new Coordinate[counter];
            counter = 0;
            for (int i = 0; i < array.Length; i++)
                if (array[i] != null) {
                    ClearedShipMarks[counter] = array[i];
                    counter++;
                }
            return ClearedShipMarks;
        }

        public static bool Exist(this Coordinate[] array, Coordinate element) {
            if (element == null)
                return false;
            for (int i = 0; i < array.Length; i++) {
                if (array[i] == null)
                    continue;
                if (array[i] == element)
                    return true;
            }
            return false;
        }

        public static void Replace(this Coordinate[] array, Coordinate element, Coordinate replaceElement) {
            for (int i = 0; i < array.Length; i++) {
                if (array[i] == null)
                    return;
                if (array[i] == element) {
                    array[i] = replaceElement;
                    return;
                }
            }
        }

    }
}
