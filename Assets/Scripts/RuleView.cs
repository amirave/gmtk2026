using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class RuleView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private int _id;
        public int Id => _id;

        public void Populate(int id, Rule rule)
        {
            _id = id;
            switch (rule.property)
            {
                case ColorProperty cp:
                    PopulateColor(cp.ColorType);
                    break;
                case ShapeProperty sp:
                    PopulateShape(sp.shape);
                    break;
            }
        }

        private void PopulateColor(ColorType type)
        {
            var content = "Item has <size=150%><color=#{0}>{1}</color></size>";
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

            _text.text = content;
        }

        private void PopulateShape(ShapeType type)
        {
            var content = "Item is <size=150%>{0}</size>";
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
            _text.text = content;
        }
    }
}