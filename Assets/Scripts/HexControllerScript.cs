using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexControllerScript : MonoBehaviour {

    private GameControllerScript gameController;
    Vector3 newTuilePos; //world position de chaque nouvelle tuile apparaissant autour de cette tuile-ci
    private bool isActive;
    public bool IsActive {

        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
            Verify();
            ChangeColor();
            SendDataToTouchDesigner();
        }
    }


    public string packID;
    public int hexID;
    private HexData hexDataScript;
    public float lifeTime;
    public int otherPackID;
    public string otherHexColor;
    public bool newHex;
    public bool someoneOn;
    public string color;
    public bool isBad;
    public bool isCircled;
    public Collider2D otherHex;
    public OSC osc;

    private Vector2 pos2D = new Vector2();
    private List<Collider2D> ActiveHexPack;
    private bool checker;
    private Material material;





    private void Start()
    {
        osc = transform.parent.gameObject.GetComponent<OSC>();
        material = GetComponent<Renderer>().material;
        isBad = false;
        checker = false;
        IsActive = false;

        pos2D = (Vector2)transform.position;
        SomeoneOn();
        if (IsActive)
        {
            Verify();
        }

        //SendDataToTouchDesigner();

    }






    void SomeoneOn()
    {
        //est-ce une nouvelle tuile, car seulement les nouvelles tuiles vont runner les fonctions de détection des tuiles environnantes
        //une tuile est considérée new seulement lorsqu'une tuile passe de someoneOn = false à someoneOn = true, pour ne pas que son OnTriggerEnter tout le temps
        if (someoneOn)
        {
            newHex  = true;
        }


        //durée de vie restante de chaque tuile
        lifeTime = 300;


        //get the game controller object
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameControllerScript>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }

        //ajouter cette tuile à la liste dans le gameController
        //gameController.AjoutAListe(this.gameObject);


        //get the hexdata script pour accéder à ses fonctions plus tard
        //hexDataScript = gameObject.GetComponent<HexData>();
    }

    public void SendDataToTouchDesigner()
    {
        
        OscMessage message = new OscMessage();

        SendOscMessage("/hex", hexID, StatusToInt(GetStatus()));

    }



    private void SendOscMessage(string address, int value, int value2)
    {
        Debug.Log("sending osc: " + address + value.ToString() + " " + value2.ToString());

        OscMessage message = new OscMessage();

        message.address = address;
        message.values.Add(value);
        message.values.Add(value2);
        osc.Send(message);
    }


    int StatusToInt(string status)
    {
        int returnValue = 0;
        switch (status)
        {
            case "OFF": returnValue = 0; break;
            case "ON": returnValue = 1; break;
            case "BAD": returnValue = 2; break;
        }

        return returnValue;

    }

    string GetStatus()
    {
        string status = "OFF";
        if (isBad)
        {
            status = "BAD";
        }
        else if (IsActive)
        {
            status = "ON";
        }
        return status;
    }

    void ChangeColor()
    {
        switch (GetStatus())
        {

            case "OFF": material.color = Color.gray; break;
            case "ON": material.color = Color.white; break;
            case "BAD": material.color = Color.red; break;
        }
        
    }

    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0))
        {
            IsActive = !IsActive;
        }
            
    }

    private void Update()
    {
       

        /*if (this.checker != this.IsActive)
        {
            //Debug.Log(hexID + "changement d'état");
            if (gameObject.GetComponent<HexControllerScript>().IsActive == true)
            {
                //Debug.Log(hexID + "IsActive est true et je Verify()");
                
            }
           checker = IsActive;
        }*/
        

        /*if (someoneOn == true)
        {
            IsActive = true;
        }*/

        /*if (IsActive)
        {
            lifeTime = lifeTime - Time.deltaTime;
            if (lifeTime < 0)
            {
                lifeTime = 0;
                IsActive = false;
                checker = IsActive;
            }
            //Verify();
        }*/

    }

    public void Verify()
    {
        //Debug.Log("verifying");
        Collider2D[] otherColliders;
        ActiveHexPack = new List<Collider2D>();
        otherColliders = Physics2D.OverlapCircleAll(pos2D, 2);

        for (int i = 0; i < otherColliders.Length; i++)
        {
            otherHex = otherColliders[i];

            if (hexID != otherHex.GetComponent<HexControllerScript>().hexID && otherHex.GetComponent<HexControllerScript>().IsActive == true)
            {
                ActiveHexPack.Add(otherHex);
                //Debug.Log("HexID " + hexID + "ActiveHexPack count is " + ActiveHexPack.Count);
            }

            //si une tuile dans son environnement est isBad, on veut que cette mauvaise tuile vérifie son état pour voir si elle est maintenant encerclée
            if (otherHex.GetComponent<HexControllerScript>().isBad == true && hexID != otherHex.GetComponent<HexControllerScript>().hexID)
            {
                otherHex.GetComponent<HexControllerScript>().Verify();
            }
        }

        //Savoir si une mauvaise tuile est entourée de tuiles toutes actives
        if (isBad == true)
        {
            if (ActiveHexPack.Count ==6)
            {
                isCircled = true;
                //Debug.Log(hexID + " isCicrled");
                //isBad = false;
                SendDataToTouchDesigner();
            }
            else
            {
                //Debug.Log("ActiveHexPack around " + hexID + "is" + ActiveHexPack.Count);
            }

        }

        Array.Clear(otherColliders, 0, otherColliders.Length);
        
    }








   /* void OnTriggerEnter2D(Collider2D tuile) //infos relatives à chaque tuile qui apparait autour de cette tuile-ci
    {
        if (newHex)
        {
            //Debug.Log("touché");
            //newTuilePos = tuile.transform.position;
            //Debug.Log(newTuilePos);
            //CalculateAngle();

            otherPackID = tuile.gameObject.GetComponent<HexControllerScript>().packID;
            Debug.Log("l'autre tuile packID est " + otherPackID);
            Debug.Log("je joue");
            newHex = false;

            //fonction HexPackManager.ajouteràtableau(otherPackID)


        }


    }




    //     void OnTriggerExit2D(Collider2D tuile) //retirer les infos de chaque tuile qui disparrait autour de cette tuile-ci
    //     {
    //        Debug.Log("out");
    //        newTuilePos = tuile.transform.position;
    //        Debug.Log(newTuilePos);
    //     }





    /* void CalculateAngle()
     {
         Vector3 relative = transform.InverseTransformPoint(newTuilePos);
         float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
         Debug.Log("relative" + relative);
         Debug.Log("angle" + angle);

     }
     */


}
