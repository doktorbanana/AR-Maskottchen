using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Wecken : MonoBehaviour
{
    [SerializeField]
    GameObject buttons;
    [SerializeField]
    Image fill;

    float currentscale;
    float MicLoudness;
    private string device;
    AudioClip recordedClip;
    
    


    #region Unity Callbacks

    void OnEnable(){
        InitMic();
        currentscale = 0;
    }

    void OnDisable(){
        StopMicrophone();
    }


    void Update()
    {

        // Die Länge des Ladebalkens soll der Summe der gemessenen Amplituden entsprechen

        if(currentscale < 1){
            currentscale += GetAmplitude() /10;
            fill.rectTransform.localScale = new Vector2(1, currentscale);
        }else{
            Maskottchen.Manager.Maskottchen_Manager.wakingUp = true;
            this.gameObject.SetActive(false);
            buttons.SetActive(true);
        }

    }

    #endregion

    
    #region Private Methods

    //Mic initialization
    
    void InitMic(){
        if(device == null) device = Microphone.devices[0];
            recordedClip = Microphone.Start(device, false, 999, 44100);
        }
     
    void StopMicrophone()
    {
        Microphone.End(device);
    }
     
 
    //Daten vom Mikrofon in Audio Clip schreiben
    float  GetAmplitude()
    {
        int sampleWindow = 128;
        float amp = 0;
        float[] waveData = new float[sampleWindow];
        
        int micPosition = Microphone.GetPosition(null)-(sampleWindow+1);
        if (micPosition < 0) return 0;

        recordedClip.GetData(waveData, micPosition);

        // Amplitude finde
        for (int i = 0; i < sampleWindow; i++) {
            float wavePeak = waveData[i] * waveData[i];   

            if (amp < wavePeak)
            {
                amp = wavePeak;
            }
        }

        return amp;
    }

    #endregion
}
