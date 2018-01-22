using System;

namespace NeuroEvol
{
    internal class Distribucije
    {
        public static double Normalna(double mi, double sigma)
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mi + Math.Sqrt(sigma) * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }
    }
}