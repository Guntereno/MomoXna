using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core
{
    public class NamedObject
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private string mName = null;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public string Name
        {
            get { return mName; }
        }


        public NamedObject()
        {

        }


        public NamedObject(string name)
        {
            mName = name;
        }

    }
}
