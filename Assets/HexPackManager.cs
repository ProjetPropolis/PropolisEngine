using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPackManager : MonoBehaviour {

	public List<GameObject> OrangePack = new List <GameObject>();
    public List<GameObject> BluePack = new List<GameObject>();
    public List<GameObject> RedPack = new List<GameObject>();
    public List<GameObject> YellowPack = new List<GameObject>();
    GameObject hex;

    public void AddToPack (GameObject m_hex, string m_color)
    {
        GameObject hex = m_hex;
        
        string color = m_color;

        if (color == "orange")
        {
            OrangePack.Add(hex);
        }
        if (color == "blue")
        {
            BluePack.Add(hex);
        }
        if (color == "red")
        {
            RedPack.Add(hex);
        }
        if (color == "yellow")
        {
            YellowPack.Add(hex);
        }

    }

    public void RemoveFromPack(GameObject m_hex, string m_packID)
    {
        Debug.Log("Got removed from " + m_packID);
    }

    private void Start()
    {
        Debug.Log(OrangePack.Count);
    }
}
