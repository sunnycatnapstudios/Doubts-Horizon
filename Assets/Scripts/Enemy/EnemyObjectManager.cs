using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class EnemyObjectManager : ScriptableObject
{

    [SerializeField] public Sprite OverworldSprite;
   public string Name;

        

        [SerializeField] private int damage;

        public int Damage {
            get { return damage; }
        }

        [SerializeField] private int health;

        public int Health {
            get { return health; }
        }


    public Sprite Sprite;
    [SerializeField] public RuntimeAnimatorController Animcontroller;
    [SerializeField] public RuntimeAnimatorController BattleAnimeController;

    public string GetName() {
        return name;
    }

    public int GetHealth() {
        return health;
    }

    public bool complexEnemy;

    public List<string> moveset;


}
