using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatImpl
{


    class ANN
    {

        private class Connection 
        {
            public int innovationNumber { get; private set; }
            public bool isReady { get; private set; }

            public int inNode { get; private set; }
            public int outNode { get; private set; }

            private float weight;
            private float value;
            

            public Connection(ConnectionGene conn)
            {
                innovationNumber = conn.InnovationNumber;
                weight = conn.Weight;

                inNode = conn.NodeIn;
                outNode = conn.NodeOut;

                Reset();
            }
            /// <summary>
            /// Test isReady property first.
            /// </summary>
            /// <returns></returns>
            public float GetValue()
            {
                return value;
            }
            public void SetValue(float value)
            {
                this.value = value * weight;
                isReady = true;
            }
            public void Reset()
            {
                value = 0f;
                isReady = false;
            }
        }

        private class Node
        {
            public int id { get; private set; }
            public NodeGene.NODE_TYPE type { get; private set; }
            public float net { get; private set; }
            public bool isDone { get; private set; }

            private HashSet<Connection> connIn,connOut;

            private float sum;
            

            public Node(NodeGene node,HashSet<Connection> conns)
            {
                id = node.Id;
                type = node.NodeType;

                Reset();

                connIn = new HashSet<Connection>(conns.Where(x => x.outNode == id ));
                connOut = new HashSet<Connection>(conns.Where(x => x.inNode == id));

            }
            public void Reset()
            {
                isDone = false;
                sum = 0f;
            }
            public bool TryCollectAndTransmit()
            {
                Reset();

                bool status = true;
                foreach(var conn in connIn)
                {
                    if (conn.isReady)
                    {
                        Sum(conn.GetValue());
                    }
                    else status = false;
                }

                if (status)
                {
                    CalculateNet();
                    Transmit();
                }

                return isDone = status;
            }
            public void Transmit()
            {
                foreach(var conn in connOut)
                {
                    conn.SetValue(net);
                }
            }
            public void Sum(float value)
            {
                sum += value;
            }
            public float CalculateNet()
            {
                return net = ActivationFunction(sum);
            }
            private float ActivationFunction(float x)
            {
                //TODO
                return (float)Math.Tanh(x);
            }
            public void SetNet(float value)
            {
                isDone = true;
                net = value;
            }
            public void SetNetAndTransmit(float value)
            {
                SetNet(value);
                Transmit();
            }

        }


        private HashSet<Node> inputNodes, outputNodes, hiddenNodes;
        private HashSet<Connection> conns;
       

        public ANN(Genome genome)
        {
            Setup(genome);

        }
        private void Setup(Genome genome)
        {
            conns = new HashSet<Connection>(genome.ConnectionGeneSet.Where(x=>x.IsEnabled).Select(x => new Connection(x)).OrderBy(x => x.innovationNumber));


            inputNodes = new HashSet<Node>(genome.NodeGeneSet.Where(x => x.NodeType.Equals(NodeGene.NODE_TYPE.INPUT)).Select(x=> new Node(x,conns)).OrderBy(x=>x.id));

            hiddenNodes = new HashSet<Node>(genome.NodeGeneSet.Where(x => x.NodeType.Equals(NodeGene.NODE_TYPE.HIDDEN)).Select(x => new Node(x, conns)).OrderBy(x=>x.id));

            outputNodes = new HashSet<Node>(genome.NodeGeneSet.Where(x => x.NodeType.Equals(NodeGene.NODE_TYPE.OUTPUT)).Select(x => new Node(x, conns)).OrderBy(x => x.id));

        }
        public void Print_All()
        {
            Console.WriteLine("All Nodes:");

            foreach(var node in inputNodes)
            {
                Console.WriteLine($"INPUT:{node.id}");
            }
            foreach (var node in hiddenNodes)
            {
                Console.WriteLine($"HIDDEN:{node.id}");
            }
            foreach (var node in outputNodes)
            {
                Console.WriteLine($"OUTPUT:{node.id}");
            }

            foreach(var conn in conns)
            {
                Console.WriteLine($"{conn.isReady} {conn.innovationNumber} {conn.GetValue()}");
            }
        }
        public float[] FeedForward(float[] inputs)
        {
            if (inputs.Count() != inputNodes.Count())
                return null;


            var items = inputNodes.Zip(inputs, (x, y) => new { x, y });

            foreach (var item in items)
            {
                item.x.SetNetAndTransmit(item.y);
            }


            do
            {

                foreach (var item in hiddenNodes.SkipWhile(x => x.isDone))
                {
                    item.TryCollectAndTransmit();
                }

            } while (hiddenNodes.TakeWhile(x => !x.isDone).Count() > 0) ;

            foreach (var item in outputNodes)
            {
                item.TryCollectAndTransmit();
            }

            return outputNodes.Select(x => x.net).ToArray();


        }

        //public float[] Pass(float[] inputs)
        //{

        //}
    }
}
