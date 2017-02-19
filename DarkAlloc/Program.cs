using System;
using DarkAlloc;

namespace DarkAlloc
{
    class Foo
    {
        public int value = 1337;
    }

    class Test
    {
        public int x = 42;

        public Test(int z)
        {
            Console.WriteLine("Hi! z = {0}", z);
        }

        public Test(Foo x)
        {
            Console.WriteLine("Constructing me with a Foo ({0})", x.value);
        }

        public void Print()
        {
            Console.WriteLine("Printing! BTW, x is " + this.x + "!");
        }
    }

    class Program
    {
        static void GcTest()
        {
            // Precondition: we're the first to allocate, and thus pinned at &DarkAlloc.heap[0]

            Console.WriteLine("Constructing object on heap");

            Test test = DarkAlloc.Alloc<Test>(5);
            test = null; // mark the reference up for GC

            Console.WriteLine("Triggering a garbage collection");

            // Trigger a GC
            System.GC.Collect();

            Console.WriteLine("Getting a new object reference");

            // Get a new object reference from the heap, pointing to the same object
            test = DarkAlloc.FromPtr<Test>(DarkAlloc.HeapStart);

            Console.WriteLine("Calling Print on it");

            // Do stuff with it
            test.Print();
        }

        static void Main(string[] args)
        {
            GcTest();
            Console.WriteLine("\n========\n");

            Test test = DarkAlloc.Alloc<Test>(5);
            Test test2 = DarkAlloc.Alloc<Test>(DarkAlloc.Alloc<Foo>() );

            test.Print();
            Console.WriteLine("x = {0}", test.x);

            Console.ReadKey(true);
        }
    }
}
