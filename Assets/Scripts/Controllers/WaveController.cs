using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;

public class WaveController : MonoBehaviour {

    public MolecularGameController MolecularGameController;

	// Use this for initialization
	void Start () {
        MolecularGameController = GameObject.Find("Controllers").GetComponent<MolecularGameController>();

    }

    public void  ResetPosition()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AbstractItem atom = other.GetComponent<AbstractItem>();

        if(atom != null)
        {
            MolecularGameController.CorruptedAtomWithWave(atom);
        }
    }


    // Update is called once per frame
    public void UpdateMovement () {
        if (PropolisData.Instance.WaveActivated)
        {
            MolecularGameController.SetWavePosition(PropolisData.Instance.WaveProgress +  PropolisGameSettings.WaveSpeed);
            transform.position = new Vector3(
                Mathf.Lerp(
                    MolecularGameController.GameArea.x + MolecularGameController.GameArea.width,
                    MolecularGameController.GameArea.x,
                    PropolisData.Instance.WaveProgress
            ),
            transform.position.y,
            transform.position.z);
            if(PropolisData.Instance.WaveProgress >= 1.0f)
            {
                MolecularGameController.SetWaveActiveStatus(false);
            }
           
        }
    }
}
