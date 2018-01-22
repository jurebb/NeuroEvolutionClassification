using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEvol
{
    public class Dataset
    {
        private string _filename;
        private double[][] _dataset;

        public Dataset(string filename)
        {
            _filename = filename;
            _dataset = new double[64][];
            ReadFromFile();
        }

        private void ReadFromFile()
        {
            int i = 0;
            try
            {
                using (StreamReader sr = File.OpenText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), _filename)))
                {
                    do
                    {
                        _dataset[i] = new double[5];

                        string[] redak = sr.ReadLine().Split('\t');
                        redak = redak.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        _dataset[i][0] = double.Parse(redak[0], CultureInfo.InvariantCulture);
                        _dataset[i][1] = double.Parse(redak[1], CultureInfo.InvariantCulture);
                        _dataset[i][2] = double.Parse(redak[2], CultureInfo.InvariantCulture);
                        _dataset[i][3] = double.Parse(redak[3], CultureInfo.InvariantCulture);
                        _dataset[i][4] = double.Parse(redak[4], CultureInfo.InvariantCulture);

                        i++;
                    } while (sr.Peek() != -1);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public double[] ExampleAtIndex(int index)
        {
            return _dataset[index];
        }

        public int DatasetLength
        {
            get
            {
                return _dataset.Length;
            }
        }
    }
}
