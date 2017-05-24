using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Showbaby.Bluetooth;
using UnityEngine.Events;

public class StartButton : MonoBehaviour
{

    [SerializeField] private Button startButton;
    public UnityEvent OnStartGameEvent;
	// Use this for initialization
	void Start ()
    {
	    startButton.onClick.AddListener(StartGame);
	}

    void StartGame()
    {
        if (BluetoothSDK.BluetoothSdk.IsConnected)
        {
            OnStartGameEvent.Invoke();
        }
        else
        {
            BluetoothSDK.BluetoothSdk.OpenBluetoothPanel();
        }
    }
}
