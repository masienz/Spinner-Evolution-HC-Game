using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class EndGame : MonoBehaviour
{
    [System.Serializable]
    public struct Beyblade
    {
        public GameObject beybladeObj;
        public GameObject growObj;
        public GameObject changedMatObj;
        public float factor;
        public float power;
    }

    public List<Beyblade> enemyBeyblades;

    public List<GameObject> endPathPoints;

    public List<ParticleSystem> endEffects;
}
