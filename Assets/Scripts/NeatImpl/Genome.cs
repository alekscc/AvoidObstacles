using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatImpl
{
    public class Genome : ICloneable,IComparable<Genome>
    {
        public float[] parentColor;

        public bool isElite = false;

        //private const float COEF1 = 1f,
        //                    COEF2 = 1f,
        //                    COEF3 = .4f;

        public int identifier { get; private set; }

        //debug purpose
        //public UnityEngine.Color color;

        //private const float MUTATE_NODE_CHANCE = 0.01f;
        //private const float MUTATE_LINK_CHANCE = 0.02f;
        //private const float MUTATE_WEIGHT_SHIFT_CHANCE = 0.02f;
        //private const float MUTATE_WEIGHT_RANDOMIZE_CHANCE = 0.02f;
        //private const float MUTATE_ENABLE_DISABLE_CHANCE = 0.02f;

        private const float WEIGHT_SHIFT_STRENGTH = .3f;
        //private SortedSet<int> invKeyList;
        private SortedSet<NodeGene> nodeGeneSet;
        private SortedSet<ConnectionGene> connGeneSet;

        //private Dictionary<int, ConnectionGene> connGeneMap;

        //private SortedSet<NodeGene> inputNodeSet,hiddenNodeSet, outputNodeSet;
        
        private const float mutationRate = 0.02f;

        public GenePool pool { get; private set; }


        public int InputNodeNumber { get; private set; }
        public int OutputNodeNumber { get; private set; }

        public float Fitness { get; private set; }

        public Species species { get; private set; }

        public void SetSpecies(Species species)
        {
            this.species = species;
        }

        public SortedSet<NodeGene> NodeGeneSet
        {
            get
            {
                return nodeGeneSet;
            }
        }
        public SortedSet<ConnectionGene> ConnectionGeneSet
        {
            get
            {
                return connGeneSet;
            }
        }
        private void Initialization(int nIn,int nOut,int id = -1)
        {
            nodeGeneSet = new SortedSet<NodeGene>();
            connGeneSet = new SortedSet<ConnectionGene>();

            InputNodeNumber = nIn;
            OutputNodeNumber = nOut;

            identifier = (id < 0) ? pool.GetNextGenomeIdentifier() : id;

        }

        public Genome(GenomeData gd)
        {
            nodeGeneSet = new SortedSet<NodeGene>(GetNodeGenes(gd.nodes));
            connGeneSet = new SortedSet<ConnectionGene>(GetConnectionGenes(gd.conns));

            Fitness = gd.trainingFitness;

            InputNodeNumber = nodeGeneSet.Count(x => x.NodeType == NodeGene.NODE_TYPE.INPUT);
            OutputNodeNumber = nodeGeneSet.Count(x => x.NodeType == NodeGene.NODE_TYPE.OUTPUT);

        }
        private IEnumerable<NodeGene> GetNodeGenes(IEnumerable<NodeData> nd)
        {
            foreach(var item in nd)
            {
                NodeGene node = new NodeGene(item.id, item.type);
                node.x = item.x;
                node.y = item.y;

                yield return node;
            }
        }
        private IEnumerable<ConnectionGene> GetConnectionGenes(IEnumerable<ConnectionData> cd)
        {
            foreach(var item in cd)
            {
                yield return new ConnectionGene(item.nodeIn,
                                                item.nodeOut,
                                                item.weight,
                                                item.active,
                                                item.id,
                                                item.id);
            }
        }

        //public Genome()
        //{

        //}

        /// <summary>
        /// Creates new genome and randomize it. Good for begginig population.
        /// </summary>
        /// <param name="nIn"></param>
        /// <param name="nOut"></param>
        public Genome(int nIn,int nOut,float connCoverage,Random rand,GenePool pool)
        {

            this.pool = pool;

            Initialization(nIn,nOut);

            Setup();

            RandomizeGenome(connCoverage,rand);

            
      
        }
        /// <summary>
        /// Creates new genome with only input and output nodes.
        /// </summary>
        /// <param name="nIn"></param>
        /// <param name="nOut"></param>
        public Genome(int nIn,int nOut,GenePool pool)
        {
            this.pool = pool;

            Initialization(nIn,nOut);

            Setup();
            
        }
        /// <summary>
        /// Creates identical copy of origin genome.
        /// </summary>
        /// <param name="g">Origin genome.</param>
        public Genome (Genome g)
        {

            //UnityEngine.Debug.Log($"node input :{g.InputNodeNumber} node output :{g.InputNodeNumber}");

            //identifier = g.identifier;

            pool = g.pool;

            Initialization(g.InputNodeNumber,g.OutputNodeNumber,g.identifier);

            species = g.species;

            //UnityEngine.Debug.Log($"before setup nodest:{NodeGeneSet.Count()} connset:{connGeneSet.Count()}");

            Setup();

            Fitness = g.Fitness;

            //UnityEngine.Debug.Log($"After setup and init nodest:{NodeGeneSet.Count()} connset:{connGeneSet.Count()}");


            foreach (NodeGene node in g.NodeGeneSet)
            {
                nodeGeneSet.Add(node);
            }

            foreach (ConnectionGene conn in g.ConnectionGeneSet)
            {
                connGeneSet.Add(conn.Clone() as ConnectionGene);
            }
        }
        public void Evaluate(float fitness)
        {
            Fitness = fitness;
        }
        public Genome Crossover(Genome genomeB, Random rand)
        {
            Genome genomeA, offspring = new Genome(InputNodeNumber, OutputNodeNumber,pool);

            if (genomeB.Fitness > Fitness)
            {
                genomeA = genomeB;
                genomeB = this;
            }
            else
            {
                genomeA = this;
            }


            //if (genomeB.ConnectionGeneSet.Max(x => x.InnovationNumber) > this.ConnectionGeneSet.Max(x => x.InnovationNumber))
            //{
            //    genomeA = genomeB;
            //    genomeB = this;
            //}
            //else
            //{
            //    genomeA = this;
            //}

            int iGenomeA = 0,
                iGenomeB = 0,
                countA = genomeA.ConnectionGeneSet.Count,
                countB = genomeB.ConnectionGeneSet.Count;

            while (iGenomeA < countA && iGenomeB < countB)
            {
                ConnectionGene connA = genomeA.ConnectionGeneSet.ElementAt(iGenomeA),
                               connB = genomeB.ConnectionGeneSet.ElementAt(iGenomeB);

                int innovA = connA.InnovationNumber,
                    innovB = connB.InnovationNumber;

                if (innovA == innovB)
                {

                    ConnectionGene newConn;
                    if (rand.NextDouble() < .5)
                    {
                        newConn = connA.Clone() as ConnectionGene;
                    }
                    else
                    {
                        newConn = connB.Clone() as ConnectionGene;
                    }
                    //ConnectionGene conn = connA.Clone() as ConnectionGene;
                    //conn.RandomizeWeight();

                    //offspring.ConnectionGeneSet.Add(conn);

                    //float min = Math.Min(connA.Weight, connB.Weight),
                    //    max = Math.Max(connA.Weight, connB.Weight);

                    //newConn.Weight = (float)((max - min) * rand.NextDouble()) + min;

                    //newConn.Weight = BLX(connA.Weight, connB.Weight, rand);

                    //if (rand.NextDouble() < .25)
                    //    newConn.IsEnabled = !newConn.IsEnabled;

                    offspring.ConnectionGeneSet.Add(newConn);



                    iGenomeA++;
                    iGenomeB++;
                }
                else if (innovA > innovB)
                {
                    // disjoint of geneB
                    iGenomeB++;
                }
                else
                {
                    //disjoint of geneA
                    offspring.ConnectionGeneSet.Add(connA.Clone() as ConnectionGene);
                    iGenomeA++;
                }
           

            }


            while(iGenomeA < countA)
            {
                ConnectionGene conn = genomeA.ConnectionGeneSet.ElementAt(iGenomeA).Clone() as ConnectionGene;
                offspring.ConnectionGeneSet.Add(conn);
                iGenomeA++;
            }

            foreach(ConnectionGene conn in genomeA.ConnectionGeneSet)
            {
                offspring.NodeGeneSet.Add(pool.NodeGene_Get(conn.NodeIn));
                offspring.NodeGeneSet.Add(pool.NodeGene_Get(conn.NodeOut));
            }

            offspring.parentColor = species?.color ?? new float[] { 1f, 0f, 0f, };

            return offspring;
        }
        //private float BLX(float x1,float x2,Random rand)
        //{
        //    float a = rand.Next(0, 100) * 0.01f,
        //            d = Math.Abs(x2 - x1),
        //            min = Math.Min(x1, x2),
        //            max = Math.Max(x1, x2);

        //    return Utils.Utilities.Lerp(min - a * d, max + a * d, (float)rand.NextDouble());
        //}
        public float CalculateDistance(Genome genomeB,float coef1,float coef2,float coef3)
        {
            Genome genomeA;


            if (genomeB.ConnectionGeneSet.Max(x => x.InnovationNumber) > this.ConnectionGeneSet.Max(x => x.InnovationNumber))
            {
                genomeA = genomeB;
                genomeB = this;
            }
            else
            {
                genomeA = this;
            }

            int nExcess = 0,
                nDisjoints = 0,
                nSimilar = 0,
                iGenomeA = 0,
                iGenomeB = 0;

            float weight_diff = 0f;

            int countA = genomeA.ConnectionGeneSet.Count(),
                countB = genomeB.ConnectionGeneSet.Count();

            while(iGenomeA < countA && iGenomeB < countB)
            {
                ConnectionGene connA = genomeA.ConnectionGeneSet.ElementAt(iGenomeA),
                               connB = genomeB.ConnectionGeneSet.ElementAt(iGenomeB);

                int innovA = connA.InnovationNumber,
                    innovB = connB.InnovationNumber;

                if(innovA == innovB)
                {
                    nSimilar++;
                    weight_diff += Math.Abs(connA.Weight - connB.Weight);
                    iGenomeA++;
                    iGenomeB++;
                }else if(innovA > innovB)
                {
                    // disjoint of geneA
                    nDisjoints++;
                    iGenomeB++;
                }else
                {
                    //disjoint of geneB
                    nDisjoints++;
                    iGenomeA++;
                }
            }
            // string innovGenomeA = "";



            // UnityEngine.Debug.Log("similarity = " + nSimilar);

            // UnityEngine.Debug.Log("pre weight_diff = " + weight_diff);
            weight_diff /= (nSimilar > 0) ? nSimilar : 1;
         //   UnityEngine.Debug.Log("post weight_diff = " + weight_diff);

            nExcess = countA - iGenomeA;

            float max = Math.Max(countA, countB);

            float n = (max < 20) ? 1f : max;


            float dist = coef1 * nExcess / n + coef2 * nDisjoints / n + coef3 * weight_diff;



            return dist;
                
        }
        private void Setup()
        {
            float scale_x = 1f, scale_y = 1f;
            float offset_y = scale_y / (InputNodeNumber+1);
            float pos_x = .1f, pos_y = offset_y;

            for (int i = 1; i <= InputNodeNumber; i++)
            {
                var node = pool.NodeGene_GetOrCreate(i, NodeGene.NODE_TYPE.INPUT);
                node.x = pos_x;
                node.y = pos_y;
                nodeGeneSet.Add(node);
                //inputNodeSet.Add(node);
                pos_y += offset_y;
            
               
            }

            pos_x = .9f;
            offset_y = scale_y / (OutputNodeNumber+1);
            pos_y = offset_y;
            for (int i = 1; i <= OutputNodeNumber; i++)
            {
                var node = pool.NodeGene_GetOrCreate(InputNodeNumber + i, NodeGene.NODE_TYPE.OUTPUT);
                node.x = pos_x;
                node.y = pos_y;
                nodeGeneSet.Add(node);
                //inputNodeSet.Add(node);
                pos_y += offset_y;

                //for (int j = 1; j <= nIn; j++)
                //{
                //    connGeneSet.Add(pool.ConnectionGene_GetOrCreate(j, i + nIn));

                //}
            }
            //var hidden = pool.NodeGene_AddNext(NodeGene.NODE_TYPE.HIDDEN);
            //nodeGeneSet.Add(hidden);

            //connGeneSet.Add(pool.ConnectionGene_GetOrCreate(hidden.Id,nIn+1));
            //connGeneSet.Add(pool.ConnectionGene_GetOrCreate(1,hidden.Id));



        }

        private void RandomizeGenome(float connCoverage,Random r)
        {
            //GenePool.Instance().ConnectionGene_Add()


            //for(int i=0;i<OutputNodeNumber * InputNodeNumber; i++)
            //{
            //    Mutate_Link(r);
            //}


            if (connCoverage < 1f)
            {

                int nConnections = (int)(InputNodeNumber * OutputNodeNumber * connCoverage);

                for (int i = 0; i < nConnections /*r.Next(OutputNodeNumber, InputNodeNumber * OutputNodeNumber)*/ /*Math.Max(InputNodeNumber , OutputNodeNumber))*/; i++)
                    Mutate_Link(r); 
            }
            else
            {
                for (int i = 1; i <= OutputNodeNumber; i++)
                {

                    for (int j = 1; j <= InputNodeNumber; j++)
                    {
                        connGeneSet.Add(pool.ConnectionGene_GetOrCreate(j, i + InputNodeNumber));

                    }
                }
            }

            //if (r.NextDouble() < .5)
            //    Mutate_Node(r);

            foreach (ConnectionGene conn in connGeneSet)
            {
                conn.RandomizeWeight();

                //if (r.NextDouble() < .3)
                //    conn.IsEnabled = false;
            }

        }
     
        //public void ExecuteMutation(int n,Random rand)
        //{
        //    switch (n)
        //    {
        //        case 0:
        //            {
        //                Mutate_Link(rand);
        //            }break;
        //        case 1:
        //            {
        //                Mutate_Node(rand);
        //            }break;
        //        case 2:
        //            {
        //                Mutate_WeightShift(rand);
        //            }break;
        //        case 3:
        //            {
        //                Mutate_WeightRandomize(rand);
        //            }break;
        //        case 4:
        //            {
        //                Mutate_EnableDisable(rand);
        //            }break;

        //    }
        //}
        private void Mutate_Link(Random rand)
        {

            for(int i = 0; i < 100; i++)
            {
                NodeGene node1 = NodeGeneSet.Select(x => x).OrderBy(x => rand.Next()).FirstOrDefault(),
                         node2 = NodeGeneSet.Select(x => x).OrderBy(x => rand.Next()).FirstOrDefault();

                if (node1.Id != node2.Id && (node2.NodeType == NodeGene.NODE_TYPE.HIDDEN || node2.NodeType == NodeGene.NODE_TYPE.OUTPUT) )
                {
                    ConnectionGene conn;
                    if (node1.x < node2.x)
                    {
                        conn = pool.ConnectionGene_GetOrCreate(node1.Id, node2.Id);
                    }
                    else continue;


                    if (ConnectionGeneSet.Contains(conn))
                        continue;

                    conn.RandomizeWeight();
                    conn.IsEnabled = true;

                    ConnectionGeneSet.Add(conn);

                   

                    break;

                }
            }

        }
        private void Mutate_Node(Random rand)
        {

            //UnityEngine.Debug.Log("starting mutate node");
            if (ConnectionGeneSet.Count > 0)
            {
                ConnectionGene conn = ConnectionGeneSet.OrderBy(x => rand.Next()).FirstOrDefault();

                //for(int i=0;i<ConnectionGeneSet.Count;i++)
                //{

                    //ConnectionGene conn = ConnectionGeneSet.ElementAt(i);


                    NodeGene nodeFrom = NodeGeneSet.Where(x => x.Id.Equals(conn.NodeIn)).FirstOrDefault(),
                    nodeTo = NodeGeneSet.Where(x => x.Id.Equals(conn.NodeOut)).FirstOrDefault();

                    NodeGene node = pool.NodeGene_Get(conn.ReplaceIndex);


                    node.x = (nodeFrom.x + nodeTo.x) * .5f;
                    node.y = (nodeFrom.y + nodeTo.y) * (.4f + rand.Next(1, 20) * .01f);

                    node.y = (node.y >= 1f) ? 0.99f : (node.y <= 0f) ? 0.01f : node.y;

                    ConnectionGene connA = pool.ConnectionGene_GetOrCreate(conn.NodeIn, node.Id),
                                    connB = pool.ConnectionGene_GetOrCreate(node.Id, conn.NodeOut);

                    connA.RandomizeWeight();
                    connA.IsEnabled = true;

                    connB.IsEnabled = conn.IsEnabled;
                    connB.Weight = conn.Weight;

                    connGeneSet.Add(connA);
                    connGeneSet.Add(connB);

                    conn.IsEnabled = false;

                    nodeGeneSet.Add(node);

                //}


               // UnityEngine.Debug.Log("mutate node succesfull");

            }
        }
        private void Mutate_WeightShift(Random rand)
        {
            if (connGeneSet.Count > 0)
            {

                ConnectionGene conn = ConnectionGeneSet.Select(x => x).OrderBy(x => rand.Next()).FirstOrDefault();

                conn.ShiftWeight(WEIGHT_SHIFT_STRENGTH);

            }
        }
        private void Mutate_WeightRandomize(Random rand)
        {
            if (connGeneSet.Count > 0)
            {

                ConnectionGene conn = ConnectionGeneSet.Select(x => x).OrderBy(x => rand.Next()).FirstOrDefault();

                conn.RandomizeWeight();

            }

        }
        private void Mutate_EnableDisable(Random rand)
        {

            if (connGeneSet.Count > 0)
            {

                ConnectionGene conn = ConnectionGeneSet.Select(x => x).OrderBy(x => rand.Next()).FirstOrDefault();
                //if (conn == null)
                //{
                //    conn = ConnectionGeneSet.FirstOrDefault();
                //}


                conn.IsEnabled = !conn.IsEnabled;

            }


        }
        public void Mutate(float mutateLinkChance,float mutateNodeChance, float mutateWeightChance,float mutateWeightShiftChance,float mutateToggleEnableChance,Random r)
        {
            // disable\enabble connections mutate_enable_disable

            // ADD LINK mutate_link

            // ADD NODE mutate_node (creates two conns, actually one with random weight second have previous weight)

            // SHIFT WEIGHTS mutate_weight_shift (multiply wieght by number from 0 to 2)

            // RANDOM WEIGHT mutate_weight_random 

            if(r.NextDouble() < mutateLinkChance)
            {
                Mutate_Link(r);
            }

            if (r.NextDouble() < mutateNodeChance)
            {
                Mutate_Node(r);
            }

            if (r.NextDouble() < .8)
            {
                if (r.NextDouble() < mutateWeightChance)
                {
                    Mutate_WeightRandomize(r);
                }

                if (r.NextDouble() < mutateWeightShiftChance)
                {
                    Mutate_WeightShift(r);
                } 
            }

            if (r.NextDouble() < mutateToggleEnableChance)
            {
                Mutate_EnableDisable(r);
            }

        }


        public void Print()
        {

            Console.WriteLine("\nNodes:");

            foreach (NodeGene node in nodeGeneSet)
                Console.WriteLine($"{node.Id} : {node.NodeType}");

            Console.WriteLine("\nConnections:");

            foreach (ConnectionGene conn in connGeneSet)
                Console.WriteLine($"{conn.InnovationNumber} IN:{conn.NodeIn} OUT:{conn.NodeOut} W:{conn.Weight} | E:{conn.IsEnabled}");

            Console.WriteLine();
        }

        //public int CompareTo(Genome other)
        //{
        //    if (other.Fitness > Fitness)
        //        return -1;
        //    else if (other.Fitness < Fitness)
        //        return 1;
        //    else return 0;
        //}
        public object Clone()
        {
            return new Genome(this);
        }

        public int CompareTo(Genome other)
        {
            return other.identifier - identifier;
        }
    }
}
