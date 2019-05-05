using System;
using System.Collections.Generic;

namespace Common
{
    public class Thing
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
    }

    public interface IDataService
    {
        List<Thing> GetThings();
    }

}
