using System;

namespace SXF.Utils
{
    public static class ArrHelper
    {
        /// <summary>
        /// 对类型为T的数组进行扩展，把满足条件的元素移动到数组的最前面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">源数组</param>
        /// <param name="match">lamda表达式</param>
        /// <returns></returns>
        public static bool MoveToFront<T>(this T[] arr, Predicate<T> match)
        {
            //如果数组的长度为0
            if (arr.Length == 0)
            {
                return false;
            }
            //获取满足条件的数组元素的索引
            var index = Array.FindIndex(arr, match);
            //如果没有找到满足条件的数组元素
            if (index == -1)
            {
                return false;
            }
            //把满足条件的数组元素赋值给临时变量
            var temp = arr[index];
            Array.Copy(arr, 0, arr, 1, index);
            arr[0] = temp;
            return true;
        }
    }
}
