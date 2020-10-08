using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Beats
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    [RequireComponent(typeof(ContentSizeFitter))]
    [RequireComponent(typeof(RectTransform))]

    public class TrackView : MonoBehaviour
    {
        public enum Trigger { Missed, Right, Wrong}

//        [SerializeField] Track _track;

        [SerializeField] RectTransform _left;
        [SerializeField] RectTransform _right;
        [SerializeField] RectTransform _up;
        [SerializeField] RectTransform _down;

        [SerializeField] RectTransform _empty;

        RectTransform _rTransform;
        List<Image> _beatView;

        Vector2 _position;
        float _beatViewSize;
        float _spacing;

        public float position
        {
            get { return _position.y; }
            set
            {
                if(value != _position.y)
                {
                    _position.y = value;
                    _rTransform.anchoredPosition = _position;
                }
            }
        }

        // to control whenever it is initialize
        public void Init(Track track)
        {
            _rTransform = (RectTransform)transform;
            _position = _rTransform.anchoredPosition;

            _beatViewSize = _empty.rect.height;
            _spacing = GetComponent<VerticalLayoutGroup>().spacing;

            _beatView = new List<Image>();

            foreach(int b in track.beats)
            {
                GameObject g;
                switch (b)
                {
                    case 0:
                        g = _left.gameObject;
                        break;
                    case 1:
                        g = _up.gameObject;
                        
                        break;
                    case 2:
                        g = _down.gameObject;
                        break;
                    case 3:
                        g = _right.gameObject;
                        break;
                    default:
                        g = _empty.gameObject;
                        break;
                }

               Image view = GameObject.Instantiate(g, transform).transform.GetComponent<Image>();
                view.transform.SetAsFirstSibling();

                _beatView.Add(view);
            }
        }

        private void Start()
        {
            Init(GameplayController.Instance.track);
        }


        private void Update()
        {
            position -= (_beatViewSize + _spacing) * Time.deltaTime * GameplayController.Instance.beatsPerSeconds;
        }

        public void TriggerBeatView(int index, Trigger trigger)
        {
            switch (trigger)
            {
                case Trigger.Missed:
                    _beatView[index].color = Color.gray;
                    break;

                case Trigger.Right:
                    //_beatView[index].color = Color.green;
                    _beatView[index].gameObject.transform.localScale = Vector2.zero;
                    break;

                case Trigger.Wrong:
                    _beatView[index].color = Color.red;
                    break;
            }
        }

    }
}

