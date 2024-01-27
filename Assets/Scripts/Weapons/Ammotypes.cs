using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ammo;

namespace Ammotypes
{
    public class ammo
    {
        public Generalammo RifleDefault=new Generalammo(1,4,false,0,0,0);
        public Generalammo LMGDefault = new Generalammo(1f, 1f, false, 0, 0,0);
        public Generalammo LMGHE = new Generalammo(2f, 4, true, 1, 2,0);
        public Generalammo GLS = new Generalammo(0f, 0, true, 10, 10,0.5f);
    }
}
