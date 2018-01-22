using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEvol
{
    class KromosomDouble
    {
        double[] _poljeRjesenja;
        public double _dobrota;

        public KromosomDouble()
        {
            _poljeRjesenja = new double[GA._dimenzionalnost];
        }

        public double[] PoljeRjesenja
        {
            get
            {
                return _poljeRjesenja;
            }
        }

        public double Rjesenje(int index)
        {
            if (index >= 0 && index <= GA._dimenzionalnost)
            {
                return _poljeRjesenja[index];
            }
            else
            {
                Console.WriteLine("Index out of range");
                return -10;
            }
        }

        public void PostaviRjesenje(int index, double value)
        {
            if (index >= 0 && index <= GA._dimenzionalnost)
            {
                _poljeRjesenja[index] = value;
            }
            else
            {
                Console.WriteLine("Index out of range");
            }
        }
    }
}
