using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ammo
{
    public class Generalammo
    {
        public float damage;
        public float armorpen;
        public bool explosive;
        public float HEradius;
        public float splashdamage;
        public float timetoexplode;

        public Generalammo()
        {
        damage=1;
        armorpen=0;
        explosive=false;
        HEradius=0;
        splashdamage=0;
            timetoexplode = 0;
    }

        public Generalammo(float _damage, float _armorpen, bool _explosive ,float _HEradius,float _splashdamage,float _timetoexplode)
        {
            damage = _damage;
            armorpen = _armorpen;
            explosive = _explosive;
            HEradius = _HEradius;
            splashdamage = _splashdamage;
            timetoexplode = _timetoexplode;
        }
    }
}
