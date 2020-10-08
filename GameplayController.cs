using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beats
{
    [RequireComponent(typeof(AudioSource))]
    public class GameplayController : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] KeyCode _left;
        [SerializeField] KeyCode _right;
        [SerializeField] KeyCode _up;
        [SerializeField] KeyCode _down;

        [Header("Track")]
        [Tooltip("Beat Track to Play")]
        [SerializeField] private Track _track;       // reference to the Track
        /// <summary>
        /// The current track.
        /// </summary>
        public Track track { get{ return _track; } }
        public float beatsPerSeconds { get; private set; }
        public float secondsPerBeat { get; private set; }

        [Header("Setting")]
        [Tooltip("Setting for Player Data")]
        public FloatVariable score ;
        public FloatVariable highScore;
        public FloatVariable combo;
        public FloatVariable highestCombo;
        public FloatVariable scoreMultiplier;
        public StringVariable hit;

        [Header("UI")]
        public GameObject comboUI;
        public GameObject hitUI;

        [Header("Particle")]
        public GameObject particle;

        private bool _played;
        private bool completed;
        AudioSource _audioSource;
        TrackView _trackView;

        private WaitForSeconds waitAndStop;

        private static GameplayController _instance;
        /// <summary>
        /// Reference to a gameplay Controller
        /// whenever we access to the instance,
        /// then it will go check the instace exist in the scene.
        /// </summary>
        public static GameplayController Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = (GameplayController)GameObject.FindObjectOfType(typeof(GameplayController));
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        #region MonoBehavior Methods

        private void Awake()
        {
            _instance = this;
            beatsPerSeconds = track.bpm / 60f;       //beatsPerSeconds
            secondsPerBeat = 60f / track.bpm;       //secondsPerBeat
            waitAndStop = new WaitForSeconds(secondsPerBeat * 2);

            _trackView = FindObjectOfType<TrackView>();
            if (!_trackView)
                Debug.LogWarning("No TrackView Found");
        }

        // Start is called before the first frame update
        void Start()
        {
            // Reset the score
            score.value = 0;
            hit.value = "";

            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = track._audioClip;
            _audioSource.Play();

            InvokeRepeating("NextBeat", 0f, secondsPerBeat);
        }

        // Update is called once per frame
        void Update()
        {
            if (_played || completed)
                return;

            if (Input.GetKeyDown(_left))
            {
                PlayBeat(0);
            }
            if (Input.GetKeyDown(_up))
            {
                PlayBeat(1);
            }
            if (Input.GetKeyDown(_down))
            {
                PlayBeat(2);
            }
            if (Input.GetKeyDown(_right))
            {
                PlayBeat(3);
            }
        }
        #endregion

        #region Gameplay
        private int _current;
        public int current
        {
            get { return _current; }
            set
            {
                if(value != _current)
                {
                    _current = value;

                    if(_current == _track.beats.Count)
                    {
                        CancelInvoke("NextBeat");

                        completed = true;

                        StartCoroutine(WaitAndStop());
                    }
                }
            }
        }

        void PlayBeat(int input)
        {
            _played = true;

            if(_track.beats[current] == -1)
            {
            //    Debug.Log(string.Format("{0} Bad", input));
            }
            else if(_track.beats[current] == input)
            {
            //    Debug.Log(string.Format("{0} Cool", input));
                _trackView.TriggerBeatView(current, TrackView.Trigger.Right);

                //  Increase combo and check whether to increase the Score Multiplier;  
                combo.value += 1;
                ComboIncreaseScoreMultipier();
                score.value += 80 * scoreMultiplier.value;

                hitUI.GetComponent<Text>().color = Color.cyan;
                hit.value = "Perfect";
                hitUI.GetComponent<Animation>().Play();
                GameObject.Instantiate(particle);

                if (combo.value >= 3)
                {
                    comboUI.transform.localScale = Vector3.one;
                    comboUI.GetComponentInChildren<Animation>().Play();
                }

            }
            else
            {
            //    Debug.Log(string.Format("{0} Perfect Missed", _track.beats[current]));
                _trackView.TriggerBeatView(current, TrackView.Trigger.Wrong);

                combo.value = 0;
                scoreMultiplier.value = 1;
                comboUI.transform.localScale = Vector3.zero;

                hitUI.GetComponent<Text>().color = Color.red;
                hit.value = "Bad";
                hitUI.GetComponent<Animation>().Play();

            }

        }

        void NextBeat()
        {
        //    Debug.Log("Tick");

            if(!_played && _track.beats[current] != -1)
            {
        //        Debug.Log(string.Format("{0} missed", _track.beats[current]));
                _trackView.TriggerBeatView(current,TrackView.Trigger.Missed);

                comboUI.transform.localScale = Vector3.zero;
                combo.value = 0;
                scoreMultiplier.value = 1;

                hitUI.GetComponent<Text>().color = Color.gray;
                hit.value = "Missed";
                hitUI.GetComponent<Animation>().Play();

            }
            _played = false;

            current++;
        }

        void ComboIncreaseScoreMultipier()
        {
            if(combo.value == 100)
            {
                scoreMultiplier.value = 2;
        //        Debug.Log("Score Multiplier =" + scoreMultiplier.value);
            }
            else if (combo.value == 300)
            {
                scoreMultiplier.value = 3;
        //        Debug.Log("Score Multiplier =" + scoreMultiplier.value);
            }
            else if (combo.value == 500)
            {
                scoreMultiplier.value = 4;
        //        Debug.Log("Score Multiplier =" + scoreMultiplier.value);
            }
            else if (combo.value == 1000)
            {
                scoreMultiplier.value = 5;
        //        Debug.Log("Score Multiplier =" + scoreMultiplier.value);
            }
            else if (combo.value == 2000)
            {
                scoreMultiplier.value = 6;
        //        Debug.Log("Score Multiplier =" + scoreMultiplier.value);
            }
        }

        private IEnumerator WaitAndStop()
        {
            yield return waitAndStop;
            enabled = false;
        }

        #endregion
    }
}

