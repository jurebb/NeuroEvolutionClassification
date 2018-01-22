using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEvol
{
    public class NeuralNetwork
    {
        private int[] _architecture;
        private int _numberOfLayers;
        private double[] _neurons;

        public NeuralNetwork(int[] architecture)
        {
            _architecture = architecture;
            _numberOfLayers = architecture.Count();

            _neurons = new double[CalculateNumOfNeurons(_architecture)];
        }

        public double[] CalculateOutput(double[] param, double[] input)
        {
            //prvi sloj
            _neurons[0] = input[0];     //x
            _neurons[1] = input[1];     //y

            int paramIndex = 0;
            int neuronIndex = 0;

            for (int i = 2; i < _architecture[1] + 2; i++)      //od 2 do architecture[1] + 2 u polju _neurons su neuroni tipa 1
            {
                //drugi sloj
                _neurons[i] = NeuronActivations.Type1(param, paramIndex, _neurons);  //param, indexOfParam, input
                paramIndex += 4;
                neuronIndex = i + 1;
            }

            int currentLayer = 2;
            int paramNumPerSingleNeuronT2;
            for (int j = currentLayer; j < _numberOfLayers; j++)        //j == trenutni sloj
            {
                //ostali slojevi, neuroni tipa 2
                paramNumPerSingleNeuronT2 = _architecture[j - 1] + 1;        // w-ovi + w0

                for (int i = 0; i < _architecture[j]; i++)
                {
                    _neurons[neuronIndex + i] = NeuronActivations.Type2(param, paramIndex, paramNumPerSingleNeuronT2, _neurons, 
                        neuronIndex - _architecture[j - 1]);
                    paramIndex += paramNumPerSingleNeuronT2;
                }
                neuronIndex += _architecture[j];
            }

            double[] output = new double[_architecture[_numberOfLayers - 1]];
            for (int i = 0; i < _architecture[_numberOfLayers - 1]; i++)
            {
                output[i] = _neurons[CalculateNumOfNeurons(_architecture) - _architecture[_numberOfLayers - 1] + i];
            }

            return output;
        }

        internal void WriteParamsToFile(KromosomDouble param, string filenameN1, string filenameN2)
        {
            int paramIndex = 0;

            using (FileStream fs = File.Open(filenameN1, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                for (int i = 2; i < _architecture[1] + 2; i++)      //od 2 do architecture[1] + 2 
                {
                    //             w1  s1  w2 s2 w1/s1 w2/s2
                    sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", param.PoljeRjesenja[paramIndex].ToString(CultureInfo.InvariantCulture), param.PoljeRjesenja[paramIndex + 1].ToString(CultureInfo.InvariantCulture),
                        param.PoljeRjesenja[paramIndex + 2].ToString(CultureInfo.InvariantCulture), param.PoljeRjesenja[paramIndex + 3].ToString(CultureInfo.InvariantCulture),
                        (param.PoljeRjesenja[paramIndex]/ param.PoljeRjesenja[paramIndex + 1]).ToString(CultureInfo.InvariantCulture),
                        (param.PoljeRjesenja[paramIndex + 2]/ param.PoljeRjesenja[paramIndex + 3]).ToString(CultureInfo.InvariantCulture));
                    paramIndex += 4;
                }
            }

            using (FileStream fs = File.Open(filenameN2, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                int currentLayer = 2;
                int paramNumPerSingleNeuronT2;
                for (int j = currentLayer; j < _numberOfLayers; j++)        //j == trenutni sloj
                {
                    //ostali slojevi, neuroni tipa 2
                    paramNumPerSingleNeuronT2 = _architecture[j - 1] + 1;        // w-ovi + w0

                    for (int i = 0; i < _architecture[j]; i++)
                    {
                        //_neurons[neuronIndex + i] = NeuronActivations.Type2(param, paramIndex, paramNumPerSingleNeuronT2, _neurons,
                        //    neuronIndex - _architecture[j - 1]);
                        for (int k = 0; k < paramNumPerSingleNeuronT2; k++)
                        {
                            sw.Write("{0}\t", param.PoljeRjesenja[paramIndex + k].ToString(CultureInfo.InvariantCulture));
                        }
                        sw.WriteLine();
                        paramIndex += paramNumPerSingleNeuronT2;
                    }
                }
            }
        }

        public double CalculateError(double[] param, Dataset data)
        {
            double sum = 0;
            for (int i = 0; i < data.DatasetLength; i++)
            {
                for (int j = 0; j < _architecture[_numberOfLayers - 1]; j++)
                {
                    double[] y = CalculateOutput(param, data.ExampleAtIndex(i));
                    sum += Math.Pow(data.ExampleAtIndex(i)[j + 2] - y[j], 2);
                }
            }
            return sum / data.DatasetLength;
        }

        internal void InspectClassification(KromosomDouble param, Dataset dataset)
        {
            Console.WriteLine("\nIspitivanje mreze:");
            int N = dataset.DatasetLength;
            int korektno = 0;
            int pogresno = 0;
            for (int broj = 0; broj < N; broj++)
            {
                var output = CalculateOutput(param.PoljeRjesenja, dataset.ExampleAtIndex(broj));

                Console.Write(">Uzorak {0} {1}:", dataset.ExampleAtIndex(broj)[0], dataset.ExampleAtIndex(broj)[1]);
                Console.Write("Izlaz: {0} {1} {2}: ", dataset.ExampleAtIndex(broj)[2], dataset.ExampleAtIndex(broj)[3], dataset.ExampleAtIndex(broj)[4]);

                for (int j = 0; j < 3; j++)
                {
                    if(output[j] > 0.5)
                    {
                        output[j] = 1;
                    }
                    else
                    {
                        output[j] = 0;
                    }
                }

                Console.Write("Izlaz mreze: {0} {1} {2}: ", output[0], output[1], output[2]);

                if (output[0] == dataset.ExampleAtIndex(broj)[2] && output[1] == dataset.ExampleAtIndex(broj)[3] && output[2] == dataset.ExampleAtIndex(broj)[4])
                {
                    korektno++;
                    Console.Write("Korektno klas.\n");
                }
                else
                {
                    pogresno++;
                    Console.Write("Pogresno klas.\n");
                }
            }
            Console.WriteLine(">> Broj korektno klasificiranih: {0}", korektno);
            Console.WriteLine(">> Broj pogresno klasificiranih: {0}", pogresno);
        }

        internal void InspectOutput(KromosomDouble param, Dataset dataset)
        {
            Console.WriteLine("\nIsprobavanje mreze:");

            while (true)
            {
                Console.WriteLine("Unesite x [0, 63]:");
                var broj = int.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                var output = CalculateOutput(param.PoljeRjesenja, dataset.ExampleAtIndex(broj));

                if (broj == -1)
                {
                    break;
                }

                Console.WriteLine("Izlaz mreze: {0} {1} {2}", output[0], output[1], output[2]);
                Console.WriteLine("Stvarni iznos: {0} {1} {2}", dataset.ExampleAtIndex(broj)[2], dataset.ExampleAtIndex(broj)[3], dataset.ExampleAtIndex(broj)[4]);
            }
        }

        #region PomocneMetode
        public static int CalculateNumOfParams(int[] architecture)
        {
            int num = 0;
            num += architecture[1] * 2 * 2;

            for (int i = 2; i < architecture.Length; i++)
            {
                num += (architecture[i - 1] + 1) * architecture[i];
            }

            return num;
        }

        public static int CalculateNumOfNeurons(int[] architecture)
        {
            int num = 0;
            for (int i = 0; i < architecture.Length; i++)
            {
                num += architecture[i];
            }

            return num;
        }
        #endregion
    }
}
