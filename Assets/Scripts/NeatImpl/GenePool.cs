using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatImpl
{
    public sealed class GenePool
    {
        private static GenePool instance;
        private Random rand;

        //VARS
        private HashSet<ConnectionGene> connGeneList;
        private List<NodeGene> nodeGeneList;
        private List<int> innovationNumberList;

        private int innovationNumber;

        private int genomeIdentifier = 0;


        public GenePool(Random rand)
        {
            this.rand = rand;

            connGeneList = new HashSet<ConnectionGene>();
            nodeGeneList = new List<NodeGene>();
            innovationNumberList = new List<int>();

            innovationNumber = 1;
        }

        public int GetNextGenomeIdentifier()
        {
            return genomeIdentifier++;
        }

        //public List<NodeGene> SortedNodeGene
        //{
        //    get
        //    {
        //        return nodeGeneList;
        //    }
        //}

        public void UnityDebugInfo()
        {
            int _maxNodeInnovation = nodeGeneList.Max(x => x.Id);
            int _maxConnInnovation = connGeneList.Max(x => x.InnovationNumber);

            UnityEngine.Debug.Log($" node:{_maxNodeInnovation} conn:{_maxConnInnovation}");
        }
        
        //public static GenePool Instance()
        //{
        //    return instance ?? (instance = new GenePool(new Random()));
        //}
        /// <summary>
        /// Returns new NodeGene of type HIDDEN.
        /// </summary>
        /// <returns></returns>
        private NodeGene NodeGene_Next()
        {
            return NodeGene_Add(nodeGeneList.Count + 1, NodeGene.NODE_TYPE.HIDDEN);
        }
        /// <summary>
        /// Returns NodeGene or if not exists create new one.
        /// </summary>
        /// <param name="nodeId">Id number of a node.</param>
        /// <param name="nodeId">Type of node.</param>
        /// <returns></returns>
        public NodeGene NodeGene_GetOrCreate(int nodeId,NodeGene.NODE_TYPE type)
        {
            return nodeGeneList.Where(x => x.Id == nodeId).FirstOrDefault() ?? NodeGene_Add(nodeId,type);
        }

        public NodeGene NodeGene_Get(int nodeId)
        {
            return nodeGeneList.Where(x => x.Id == nodeId).FirstOrDefault();
        }

        //public NodeGene NodeGene_AddOnConnection(ConnectionGene conn)
        //{
        //    //var nodeA 

        //}

        /// <summary>
        /// Create and add new node gene to list.
        /// </summary>
        /// <param name="nodeId">Id number of node.</param>
        /// <param name="nodeId">Type of node.</param>
        /// <returns>New node gene.</returns>
        private NodeGene NodeGene_Add(int nodeId, NodeGene.NODE_TYPE type)
        {
            NodeGene node = new NodeGene(nodeId, type);

            nodeGeneList.Add(node);

            return node;
        }
        /// <summary>
        /// Returns ConnectionGene by innovation number.
        /// </summary>
        /// <param name="innovationNumber">Innovation number of specific ConnectionGene</param>
        /// <returns></returns>
        //public ConnectionGene ConnectionGene_Get(int innovationNumber)
        //{
        //    return connGeneList.Where(x => x.InnovationNumber == innovationNumber).FirstOrDefault();
        //}
        /// <summary>
        /// Returns instance of ConnectionGene class or if not exists create new one.
        /// </summary>
        /// <param name="nIn">Input node id</param>
        /// <param name="nOut">Output node id</param>
        /// <returns></returns>
        public ConnectionGene ConnectionGene_GetOrCreate(int nIn,int nOut)
        {
            return ( connGeneList.Where(x => x.NodeIn == nIn && x.NodeOut == nOut).FirstOrDefault() ?? ConnectionGene_Add(nIn, nOut) ).Clone() as ConnectionGene;
        }
        /// <summary>
        /// Creates new ConnectionGene.
        /// </summary>
        /// <param name="nIn">Input node id</param>
        /// <param name="nOut">Output node id</param>
        /// <returns></returns>
        public ConnectionGene ConnectionGene_Add(int nIn,int nOut)
        {
            //int inv = ((connGeneList.Count > 0) ? connGeneList.Max(x => x.InnovationNumber) : 0) + 1;

            innovationNumberList.Add(innovationNumber++);

            int replaceIndex = NodeGene_Next().Id;

            ConnectionGene connGene = new ConnectionGene(nIn,
                                                        nOut,
                                                        GetRandomWeight(),
                                                        true,
                                                        innovationNumberList.Last(),
                                                        replaceIndex);
            connGeneList.Add(connGene);


            return connGene;
        }
        private float GetRandomWeight()
        {
            return GenomeUtils.GetRandomWeight();
        }
        /// <summary>
        /// Prints all ConnectionGenes.
        /// </summary>
        public void ConnectionGene_PrintAll()
        {
            foreach(ConnectionGene conn in connGeneList)
            {
                Console.WriteLine($"INV:{conn.InnovationNumber} IN:{conn.NodeIn} OUT:{conn.NodeOut} W:{conn.Weight} E:{conn.IsEnabled}");
            }
        }
    }
}
