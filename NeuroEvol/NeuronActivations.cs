using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEvol
{
    class NeuronActivations
    {
        
        internal static double Type1(double[] param, int indexOfParam, double[] input)
        {
            //Console.WriteLine("{0} {1} {2} {3}", indexOfParam, indexOfParam + 1, indexOfParam + 2, indexOfParam + 3);
            return 1 / (1 + (Math.Abs(input[0] - param[indexOfParam]) / Math.Abs(param[indexOfParam + 1])) + (Math.Abs(input[1] - param[indexOfParam + 2]) / Math.Abs(param[indexOfParam + 3])));
        }

        internal static double Type2(double[] param, int paramIndex, int paramNumPerSingleNeuronT2, double[] _neurons, int neuronIndex)
        {
            double net = 0;
            net += 1 * param[paramIndex];   // 1*w0
            //Console.WriteLine("{0}", paramIndex);
            for (int i = 1; i < paramNumPerSingleNeuronT2; i++)
            {
                //Console.WriteLine("{0}", paramIndex + i);
                //Console.WriteLine("{0}", neuronIndex + i - 1);

                net += _neurons[neuronIndex + i - 1] * param[paramIndex + i];
            }

            return 1 / (1 + Math.Pow(Math.E, -net));
        }
    }
}
