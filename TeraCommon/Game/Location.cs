﻿using System.Windows;

namespace Tera.Game
{
    public class Location
    {
        public uint World, Guard, Section;
        public Point Position;
        public Location(uint w, uint g, uint s, double x, double y)
        {
            World = w;
            Guard = g;
            Section = s;
            Position = new Point(x, y);
        }
        
    }
}
