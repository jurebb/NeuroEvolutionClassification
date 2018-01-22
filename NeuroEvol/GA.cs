using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEvol
{
    class GA
    {
        private int _velicinaPop;
        private double _vjvMut;
        private KromosomDouble _best;                         
        private KromosomDouble _oldBest;
        private KromosomDouble _worst;
        private KromosomDouble _worstPop;
        public static int _dimenzionalnost;
        int t = 0;
        private int _brojIteracija;
        private int _dg;
        private int _gg;
        private NeuralNetwork _nn;
        private Dataset _data;
        private double _vjvMutKomp1;
        private double _vjvMutKomp2;
        private double _vjvOdabiraMut1;
        private double _sigma1;
        private double _sigma2;
        private string _imeAlg;
        private Random _rand;
        private bool _verbose;

        public GA(int brojIteracija, int velicinaPopulacije, double vjerojatnostMutacije,
            int dimenzionalnostFunkcije, int gornjaGranica, int donjaGranica, NeuralNetwork nn, Dataset data, 
            double vjvMutKomp1, double vjvMutKomp2, double vjvOdabiraMut1, double sigma1, double sigma2, string imeAlgoritma, bool verbose)
        {
            _velicinaPop = velicinaPopulacije;
            _vjvMut = vjerojatnostMutacije;
            _brojIteracija = brojIteracija;
            _dimenzionalnost = dimenzionalnostFunkcije;
            _dg = donjaGranica;
            _gg = gornjaGranica;
            _nn = nn;
            _data = data;
            _vjvMutKomp1 = vjvMutKomp1;
            _vjvMutKomp2 = vjvMutKomp2;
            _vjvOdabiraMut1 = vjvOdabiraMut1;
            _sigma1 = sigma1;
            _sigma2 = sigma2;
            _imeAlg = imeAlgoritma;
            _rand = new Random();
            _verbose = verbose;
        }
        


        public KromosomDouble ZapocniAlgoritam()
        {
            t = 0;
            KromosomDouble[] P0 = GenerirajPocetnu(_velicinaPop);        //generiraj početnu populaciju potencijalnih rješenja P(0);
            List<KromosomDouble> tournament = new List<KromosomDouble>(3);
            KromosomDouble dijete;
            InitialWorstBest(P0);
            while (UvjetIspunjen() && t < _brojIteracija)
            {
                t = t + 1;
                NadjiBest(P0);                  
                tournament = SelektirajIz(P0);
                _worst = NadjiWorst(tournament);
                tournament.Remove(_worst);
                dijete = Krizanje(tournament);
                dijete = Mutacija(dijete);

                if(_nn.CalculateError(dijete.PoljeRjesenja, _data) < _nn.CalculateError(_best.PoljeRjesenja, _data) )
                {
                    _best = dijete;
                    _best._dobrota = dijete._dobrota;
                }
                else if(_nn.CalculateError(dijete.PoljeRjesenja, _data) > _nn.CalculateError(_worstPop.PoljeRjesenja, _data))
                {
                    _worstPop = dijete;
                    _worstPop._dobrota = dijete._dobrota;
                }

                dijete._dobrota = Dobrota(dijete); //samo evaluiraj novo dijete

                P0 = ZamijeniDijeteZaWorst(P0, dijete, _worst);
            }
            NadjiBest(P0);
            IspisiRjesenje(_best);

            return _best;
        }

        private bool UvjetIspunjen()
        {
            if (t == 0)
            {
                return true;
            }

            if (_nn.CalculateError(_best.PoljeRjesenja, _data) < Math.Pow(10, -6))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void IspisiRjesenje(KromosomDouble _best)
        {
            Console.WriteLine("============================================================");
            Console.WriteLine("\n>>>>>>> Algoritam {0} zaustavljen:", _imeAlg);
            Console.WriteLine("\tBroj iteracija: {0}", _brojIteracija);
            Console.WriteLine("\tGeneracija: {0}", (int)t / _velicinaPop);
            Console.Write("\tVrijednost funkcije: {0} \n\tParametri:", _nn.CalculateError(_best.PoljeRjesenja, _data));
            for (int i = 0; i < _dimenzionalnost; i++)
            {
                Console.Write(" {0}  ", _best.Rjesenje(i));
                if (i == _dimenzionalnost - 1)
                {
                    Console.Write("\n");
                }
            }
            Console.WriteLine("============================================================\n");
        }

        private KromosomDouble[] ZamijeniDijeteZaWorst(KromosomDouble[] p0, KromosomDouble dijete, KromosomDouble _worst)
        {
            for (int i = 0; i < _velicinaPop; i++)
            {
                if (p0[i] == _worst)
                {
                    p0[i] = dijete;
                    return p0;
                }
            }

            throw new Exception("ne radi kako bi trebalo");
        }

        private void InitialWorstBest(KromosomDouble[] p)
        {

            _best = p[0];
            _worstPop = p[1];
            for (int k = 0; k < _velicinaPop; k++)              //nadji best
            {
                if (_nn.CalculateError(p[k].PoljeRjesenja, _data) < _nn.CalculateError(_best.PoljeRjesenja, _data))
                {
                    _best = p[k];                               //najbolja jedinka
                }
                if (_nn.CalculateError(p[k].PoljeRjesenja, _data) > _nn.CalculateError(_worstPop.PoljeRjesenja, _data))
                {
                    _worstPop = p[k];                               //najbolja jedinka
                }
            }
            //sad kad znamo koji je best, koji worst, svima spremi dobrote
            for (int k = 0; k < _velicinaPop; k++)
            {
                p[k]._dobrota = Dobrota(p[k]);
            }
        }

        private void NadjiBest(KromosomDouble[] p)
        {
            for (int k = 0; k < _velicinaPop; k++)              //nadji best
            {
                if (p[k]._dobrota > _best._dobrota)
                {
                    _best = p[k];                               //najbolja jedinka
                    _best._dobrota = p[k]._dobrota;
                    continue;
                }
                if (p[k]._dobrota < _worstPop._dobrota)
                {
                    _worstPop = p[k];                           //najgora jedinka
                    _worstPop._dobrota = p[k]._dobrota;
                    continue;
                }
            }

            
            if (t > 1 && (_oldBest != _best))
            {
                _oldBest = _best;
                _oldBest._dobrota = _best._dobrota;

                
                if(_verbose)
                {
                    Console.WriteLine(">>>>>>> {0} >>> Nova najbolja jedinka:", _imeAlg);
                    Console.WriteLine("\tGeneracija: {0}", (int)t / _velicinaPop);
                    Console.WriteLine("\tIteracija: {0}", t);
                    Console.Write("\tVrijednost funkcije: {0} \n\tParametri:", _nn.CalculateError(_best.PoljeRjesenja, _data));
                    for (int i = 0; i < _dimenzionalnost; i++)
                    {
                        if (i % 12 == 0)
                        {
                            Console.Write("\n");
                        }
                        Console.Write("{0:0.00} \t", _best.Rjesenje(i));
                        if (i == _dimenzionalnost - 1)
                        {
                            Console.Write("\n");
                        }
                    }
                }

                else
                {
                    Console.Write(">>> {0}: Nova najbolja jedinka:", _imeAlg);
                    Console.Write("\tGeneracija: {0}", (int)t / _velicinaPop);
                    Console.WriteLine("\tVrijednost funkcije: {0}", _nn.CalculateError(_best.PoljeRjesenja, _data));
                }
                
            }
            else if (t <= 1)
            {
                _oldBest = _best;
            }

        }

        private KromosomDouble NadjiWorst(List<KromosomDouble> tr)
        {
            KromosomDouble worst = tr[0];
            worst._dobrota = tr[0]._dobrota;
            for (int k = 0; k < 3; k++)            
            {
                if (tr[k]._dobrota < worst._dobrota)
                {
                    worst = tr[k];                               
                    worst._dobrota = tr[k]._dobrota;
                }
            }

            return worst;
        }


        private KromosomDouble Krizanje(List<KromosomDouble> roditelji)
        {

            double vjv = GenerirajSelekcija(1, _rand);

            if (vjv <= 0.33)
            {
                return SimpleArithmeticRecombination(roditelji[0], roditelji[1]);
            }
            else if (vjv <= 0.66)
            {
                return HeuristicCrossover(roditelji[0], roditelji[1]);
            }
            else
            {
                return SimulatedBinaryCrossover(roditelji[0], roditelji[1]);
            }
        }

        private KromosomDouble SimulatedBinaryCrossover(KromosomDouble kromosom1, KromosomDouble kromosom2)
        {
            double alfa = GenerirajSelekcija(1, _rand);
            KromosomDouble dijete1 = new KromosomDouble();

            for (int i = 0; i < _dimenzionalnost; i++)
            {
                double val = alfa * kromosom1.Rjesenje(i) + (1 - alfa) * kromosom2.Rjesenje(i);
                dijete1.PostaviRjesenje(i, val);
                
            }

            return dijete1;
        }

        private KromosomDouble HeuristicCrossover(KromosomDouble kromosom1, KromosomDouble kromosom2)
        {
            double a = GenerirajSelekcija(1, _rand);
            KromosomDouble dijete1 = new KromosomDouble();

            KromosomDouble bolji, losiji;
            if (kromosom1._dobrota < kromosom2._dobrota)
            {
                bolji = kromosom2;
                losiji = kromosom1;
            }
            else
            {
                bolji = kromosom1;
                losiji = kromosom2;
            }

            for (int i = 0; i < _dimenzionalnost; i++)
            {
                double val = a * (bolji.Rjesenje(i) - losiji.Rjesenje(i)) + bolji.Rjesenje(i);
                if (Math.Abs(val) > 20)                                          // WARNING, heuristicni zna izletit
                {
                    val = GenerirajSelekcija(1, _rand);
                }
                dijete1.PostaviRjesenje(i, val);
            }

            return dijete1;
        }


        private KromosomDouble SimpleArithmeticRecombination(KromosomDouble kromosom1, KromosomDouble kromosom2)
        {
            int position = (int)GenerirajSelekcija(_dimenzionalnost, _rand);
            KromosomDouble dijete1 = new KromosomDouble();
            KromosomDouble dijete2 = new KromosomDouble();

            for (int i = 0; i < position; i++)
            {
                dijete1.PostaviRjesenje(i, kromosom1.Rjesenje(i));
                
            }
            for (int i = position; i < _dimenzionalnost; i++)
            {
                double val = (kromosom1.Rjesenje(i) + kromosom2.Rjesenje(i)) / 2;
                dijete1.PostaviRjesenje(i, val);
                
            }

            return dijete1;
        }


        private KromosomDouble[] Shuffle(KromosomDouble[] pst)
        {
            KromosomDouble[] Shuffled = pst.OrderBy(x => _rand.Next()).ToArray();
            return Shuffled;
        }

        private KromosomDouble Mutacija(KromosomDouble pt)
        {
            double vjv = GenerirajSelekcija(1, _rand);

            if (vjv <= _vjvMut)
            {
                vjv = GenerirajSelekcija(1, _rand);
                if(vjv <= _vjvOdabiraMut1)
                {
                    pt = Mutacija1(pt);
                }
                else
                {
                    pt = Mutacija2(pt);
                }
            }
            return pt;
        }

        private KromosomDouble Mutacija2(KromosomDouble kr)
        {
            for (int i = 0; i < _dimenzionalnost; i++)
            {
                double vjv = GenerirajSelekcija(1, _rand);
                if (vjv <= _vjvMutKomp2)
                {
                    var val = kr.Rjesenje(i) + Distribucije.Normalna(0, _sigma2);
                    kr.PostaviRjesenje(i, val);

                    
                }
            }

            return kr;
        }

        private KromosomDouble Mutacija1(KromosomDouble kr)
        {
            for (int i = 0; i < _dimenzionalnost; i++)
            {
                double vjv = GenerirajSelekcija(1, _rand);
                if(vjv <= _vjvMutKomp1)
                {
                    var val = Distribucije.Normalna(0, _sigma1);
                    kr.PostaviRjesenje(i, val);

                    
                }
            }

            return kr;
        }

        private List<KromosomDouble> SelektirajIz(KromosomDouble[] p0)
        {
            List<KromosomDouble> tournament = new List<KromosomDouble>(3);
            
            for (int i = 0; i < 3; i++)
            {
                int position = (int)GenerirajSelekcija(_velicinaPop, _rand);
                if (!tournament.Contains(p0[position]))
                {
                    tournament.Add(p0[position]);
                }
                else
                {
                    i--;
                }
            }

            return tournament;
        }


        private double Dobrota(KromosomDouble kromosom)
        {
            double rez = (_nn.CalculateError(kromosom.PoljeRjesenja, _data) - _nn.CalculateError(_worstPop.PoljeRjesenja, _data)) 
                / (_nn.CalculateError(_best.PoljeRjesenja, _data) - _nn.CalculateError(_worstPop.PoljeRjesenja, _data));              //TODO kazna(vk) = max{dobrota(vk)} - dobrota(vk)

            return rez;
        }

        private double GenerirajSelekcija(double D, Random random)
        {
            return random.NextDouble() * (D - 0) + 0;
        }

        private KromosomDouble[] GenerirajPocetnu(int _velicinaPop)
        {
            KromosomDouble[] P_0 = new KromosomDouble[_velicinaPop];
            for (int i = 0; i < _velicinaPop; i++)
            {
                P_0[i] = new KromosomDouble();
            }
            
            for (int i = 0; i < _velicinaPop; i++)
            {
                for (int j = 0; j < _dimenzionalnost; j++)
                {
                    P_0[i].PostaviRjesenje(j, _rand.NextDouble() * (_gg - (_dg)) + (_dg));
                }
            }
            return P_0;
        }
    }
}
