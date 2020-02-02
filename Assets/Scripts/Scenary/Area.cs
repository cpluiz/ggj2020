using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour{
    // 0 is Up
    // 1 is Right
    // 2 is Down
    // 3 is Left
    [SerializeField]private Area[] linkedAreas = new Area[4];
    [SerializeField]private Portal[] linkedPortals = new Portal[4];

    [SerializeField] private GameObject challengeContainer, resourceContainer, goalContainer;
    // 0 is normal
    // 1 is challenge
    // 2 is resource
    // 3 is goal
    public int type;

    public int waypoints{
        get{
            int total = 0;
            for (int i = 0; i < 4; i++){
                total += linkedPortals[i] != null ? 1 : 0;
            }
            return total;
        }
    }

    void Awake(){
        challengeContainer.SetActive(false);
        resourceContainer.SetActive(false);
        goalContainer.SetActive(false);
    }

    public bool HasPortalInDirection(LinkedDirection direction){
        return linkedPortals[(int) direction] != null;
    }

    public bool HasPortals(bool[] directions){
        return
            (directions[0] ? HasPortalInDirection(LinkedDirection.up) : true) &&
            (directions[1] ? HasPortalInDirection(LinkedDirection.right) : true) &&
            (directions[2] ? HasPortalInDirection(LinkedDirection.down) : true) &&
            (directions[3] ? HasPortalInDirection(LinkedDirection.left) : true);
    }

    public void SetLinkedArea(Area areaRef, LinkedDirection direction){
        linkedAreas[(int)direction] = areaRef;
    }

    public void MoveCameraToArea(LinkedDirection direction){
        if (linkedAreas[(int) direction] == null)
            return;
        LevelManager.instance.MoveCameraToArea(linkedAreas[(int)direction].transform, direction);
    }

    // 0 is normal
    // 1 is challenge
    // 2 is resource
    // 3 is goal
    public void SetType(int type){
        this.type = type;
        challengeContainer.SetActive(type==1);
        resourceContainer.SetActive(type==2);
        goalContainer.SetActive(type==3);
    }
    
}

public enum LinkedDirection{
    up = 0,
    right = 1,
    down = 2,
    left = 3,
}