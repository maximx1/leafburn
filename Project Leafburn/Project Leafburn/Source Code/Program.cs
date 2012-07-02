using System;

namespace Project_Leafburn
{
#if WINDOWS || XBOX
    class GlobalEntryClass
    {
        static void Main(string[] args)
        {
            using (LeafBurn1 game = new LeafBurn1())
            {
                game.Run();
            }
        }
    }
#endif
}

