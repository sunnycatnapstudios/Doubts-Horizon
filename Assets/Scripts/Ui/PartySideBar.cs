using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySideBar : MonoBehaviour {
    public List<Image> sideBarProfilePictures;
    public List<GameObject> sideBarSlots;
    public GameObject profilePrefab;

    public void UpdateSideBar(bool destory) {
        if (!destory) {
            GameObject newSideBarProfile = Instantiate(profilePrefab, transform);
            newSideBarProfile.transform.SetSiblingIndex(0);
            newSideBarProfile.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(35, 35);
            sideBarSlots.Add(newSideBarProfile);
            newSideBarProfile.name = "Party Slot" + sideBarSlots.Count;
        } else {
            GameObject removedSideBarProfile = sideBarSlots[sideBarSlots.Count - 1];
            sideBarSlots.RemoveAt(sideBarSlots.Count - 1);
            Destroy(removedSideBarProfile);
        }
    }

    void Start() {
        // for (int i = 0; i < 3; i++)
        // {
        //     GameObject newSideBarProfile = Instantiate(profilePrefab, transform);
        //     newSideBarProfile.name = "Party SLot" + i;
        // }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            if (sideBarSlots.Count < 3) {
                UpdateSideBar(false);
            } else {
                Debug.Log("Woah there bub");
            }
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            if (sideBarSlots.Count > 0) {
                UpdateSideBar(true);
            } else {
                Debug.Log("No slots available");
            }
        }

        // this.GetComponent<VerticalLayoutGroup>().spacing = 15 - (sideBarSlots.Count);
        this.GetComponent<VerticalLayoutGroup>().spacing = Mathf.Lerp(
            this.GetComponent<VerticalLayoutGroup>().spacing,
            15 - ((sideBarSlots.Count - 1) * 2),
            Time.deltaTime * 10f);
    }
}
