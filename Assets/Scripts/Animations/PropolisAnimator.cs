using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Propolis {
    public class PropolisAnimator : MonoBehaviour
    {
        [Header("Controllers")]
        public GameController gameController;
        public HiveGameController hiveController;
        public MolecularGameController molecularGameController;

        [Header("Exports")]
        public PropolisExport propolisExport;



        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Activate()
        {
            if (PropolisData.Instance.IsGamePlaying)
            {
                gameController.SendMessage(PropolisActions.Stop);

            }

        }

        public void Desactivate()
        {

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

