using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatImpl
{

    public class ConnectionGene : Gene, ICloneable
    {
        public int NodeIn { get;  private set; }
        public int NodeOut { get; private set; }
        public float Weight { get; set; }
        public bool IsEnabled { get; set; }
        //public int UniqueId { get; private set; }
        public int ReplaceIndex { get; private set; }

        public int InnovationNumber
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
        /// <summary>
        /// Use GenePool singleton to create instance of this class.
        /// </summary>
        /// <param name="nodeIn"></param>
        /// <param name="nodeOut"></param>
        /// <param name="weight"></param>
        /// <param name="isEnabled"></param>
        /// <param name="innovationNumber"></param>
        public ConnectionGene(int nodeIn,int nodeOut,float weight,bool isEnabled,int innovationNumber,int replaceIndex)
        {
            NodeIn = nodeIn;
            NodeOut = nodeOut;
            Weight = weight;
            IsEnabled = isEnabled;
            InnovationNumber = innovationNumber;
            ReplaceIndex = replaceIndex;

            //UniqueId = nodeIn * GenomeUtils.MAX_CONNECTION_NUMBER + NodeOut;

        }

        public void RandomizeWeight()
        {
            Weight = GenomeUtils.GetRandomWeight();
        }

        public void ShiftWeight(float shiftStrength)
        {
            Weight *= GenomeUtils.GetRandomNumber() * shiftStrength;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
