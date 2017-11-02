using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexControllerScriptAvecCouleur : MonoBehaviour {

    private GameControllerScript gameController;
    Vector3 newTuilePos; //world position de chaque nouvelle tuile apparaissant autour de cette tuile-ci
    public bool isActive;
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
    public Collider2D match;
    private string matchColor;
    private Vector2 pos2D = new Vector2();
    private List<Collider2D> OrangePackLocal;
    private List<Collider2D> BluePackLocal;
    private List<Collider2D> RedPackLocal;
    private List<Collider2D> YellowPackLocal;
    


    private void Start()
    {
        
        if (hexID == 7)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }
        pos2D = (Vector2)transform.position;
        SomeoneOn();
        if (isActive)
        {
            Verify();
        }
       
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
        lifeTime = 30;


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





    private void Update()
    {

        

        if (someoneOn == true)
        {
            isActive = true;
        }

        if (isActive)
        {
            lifeTime = lifeTime - Time.deltaTime;
            if (lifeTime < 0)
            {
                lifeTime = 0;
                isActive = false;
            }
        }


        SendData();


    }




    public void Verify()
    {
        Debug.Log("verifying");
        Collider2D[] otherColliders;
        OrangePackLocal = new List<Collider2D>();
        BluePackLocal = new List<Collider2D>();
        RedPackLocal = new List<Collider2D>();
        YellowPackLocal = new List<Collider2D>();
        otherColliders = Physics2D.OverlapCircleAll(pos2D, 2);

        for (int i = 0; i < otherColliders.Length; i++)
        {
            match = otherColliders[i];
            matchColor = match.GetComponent<HexControllerScript>().color;

            if (matchColor == "orange" && hexID != match.GetComponent<HexControllerScript>().hexID)
            {
                OrangePackLocal.Add(match);
                Debug.Log("HexID " + hexID + "orangepacklocal " + OrangePackLocal.Count);
            }

            if (matchColor == "blue" && hexID != match.GetComponent<HexControllerScript>().hexID)
            {
                BluePackLocal.Add(match);
                Debug.Log("HexID " + hexID + "bluepacklocal " + BluePackLocal.Count);
            }

            if (matchColor == "red" && hexID != match.GetComponent<HexControllerScript>().hexID)
            {
                RedPackLocal.Add(match);
                Debug.Log("HexID " + hexID + "redpacklocal " + RedPackLocal.Count);
            }

            if (matchColor == "yellow" && hexID != match.GetComponent<HexControllerScript>().hexID)
            {
                YellowPackLocal.Add(match);
                Debug.Log("HexID " + hexID + "yellowpacklocal " + YellowPackLocal.Count);
            }


            //si une tuile dans son environnement est isBad, on veut que cette mauvaise tuile vérifie son état pour voir si elle est maintenant encerclée
            if (match.GetComponent<HexControllerScript>().isBad == true)
            {
                match.GetComponent<HexControllerScript>().Verify();
            }
        }

        //Savoir si une mauvaise tuile est entourée de tuiles toutes de la même couleur
        if (isBad == true)
        {
            if (OrangePackLocal.Count == 6 || BluePackLocal.Count == 6 || RedPackLocal.Count == 6 || YellowPackLocal.Count == 6)
            {
                isCircled = true;
                Debug.Log(hexID + " isCicrled");
                //animation(); déclenche un fonction d'animation pour passer de mauvaise à bonne tuile
                isBad = false;
            }

        }

        Array.Clear(otherColliders, 0, otherColliders.Length);
        
    }








    void SendData ()
    {
        //hexDataScript.SendInfos(hexID, isActive, lifeTime);

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
