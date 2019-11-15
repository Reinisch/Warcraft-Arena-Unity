using System;

namespace ElleFramework.Utils
{
    public class ArrayUtils
    {
        public static void MoveArrayElementUp(Array array, int elementPosition) {
            if ((array != null)
                && (elementPosition > 0)
                && (elementPosition < array.Length)) {
                Array.Reverse(array, elementPosition - 1, 2);
            }
        }

        public static void MoveArrayElementDown(Array array, int elementPosition) {
            if ((array != null)
                && (elementPosition >= 0)
                && (elementPosition < array.Length - 1)) {
                Array.Reverse(array, elementPosition, 2);
            }
        }

        public static void MoveArrayElement(Array array, int elementPosition, int displacement) {
            while (displacement != 0) {
                if (displacement > 0) {
                    MoveArrayElementDown(array, elementPosition);
                    displacement--;
                    elementPosition++;
                } else {
                    MoveArrayElementUp(array, elementPosition);
                    displacement++;
                    elementPosition--;
                }
                MoveArrayElement(array, elementPosition, displacement);
            }
        }
    }
}
