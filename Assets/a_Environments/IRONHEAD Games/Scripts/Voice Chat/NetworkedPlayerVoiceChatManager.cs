using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Voice.PUN;
using UnityEngine.UI;
public class NetworkedPlayerVoiceChatManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI voiceState;

    [SerializeField]
    private Image micImage;

    [SerializeField]
    private Image speakerImage;

    [SerializeField]
    private PhotonVoiceView photonVoiceView;

    private PhotonVoiceNetwork punVoiceNetwork;

    private void Awake()
    {
        this.punVoiceNetwork = PhotonVoiceNetwork.Instance;
        this.micImage.enabled = false;
        this.speakerImage.enabled = false;
    }

    private void OnEnable()
    {
        this.punVoiceNetwork.Client.StateChanged += this.VoiceClientStateChanged;
    }

    private void OnDisable()
    {
        this.punVoiceNetwork.Client.StateChanged -= this.VoiceClientStateChanged;
    }

    private void Update()
    {
        if (this.punVoiceNetwork == null)
        {
            this.punVoiceNetwork = PhotonVoiceNetwork.Instance;
        }

        this.micImage.enabled = this.photonVoiceView.IsRecording;
        this.speakerImage.enabled = this.photonVoiceView.IsSpeaking;
    }


    private void VoiceClientStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
    {
        this.UpdateUiBasedOnVoiceState(toState);
    }

    private void UpdateUiBasedOnVoiceState(Photon.Realtime.ClientState voiceClientState)
    {
        this.voiceState.text = string.Format("PhotonVoice: {0}", voiceClientState);
        if (voiceClientState == Photon.Realtime.ClientState.Joined)
        {
            voiceState.gameObject.SetActive(false);
        }
    }
}
