using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatImpl
{
    public abstract class Gene : IComparable<Gene>
    {
        protected int innovationNumber { get; set; }
        //other 2 my 4
        public int CompareTo(Gene other)
        {
            //TODO
            return  innovationNumber - other.innovationNumber;
        }
    }
}
