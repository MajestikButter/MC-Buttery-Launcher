using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Buttery_Launcher
{    
    public class WindowSize
    {
        public double width;
        public double height;

        public WindowSize(double height, double width)
        {
            this.width = width;
            this.height = height;
        }

        public WindowSize Clone()
        {
            return new WindowSize(height, width);
        }
    }
}
