using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapontypes;

namespace Gunlist
{
    public class Weapons
        {
        public Gun rifle = new Gun(304f,250f,1f);
        public Gun lightmachinegun = new Gun(2f, 250f, 10f);
        public Gun TwincannonHE = new Gun(2f, 400f, 5f);
        public Gun grenadelauncher = new Gun(0f, 0f, 0.5f);
        public Meleeweapon blade = new Meleeweapon(0f,10f,2f);
        public Dropable explosivecap = new Dropable(7f,50f,2f,0.5f);
    }

}
