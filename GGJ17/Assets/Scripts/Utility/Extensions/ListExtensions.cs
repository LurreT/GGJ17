using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Linq;

namespace ExtensionMethods {

    public static class ListExtension {

		public static int IndexOf<T>(this T[] list, T item) {
			for (int index = 0; index < list.Length; index++) {
				if ((item == null && list[index] == null) || (item != null && item.Equals(list[index]))) {
					return index;
				}
			}
			return -1;
		}

		public static int LastIndexOf<T>(this T[] list, T item) {
			for (int index = list.Length-1; index >= 0; index--) {
				if ((item == null && list[index] == null) || (item != null && item.Equals(list[index]))) {
					return index;
				}
			}
			return -1;
		}

		public static string ToFancyString<T>(this T[] list) {
            string s = "";
            for (int index = 0; index < list.Length; index++) {
                s += "[" + index + "]=" + (list[index] == null ? "null" : list[index].ToString()) + ";";
            }

            return list.ToString() + "{ " + s + " }";
        }

        public static T Get<T>(this T[] list, int index) where T : UnityEngine.Object {
            return list.Length > index && index >= 0 ? list[index] : null;
        }

        public static T Get<T>(this List<T> list, int index) where T : UnityEngine.Object {
            return list.Count > index && index >= 0 ? list[index] : null;
        }

        public static T GetRandom<T>(this T[] list) {
            return list[UnityEngine.Random.Range(0, list.Length)];
        }

        public static T GetRandom<T>(this List<T> list) {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static int GetRandomIndex<T>(this List<T> list) {
            if (list.Count == 0) return -1;
            return UnityEngine.Random.Range(0, list.Count);
        }

        public static int GetRandomIndex<T>(this List<T> list, System.Predicate<T> match) {
            List<int> valid = new List<int>();

            for (int i = 0; i < list.Count; i++) {
                if (match(list[i])) valid.Add(i);
            }

            if (valid.Count == 0) return -1;
            return valid[UnityEngine.Random.Range(0, valid.Count)];
        }

        public static T Pop<T>(this IList<T> list) {
            return list.Pop(list.Count - 1);
        }

        public static T Pop<T>(this IList<T> list, int index) {
            if (list.Count <= index || index < 0) throw new System.Exception("Invalid index");
            T value = list[index];
            list.RemoveAt(index);
            return value;
        }

		public static T Dequeue<T>(this IList<T> list) {
			return list.Pop(0);
		}

		public static T[] SubArray<T>(this T[] list, int index) {
			T[] result = new T[list.Length-index];
			Array.Copy(list, index, result, 0, list.Length-index);
			return result;
		}

		public static T[] SubArray<T>(this T[] list, int index, int length) {
			T[] result = new T[length];
			Array.Copy(list, index, result, 0, length);
			return result;
		}

		// http://stackoverflow.com/questions/273313/randomize-a-listt
		public static void Shuffle<T>(this List<T> list) {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1) {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}