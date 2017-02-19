using System;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
//using System.Reflection;
//using System.Runtime.Remoting;

namespace DarkAlloc
{
    static class DarkAlloc
    {
        static IntPtr heap = Marshal.AllocHGlobal(1024 * 1024 * 8);
        static int nextFreeIndex = 0;

        /// Also known as UnsafeAccursedUnutterableDarkAlloc<T>
        /// Alloc<T>(...) is like new T(...) but on an entirely unmanaged, non-GC'd heap.
        /// This solution was inspired by the answer at < http://stackoverflow.com/a/13826828 >, but is modified and extended.

        public static unsafe T Alloc<T>(params dynamic[] args) where T : class
        {
            // Get type info for T
            Type type = typeof(T);
            IntPtr typeInfoPtr = type.TypeHandle.Value;

            // Reserve space
            int size = Marshal.ReadInt32(typeInfoPtr, 4);
            IntPtr ptr = heap + nextFreeIndex;
            nextFreeIndex += size;

            Console.WriteLine("Size of {0} = {1}", type.Name, size);

            // Zero the memory
            byte[] zeroes = new byte[size];
            Marshal.Copy(zeroes, 0, ptr, size);

            // Write type info pointer
            Marshal.WriteIntPtr(ptr + 4, typeInfoPtr);

            // Generate some MSIL to return
            DynamicMethod m = new DynamicMethod("_Construct", typeof(T), new[] { typeof(IntPtr) }, typeof(DarkAlloc), true);
            var il = m.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ret);

            var new_ = m.CreateDelegate(typeof(Func<IntPtr, T>)) as Func<IntPtr, T>;

            // Construct the object
            T obj = new_(ptr + 4);

            // Get the types of constructor arguments
            Type[] argTypes = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                // XXX: GetType may not work on all object types (such as unmanaged types), however casting to ObjectHandle does not work on primitive/value types such as int.
                argTypes[i] = args[i].GetType();
                //argTypes[i] = ((ObjectHandle)args[i]).Unwrap().GetType();
            }

            // Find a suitable constructor given our arguments
            var ctor = type.GetConstructor(argTypes);

            // Call the constructor
            if (ctor != null)
                ctor.Invoke(obj, args);

            return obj;
        }
    }
}