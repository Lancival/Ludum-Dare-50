using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class IconManager : MonoBehaviour
{

    private List<GameObject> rings;
    private List<string> names;
    private GameObject julia;

    private static IconManager _instance = null;
    public static IconManager instance => _instance;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;

        rings = new List<GameObject>();
        names = new List<string>();
        foreach (Transform icon in transform)
        {
            GameObject ring = icon.GetChild(0).gameObject;
            rings.Add(ring);
            ring.SetActive(false);
            names.Add(icon.gameObject.name);
        }

        julia = transform.GetChild(6).gameObject;
    }

    public void Speak(string speakerName)
    {
        foreach (GameObject ring in rings)
            ring.SetActive(false);

        if (speakerName == "Everyone")
        {
            foreach (GameObject ring in rings)
                ring.SetActive(true);
        }
        else
        {
            rings[names.IndexOf(speakerName)].SetActive(true);
        }
    }

    [YarnCommand("Julia")]
    public static void ToggleJulia()
    {
        _instance.julia.SetActive(!_instance.julia.activeSelf);
    }
}
