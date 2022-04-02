using UnityEngine;

public static class ResolutionHandler
{
    public static void ToggleFullscreen()
    {

        # if UNITY_STANDALONE
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }
        else
        {
            Screen.fullScreen = true;
            Resolution[] resolutions = Screen.resolutions;
            Resolution res = resolutions[resolutions.Length - 1];
            Screen.SetResolution(res.width, res.height, FullScreenMode.ExclusiveFullScreen);
        }
        
        # else
        Screen.fullScreen = true;
        # endif
    }
}
