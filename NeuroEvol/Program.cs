using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEvol
{
    class Program
    {
        static void Main(string[] args)
        {
            Test7();
            Console.ReadKey();
        }

        private static void TestLong()
        {
            Dataset dataset = new Dataset("dataset.txt");
            int[] architecture = new int[] { 2, 8, 4, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);

            GA testEl2 = new GA(800000, 25, 0.20, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,
                0.04, 0.04, 0.5, 0.22, 0.8, "prvi", false);
            var param = testEl2.ZapocniAlgoritam();

            nn.InspectClassification(param, dataset);
            nn.WriteParamsToFile(param, "2843n1new.txt", "2843n2new.txt");
        }

        private static void Test8()
        {
            Dataset dataset = new Dataset("dataset.txt");
            int[] architecture = new int[] { 2, 6, 4, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);

            GA testEl2 = new GA(800000, 25, 0.20, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,     //najbolji do sad
                0.04, 0.04, 0.5, 0.22, 0.8, "prvi", false);
            var param = testEl2.ZapocniAlgoritam();

            nn.InspectClassification(param, dataset);
            nn.WriteParamsToFile(param, "2643n1new.txt", "2643n2new.txt");
        }

        private static void Test7()     //ovaj za 2 x 8 x 3
        {
            Dataset dataset = new Dataset("dataset.txt");
            int[] architecture = new int[] { 2, 8, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);

            GA testEl2 = new GA(50000, 25, 0.20, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,     //najbolji do sad
                0.04, 0.04, 0.5, 0.22, 0.8, "prvi", false);
            var param = testEl2.ZapocniAlgoritam();

            nn.InspectClassification(param, dataset);
            nn.WriteParamsToFile(param, "283n1new.txt", "283n2new.txt");
        }

        private static void Test5()     //ovaj koristi za 2 x 8 x 4 x 3
        {
            Dataset dataset = new Dataset("dataset.txt");
            int[] architecture = new int[] { 2, 8, 4, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);

            GA testEl2 = new GA(200000, 25, 0.20, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,     //najbolji do sad
                0.04, 0.04, 0.5, 0.22, 0.8, "prvi", false);
            var param = testEl2.ZapocniAlgoritam();

            nn.InspectClassification(param, dataset);
            nn.WriteParamsToFile(param, "2843n1b.txt", "2843n2b.txt");
        }

        private static void Test4()                 
        {
            Dataset dataset = new Dataset("dataset.txt");           
            int[] architecture = new int[] { 2, 8, 4, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);

            GA testEl2 = new GA(200000, 25, 0.20, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,
                0.04, 0.04, 0.8, 0.12, 0.3, "prvi", false);
            var param = testEl2.ZapocniAlgoritam();

            nn.InspectClassification(param, dataset);
        }
        
        private static void Test5Short()
        {
            Dataset dataset = new Dataset("dataset.txt");
            int[] architecture = new int[] { 2, 8, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);

            GA testEl2 = new GA(200000, 20, 0.2, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,
                0.04, 0.04, 0.8, 0.12, 0.3, "prvi", false);
            var param = testEl2.ZapocniAlgoritam();

            nn.InspectOutput(param, dataset);
        }

        private static void Test3()
        {
            Dataset dataset = new Dataset("dataset.txt");
            int[] architecture = new int[] { 2, 8, 4, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);
                                                                              
            GA testEl2 = new GA(50000, 35, 0.05, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,
                0.05, 0.05, 0.8, 0.12, 0.5, "test2", true);
            testEl2.ZapocniAlgoritam();
        }

        private static void TestParalel()
        {
            Dataset dataset = new Dataset("dataset.txt");

            int[] architecture = new int[] { 2, 8, 4, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);

            int[] architecture2 = new int[] { 2, 8, 4, 3 };
            NeuralNetwork nn2 = new NeuralNetwork(architecture2);

            int[] architecture3 = new int[] { 2, 8, 4, 3 };
            NeuralNetwork nn3 = new NeuralNetwork(architecture3);

            GA test1 = new GA(50000, 25, 0.1, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,
                0.05, 0.05, 0.8, 0.12, 0.5, "test1", true);

            GA test2 = new GA(50000, 25, 0.4, NeuralNetwork.CalculateNumOfParams(architecture2), 5, -5, nn2, dataset,
                0.05, 0.05, 0.8, 0.12, 0.5, "test2", true);

            GA test3 = new GA(50000, 15, 0.1, NeuralNetwork.CalculateNumOfParams(architecture3), 5, -5, nn3, dataset,
                0.05, 0.05, 0.8, 0.12, 0.5, "test3", true);

            Parallel.Invoke(
                () => test1.ZapocniAlgoritam(),
                () => test2.ZapocniAlgoritam(),
                () => test3.ZapocniAlgoritam());
        }

        private static void Test2()
        {
            Dataset dataset = new Dataset("dataset.txt");
            int[] architecture = new int[] { 2, 8, 4, 3 };
            NeuralNetwork nn = new NeuralNetwork(architecture);

            //GA testEl2 = new GA(50000, 60, 0.1, NeuralNetwork.CalculateNumOfParams(architecture), 3, -3, nn, dataset, 
            //    0.05, 0.05, 0.6, 1, 1, "sample string");                                                                                   //first try
            GA testEl2 = new GA(50000, 20, 0.1, NeuralNetwork.CalculateNumOfParams(architecture), 5, -5, nn, dataset,
                0.05, 0.05, 0.8, 0.12, 0.5, "prvi", true);
            testEl2.ZapocniAlgoritam();
        }

        private static void Test1()
        {
            Dataset dataset = new Dataset("dataset.txt");
            NeuralNetwork nn = new NeuralNetwork(new int[] { 2, 8, 3 });
            double[] sampleParams = new double[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1};

            var outp = nn.CalculateOutput(sampleParams, dataset.ExampleAtIndex(0));

            Console.WriteLine("mse: {0}", nn.CalculateError(sampleParams, dataset));
        }

        private static void Test1p()
        {
            Dataset dataset = new Dataset("dataset.txt");
            NeuralNetwork nn = new NeuralNetwork(new int[] { 2, 8, 4, 3 });
            double[] sampleParams = new double[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                            1, 1, 1};

            var outp = nn.CalculateOutput(sampleParams, dataset.ExampleAtIndex(0));

            Console.WriteLine("mse: {0}", nn.CalculateError(sampleParams, dataset));
        }
    }
}
