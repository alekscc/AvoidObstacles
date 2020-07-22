using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeatImpl
{
    public class NodeGene : Gene
    {
        public enum NODE_TYPE
        {
            INPUT,
            HIDDEN,
            OUTPUT
        }

        public int Id
        {
            get
            {
                return innovationNumber;
            }
            set
            {
                innovationNumber = value;
            }
        }

        public NODE_TYPE NodeType { get; private set; }

        public float x { get; set; }
        public float y { get; set; }

        /// <summary>
        /// Use GenePool singleton to create instance of this class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public NodeGene(int id,NODE_TYPE type)
        {
            Id = id;
            NodeType = type;
        }
        


    }


}
