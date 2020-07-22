using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatImpl
{
    [Serializable]
    public class Species
    {
        //private const float MAX_DSITANCE = 4f;

        public float[] color { get; private set; }

        //public int Id { get { return representative.; } }
        public float Score { get; private set; }
        public List<Genome> Population { get; set; }

        private Genome representative;
    
       public Species()
        {

        }

        public Species(Genome representative)
        {
            Population = new List<Genome>();

            this.representative = representative;
            this.representative.SetSpecies(this);

            Population.Add(this.representative);

            //Random rand = new Random(new Random().Next());

            color = GenomeUtils.GetRandomColor3f();

        }
        
        public void SelectNewRepresentative(Random rand)
        {
            representative = null;

            if (Population.Count > 0)
            {
                representative = Population.OrderBy(x => x.Fitness).FirstOrDefault();//Population.OrderBy(x => rand!=null ? rand.Next() : x.Fitness ).FirstOrDefault();
            }
        }
        public bool Add(Genome genome,float maxDistance,float coef1,float coef2,float coef3)
        {
            if (genome.CalculateDistance(representative,coef1,coef2,coef3) < maxDistance)
            {
                //genome.SetSpecies(this);
                //Population.Add(genome);

                ForceAdd(genome);

                return true;
            }

            return false;
        }
        public void ForceAdd(Genome genome)
        {
            //UnityEngine.Debug.Log($"New genome added. +1 = {Population.Count} color:{color[0]}-{color[1]}-{color[2]}");

            if (genome.parentColor != null && color != null) {
                if (!genome.parentColor.SequenceEqual(this.color)) {
                    UnityEngine.Debug.Log("zmiana koloru");
                } 
            }

            genome.SetSpecies(this);
            Population.Add(genome);
        }
        public void Reset(Random rand = null)
        {

            SelectNewRepresentative(rand);

            foreach (Genome item in Population)
            {
                item.SetSpecies(null);
            }

            Population.Clear();


            if (representative!=null)
            {
                Population.Add(representative);
                representative.SetSpecies(this); 
            }

            Score = 0f;
        }
        public void Extinct()
        {
            representative.SetSpecies(null);

            representative = null;

            foreach (var item in Population)
                item.SetSpecies(null);

            Population.Clear();
        }
        public void Evaluate()
        {
            float sum = 0f;
            foreach(var item in Population)
            {
                sum += item.Fitness;
            }

            Score = sum / Population.Count;
        }
        public void Kill(float perc)
        {
            //HashSet<Genome> tempSet = new HashSet<Genome>(Population.Select(x => x));

            //UnityEngine.Debug.Log("showing all pop of spices");

            Population = Population.OrderBy(x => x.Fitness).ToList();

            //foreach(Genome g in Population)
            //{
            //    UnityEngine.Debug.Log($"{g.Fitness}");
            //}

            //if (Population.Count > 1)
            //{
                int _nToKill = (int)(perc * Population.Count);

                //UnityEngine.Debug.Log($"to kill:{_nToKill} perc:{perc} pop:{Population.Count}");

                while (_nToKill-- > 0)
                {
                    Genome g = Population.ElementAt(0);
                    Population.Remove(g);
                    g.SetSpecies(null);
                }

            //}
            //UnityEngine.Debug.Log("after kill");

            //foreach (Genome g in Population)
            //{
            //    UnityEngine.Debug.Log($"{g.Fitness}");
            //}

        }

        public Genome Breed(Random rand)
        {
            //Genome parentA = Population.OrderBy(x => rand.Next()).FirstOrDefault(),
            //         parentB = Population.OrderBy(x => rand.Next()).FirstOrDefault();

            //return parentA.Crossover(parentB, rand);

            Genome parentA, parentB;

            int i = 100;
            do
            {

                parentA = GetGenomeByTournament(.3f,rand);//GetGenomeByTournament(.1f,rand);//Population.OrderBy(x => rand.Next()).FirstOrDefault();
                parentB = GetGenomeByTournament(.3f,rand);

            } while (parentA.identifier == parentB.identifier && i-- > 0);


            if (i < 0)
            {
                if(Population.Count < 2)
                {
                    parentA = parentB = Population.FirstOrDefault();
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Quick breed");
                    Genome[] parents = Population.OrderByDescending(x => rand.Next()).Take(2).ToArray();
                    parentA = parents[0];
                    parentB = parents[1];
                }
            }


            Genome offspring = parentA.Crossover(parentB, rand);

            //float _parentsDist = parentA.CalculateDistance(parentB),
            //      _parentBDist = parentA.CalculateDistance(offspring),
            //       _parentADist = parentA.CalculateDistance(offspring);
            //UnityEngine.Debug.Log($"distance parents:{_parentsDist} parentA:{_parentADist} parentB:{_parentBDist}");

            return offspring;

        }
        //private Genome GetRandomGenome(Random rand)
        //{
        //    float _fitness = Population.Sum(x => x.Fitness);


        //}
        private Genome GetGenomeByTournament(float percGroupSize, Random rand)
        {

            int _nGroupSize = (int)(percGroupSize * Population.Count);


            if(_nGroupSize <= 1)
            {
                return Population.OrderBy(x=>rand.Next()).FirstOrDefault();
            }


            return Population.OrderBy(x => rand.Next()).Take(_nGroupSize).OrderByDescending(x => x.Fitness).FirstOrDefault();

        }

    }
}
