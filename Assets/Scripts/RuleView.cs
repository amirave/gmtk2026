using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Game
{
    public class RuleView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private TimelineAsset _animIn;

        private int _id;
        public int Id => _id;

        private const string LARGE_FORMAT =
            "<voffset=-0.15em><size=150%>{0}</size></voffset>";

        public void Populate(int id, Rule rule)
        {
            _id = id;
            var content = rule.mode == RuleMode.IsNot ? "Shape is not " : "Shape is ";

            content += GetPropertyString(rule.property);

            if (rule.mode == RuleMode.And)
            {
                content += " <br> AND is " + GetPropertyString(rule.secondProperty);
            }
            
            _text.text = content;
        }

        public string GetPropertyString(IProperty property)
        {
            switch (property)
            {
                case ColorProperty cp:
                    return PopulateColor(cp.ColorType);
                case ShapeProperty sp:
                    return PopulateShape(sp.shape);
                case EmotionProperty sp:
                    return PopulateEmotion(sp.emotion);
                case PatternProperty pp:
                    return PopulatePattern(pp.PatternType);
            }

            return "ERROR";
        }

        public async UniTask AnimateIn()
        {
            _director.Play(_animIn, DirectorWrapMode.Hold);
            await UniTask.WaitForSeconds((float)_director.duration);
        }

        private string PopulateColor(ColorType type)
        {
            var content = string.Format(LARGE_FORMAT, "<color=#{0}>{1}</color>");
            switch (type)
            {
                case ColorType.Red:
                    content = string.Format(content, ResourceProvider.Instance.RedColor.ToHexString(), "RED");
                    break;
                case ColorType.Green:
                    content = string.Format(content, ResourceProvider.Instance.GreenColor.ToHexString(), "GREEN");
                    break;
                case ColorType.Blue:
                    content = string.Format(content, ResourceProvider.Instance.BlueColor.ToHexString(), "BLUE");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return content;
        }

        private string PopulateShape(ShapeType type)
        {
            var content = LARGE_FORMAT;
            switch (type)
            {
                case ShapeType.Square:
                    content = string.Format(content, "SQUARE");
                    break;
                case ShapeType.Circle:
                    content = string.Format(content, "TRIANGLE");
                    break;
                case ShapeType.Triangle:
                    content = string.Format(content, "CIRCLE");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return content;
        }

        private string PopulateEmotion(EmotionType type)
        {
            var content = LARGE_FORMAT;
            switch (type)
            {
                case EmotionType.None:
                    content = string.Format(content, "Dead");
                    break;
                case EmotionType.Happy:
                    content = string.Format(content, "Whimsical");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return content;
        }

        private string PopulatePattern(PatternType type)
        {
            var content = LARGE_FORMAT;
            switch (type)
            {
                case PatternType.Plain:
                    content = string.Format(content, "PLAIN");
                    break;
                case PatternType.Striped:
                    content = string.Format(content, "STRIPED");
                    break;
                case PatternType.Dotted:
                    content = string.Format(content, "DOTTED");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return content;
        }
    }
}
