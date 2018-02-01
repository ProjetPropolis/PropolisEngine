using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Propolis {
    public class PropolisAnimator : MonoBehaviour
    {
        [Header("Controllers")]
        public GameController gameController;
        public HiveGameController hiveController;
        public MolecularGameController molecularGameController;
        private bool _isClimaxPlaying = false;

        [Header("Exports")]
        public PropolisExport propolisExport;

        public PlayableDirector ClimaxDirector;



        // Use this for initialization
        void Start()
        {
            ClimaxDirector.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartClimax()
        {
            if (!_isClimaxPlaying)
            {
                Activate();
                ClimaxDirector.gameObject.SetActive(true);
                _isClimaxPlaying = true;           
                ClimaxDirector.Play();
                StartCoroutine(WaitForClimaxEnd());
            }

        }
        private IEnumerator WaitForClimaxEnd() {
            yield return  new WaitForSecondsRealtime((float)78.0f);
            Desactivate();
        }

        public void Activate()
        {
            if (PropolisData.Instance.IsGamePlaying)
            {
                gameController.SetDetectionOFF();
                gameController.SendMessage(PropolisActions.Stop);

            }

        }

        public void Desactivate()
        {
            gameController.SetDetectionON();
            _isClimaxPlaying = false;
            gameController.SendMessage(PropolisActions.Play);

        }

        public void PlayAnimation(string name)
        {
            switch (name)
            {
                default:break;
            }
        }
    }
}

