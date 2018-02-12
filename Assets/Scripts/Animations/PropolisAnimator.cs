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
            Activate();
            ClimaxDirector.gameObject.SetActive(true);
            _isClimaxPlaying = true;
            ClimaxDirector.Stop();
            ClimaxDirector.Play();
            

        }


        public void Activate()
        {
            if (PropolisData.Instance.IsGamePlaying)
            {
                propolisExport.SendClimaxState(1);
                gameController.SetDetectionOFF();
                gameController.propolisManager.SendCommand(PropolisActions.Stop);
                StopAllCoroutines();
                StartCoroutine(StartClimaxEndWatch());

            }

        }
        public IEnumerator StartClimaxEndWatch() {
            yield return new WaitForSecondsRealtime((float)ClimaxDirector.duration);
            Desactivate();
            gameController.propolisManager.SendCommand(PropolisActions.Play);

        }

        public void Desactivate()
        {
            propolisExport.SendClimaxState(0);
            ClimaxDirector.Stop();
            gameController.SetDetectionON();
            _isClimaxPlaying = false;
            ClimaxDirector.gameObject.SetActive(false);

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

