using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
public class ClimaxStepTrigger : MonoBehaviour {

    public PropolisExport export;
    public int Step;
    private void OnEnable()
    {
        export.SendClimaxStep(Step);   
    }
}
