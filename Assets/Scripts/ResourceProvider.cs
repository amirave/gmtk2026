using System;
using UnityEngine;

namespace Game
{
    public class ResourceProvider : MonoBehaviour
    {
        [Header("Patttern")]
        public Material PlainMaterial;
        public Material StripedMaterial;
        public Material DottedMaterial;

        [Header("Color")]
        public Color RedColor;
        public Color GreenColor;
        public Color BlueColor;

        [Header("Hat")]
        public Texture2D BrimlessYanky;
        public Texture2D Crown;
        [HideInInspector] public Texture2D Nothing;
        
        public static ResourceProvider Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Nothing = new Texture2D(0, 0);
            }
            else
            {
                Debug.LogError("Duplicate ResourceProvider!!");
                Destroy(gameObject);
            }
        }
    }
}