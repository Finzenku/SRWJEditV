using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SRWJEditV.Utilities
{
    public static class TFactory
    {
        public static object? CreateObject(Type T, object[] args)
        {
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i].GetType();
            }
            ConstructorInfo? c = T.GetConstructor(types);
            return c != null ? c.Invoke(args) : null;
        }
        public static T? CreateObject<T>(object[] args) => (T?)CreateObject(typeof(T), args);

        public static object? CreateObject(Type T, object arg)
        {
            ConstructorInfo? c = T.GetConstructor(new Type[] { arg.GetType() });
            return c != null ? c.Invoke(new object[] { arg }) : null;
        }
        public static T? CreateObject<T>(object arg) => (T?)CreateObject(typeof(T), arg);

        public static object? CreateObject(Type T) => Assembly.GetExecutingAssembly().CreateInstance(T.FullName!);
        public static T? CreateObject<T>() => (T?)CreateObject(typeof(T));

        public static IList? CreateList(Type myType) => (IList?)Activator.CreateInstance(typeof(List<>).MakeGenericType(myType));
    }
}
