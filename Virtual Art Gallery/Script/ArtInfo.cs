using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ArtInfo : MonoBehaviour {
    private RaycastHit hit;
    public GameObject viewMessage;
    public GameObject showArt;
    private bool isHit;
    public float rayDistance = 10;
    public RenderTexture videoTexture;
    private string NFTUrl;

    void Update() {
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance)) {
            Debug.DrawRay(transform.position, transform.forward, Color.red);

            if (hit.collider.tag.Equals("Art")) {
                viewMessage.SetActive(true);
                isHit = true;
            }
            else {
                viewMessage.SetActive(false);
                isHit = false;
            }
        }
        else {
            viewMessage.SetActive(false);
            isHit = false;
        }

        if (isHit) {
            if (Input.GetKeyDown(KeyCode.V) && !showArt.activeSelf) {
                showArt.SetActive(true);
                if (hit.transform.GetComponent<VideoPlayer>()) {
                    showArt.transform.GetComponent<RawImage>().texture = videoTexture;
                    showArt.transform.GetComponent<VideoPlayer>().url = hit.transform.GetComponent<VideoPlayer>().url;
                }
                else {
                    showArt.transform.GetComponent<RawImage>().texture =
                        hit.transform.GetComponent<Renderer>().material.mainTexture;
                }

                NFTUrl = hit.transform.GetComponent<ArtDetails>().address;
            }
        }


        if (Input.GetKeyDown(KeyCode.C)) {
            if (showArt.activeSelf) {
                VideoPlayer videoPlayer = showArt.transform.GetComponent<VideoPlayer>();
                videoPlayer.targetTexture.Release();

                showArt.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            Application.OpenURL(NFTUrl);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("MFPS/Scenes/MainMenu");
        }
    }
    
    
}