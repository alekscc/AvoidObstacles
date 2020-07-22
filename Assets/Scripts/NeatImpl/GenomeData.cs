using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatImpl
{
    [Serializable]
    public class NodeData
    {
        public int id { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public NodeGene.NODE_TYPE type { get; set; }


        public NodeData(NodeGene nodeGene)
        {
            id = nodeGene.Id;
            x = nodeGene.x;
            y = nodeGene.y;
            type = nodeGene.NodeType;
        }
        public NodeData()
        {

        }
    }

    [Serializable]
    public class ConnectionData
    {
        public int id { get; set; }
        public float weight { get; set; }
        public bool active { get; set; }
        public int nodeIn { get; set; }
        public int nodeOut { get; set; }

        public ConnectionData(ConnectionGene connGene)
        {
            id = connGene.InnovationNumber;
            weight = connGene.Weight;
            active = connGene.IsEnabled;
            nodeIn = connGene.NodeIn;
            nodeOut = connGene.NodeOut;
        }
        public ConnectionData()
        {

        }
    }

    [Serializable]
    public class GenomeData
    {
        public List<NodeData> nodes { get; set; }
        public List<ConnectionData> conns { get; set; }
        public float trainingFitness;
        public int inputsNumber { get; set; }
        public int outputsNumber { get; set; }

        public GenomeData(Genome genome)
        {
            nodes = genome.NodeGeneSet.Select(x => new NodeData(x)).ToList();
            conns = genome.ConnectionGeneSet.Select(x => new ConnectionData(x)).ToList();
            trainingFitness = genome.Fitness;

        }
        public GenomeData()
        {

        }
       
    }
}
