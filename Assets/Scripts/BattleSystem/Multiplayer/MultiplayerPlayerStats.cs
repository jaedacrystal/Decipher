using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Pun;

public class MultiplayerPlayerStats : MonoBehaviourPunCallbacks
{
    public int maxBandwidth = 5;
    public int currentBandwidth;
    public TextMeshProUGUI bandwidthText;
    public GameObject bandwidthIcon; // Assign UI Image to display bandwidth
    public Sprite[] bandwidthIconArray; // Array of sprites for different bandwidth levels

    private void Start()
    {
        currentBandwidth = maxBandwidth;
        UpdateBandwidthUI();
    }

    public void UseBandwidth(int amount)
    {
        if (!photonView.IsMine) return; // Only the local player can use bandwidth

        int previousBandwidth = currentBandwidth;
        currentBandwidth -= amount;
        currentBandwidth = Mathf.Max(0, currentBandwidth);

        UpdateBandwidthUI(previousBandwidth);
    }

    public void RestoreBandwidth()
    {
        if (!photonView.IsMine) return; // Only the local player's bandwidth is restored

        int previousBandwidth = currentBandwidth;
        currentBandwidth = maxBandwidth;

        UpdateBandwidthUI(previousBandwidth);
    }

    private void UpdateBandwidthUI(int previousBandwidth = -1)
    {
        if (!photonView.IsMine) return; // Only update UI for the local player

        if (bandwidthText != null)
        {
            if (previousBandwidth != -1)
            {
                DOTween.To(() => previousBandwidth, x => bandwidthText.text = $"{x}/{maxBandwidth}", currentBandwidth, 0.5f).SetEase(Ease.OutQuad);
            }
            else
            {
                bandwidthText.text = $"{currentBandwidth}/{maxBandwidth}";
            }
        }

        if (bandwidthIcon != null && bandwidthIconArray.Length > 0)
        {
            int iconIndex = Mathf.Clamp(maxBandwidth - currentBandwidth, 0, bandwidthIconArray.Length - 1);
            bandwidthIcon.GetComponent<UnityEngine.UI.Image>().sprite = bandwidthIconArray[iconIndex];
        }
    }
}