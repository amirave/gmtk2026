using System;
using UnityEngine;

namespace Game
{
    public class ResourceProvider : MonoBehaviour
    {
        public Material PlainMaterial;
        public Material StripedMaterial;
        public Material DottedMaterial;
        
        public static ResourceProvider Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("Duplicate ResourceProvider!!");
                Destroy(gameObject);
            }
        }
    }
}