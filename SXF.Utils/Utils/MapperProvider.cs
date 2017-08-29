using AutoMapper;
using System.Collections;
using System.Collections.Generic;
namespace SXF.Utils
{
    public static class MapperProvider
    {

        /// <summary>
        ///  类型映射
        /// </summary>
        public static T MapTo<T>(this object obj)
        {
            if (obj == null) return default(T);
            Mapper.CreateMap(obj.GetType(), typeof(T));
            return Mapper.Map<T>(obj);
        }

        public static IMappingExpression MapExpressionTo<T>(this object obj)
        {
            return obj == null ? null : Mapper.CreateMap(obj.GetType(), typeof(T));
        }

        public static IMappingExpression<T, T1> MapExpressionTo<T, T1>(this T obj)
        {
            return obj == null ? null : Mapper.CreateMap<T, T1>();
        }

        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> NoCreateMapToList<TDestination>(this IEnumerable source)
        {
            //IEnumerable<T> 类型需要创建元素的映射

            return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 类型映射
        /// </summary>
        public static TDestination NoCreateMapTo<TDestination>(this object obj)
            where TDestination : class
        {
            return obj == null ? default(TDestination) : Mapper.Map<TDestination>(obj);
        }

        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            if (source == null)
            {
                return new List<TDestination>();
            }
            foreach (var first in source)
            {
                var type = first.GetType();

                Mapper.CreateMap(type, typeof(TDestination));
                break;
            }
            return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            //IEnumerable<T> 类型需要创建元素的映射
            Mapper.CreateMap<TSource, TDestination>();
            return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 类型映射
        /// </summary>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;
            Mapper.CreateMap<TSource, TDestination>();
            return Mapper.Map(source, destination);
        }



    }
}
