using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;

public class WaveController : MonoBehaviour {

    public MolecularGameController MolecularGameController;

	// Use this for initialization
	void Start () {
        if (PropolisData.Instance.WaveActivated)
        {          
            if(PropolisData.Instance.WaveProgress <= 0.0f)
            {

            }
        }
    }

    public void  ResetPosition()
    {

    }

    private void OnTriggerEnter2D(Collider other)
    {

    }

    public void  SetWavePosition(float position)
    {
        position = Mathf.Clamp(position, 0.0f, 1.0f);
        MolecularGameController.SendCommand(string.Format("{0} {1}", PropolisActions.SetWavePosition, position));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
