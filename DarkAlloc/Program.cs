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
        static void Main(string[] args)
        {
            Test test = DarkAlloc.Alloc<Test>(5);
            Test test2 = DarkAlloc.Alloc<Test>(DarkAlloc.Alloc<Foo>() );

            // Trigger a GC to make sure it isn't collected
            System.GC.Collect();

            //Test test = new Test();
            test.Print();
            Console.WriteLine("x = {0}", test.x);

            Console.ReadKey(true);
        }
    }
}
