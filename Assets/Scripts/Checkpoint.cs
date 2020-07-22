using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int number = -1;
    [SerializeField] private MeshRenderer meshSign;
    [SerializeField] private bool isMeta = false;

    private void Start() {
        ResetCheck();
    }

    private void OnTriggerExit(Collider collider) {


        if (collider.tag.Equals("Player")) {

            var player = collider.gameObject.GetComponent<Player>() as IPlayer;

            if (player.GetCheckpoint() == number-1) {

                if (isMeta) {

                    player.AddLap();
             

                }
                else {
                    if (player.IsHuman()) {
                        MarkVisited();
                    }
                    player.SetCheckpoint(number);
                }

            }

           
           

        }


    }

    public void ResetCheck() {

        MarkVisited(true);
        
    }
    public bool IsMeta() {
        return isMeta;
    }
    private void MarkVisited(bool reset = false) {

        if (reset) {
            meshSign.material.SetColor("_BaseColor", Color.red);
        }
        else {
            meshSign.material.SetColor("_BaseColor", Color.green);
        }

        
    }


}
