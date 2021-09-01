using System;
using System.Collections.Generic;

namespace Test
{
    public interface ITripletService
    {
        List<string> GetTop10Triplets(string path);
    }
}
