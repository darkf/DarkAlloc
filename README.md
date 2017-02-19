**DarkAlloc** is a **proof of concept** unmanaged, non-GC'd allocator for managed C# objects.

This means that you can use it to construct normal C# object references, but on a completely separate unmanaged heap -- not subject to .NET garbage collection.
This may be useful for applications that have high memory throughput and are subject to long GC pause times.

Before you consider using it, note that this is a **proof of concept** and should not be used in production code... or, like, ever.
It's unproven, it may break, and it relies on implementation details of the internal structure of .NET objects.

That being said, it should run the provided demo code.

The idea came to me when thinking about Unity games that suffer from lag spikes due to the stop-the-world GC interrupting the game and causing 26 ms of lag.
I thought one could simply bypass the GC allocator and write their own unmanaged heaps for dealing with and reusing objects. This is the eventual result of that thought process.

Note that it could be paired with a C# post-processor that converts `new T(...)` forms into `DarkAlloc<T>(...)` forms, so that all heap allocation is done via DarkAlloc.

Hey, never say anything's impossible.
