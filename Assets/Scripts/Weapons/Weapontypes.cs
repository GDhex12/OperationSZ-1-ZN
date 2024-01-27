using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapontypes
{
    public class Generalweapon
    {
        public float damage;
        public float impactforce;
    }
    public class Gun : Generalweapon
    {
        public float range;
        public float firerate;

        public Gun ()
        {
            impactforce = 1;
            range = 1;
            firerate = 1;
        }

        public Gun(float _impactforce, float _range, float _firerate)
        {
            impactforce = _impactforce;
            range = _range;
            firerate = _firerate;
        }
    }

    public class Meleeweapon : Generalweapon
    {
        public float armorpen;
        public Meleeweapon ()
        {
            damage = 1f;
            impactforce = 1f;
            armorpen = 1f;
        }

        public Meleeweapon(float _impactforce, float _damage, float _armorpen)
        {
            damage = _damage;
            impactforce = _impactforce;
            armorpen = _armorpen;
        }
    }

    public class Dropable : Generalweapon
    {
        public float HEradius;
        public float splashdamage;
        public float timetoexplode;
        public float ejectforce;

        public Dropable()
        {
            HEradius = 1;
            splashdamage = 1;
            timetoexplode = 1;
            ejectforce = 1;
        }

        public Dropable(float _HEradius, float _splashdamage, float _timetoexplode, float _ejectforce)
        {
            HEradius = _HEradius;
            splashdamage = _splashdamage;
            timetoexplode = _timetoexplode;
            ejectforce = _ejectforce;
        }
    }
}
