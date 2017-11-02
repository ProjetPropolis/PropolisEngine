using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexData : MonoBehaviour {

    int ID;
    bool isActive;
    int LifeTime;
    public int _ID;
    public bool _isActive;
    public float _lifeTime;



    public void SendInfos(int ID, bool isActive, float lifeTime)//serialise les infos pour les envoyer en Json
    {
        int _ID = this.ID;
        bool _isActive = this.isActive;
        float _lifeTime = lifeTime;
        //Debug.Log("ID is " + _ID);
        //Debug.Log("isActive is " + _isActive);
        //Debug.Log("lifeTime is " + _lifeTime);

    }


}
