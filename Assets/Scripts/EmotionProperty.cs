using System;

using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public enum EmotionType
    {
        None,
        Happy
    }
    
    [Serializable]
    public class EmotionProperty : IProperty
    {
        [SerializeField] public EmotionType emotion;

        public EmotionProperty(EmotionType emotion)
        {
            this.emotion = emotion;
        }
        
        public EmotionProperty()
        {
            emotion = EmotionType.None;
        }
        
        public bool Match(IProperty property)
        {
            return emotion == (property as EmotionProperty)?.emotion;
        }
        
        public override string ToString()
        {
            return $"Emotion: {emotion}";
        }

        public string Name()
        {
            return emotion.ToString();
        }
    }
}