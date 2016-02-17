﻿using UnityEngine;
using System.Collections;

public class NetworkCharacterMovement : Photon.MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
    Vector3 realVelocity = Vector3.zero;
	private string url = "http://oamkpo2016.esy.es/kills";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
		} 
		else {
            transform.position = Vector3.Lerp(transform.position, realPosition, 5 * Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 5 * Time.deltaTime);
            transform.GetComponent<Rigidbody>().velocity = realVelocity;
		}

	}

	public void sendKill() {
		StartCoroutine (PostKill ());
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.isWriting){
			//our player

			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
            stream.SendNext(transform.GetComponent<Rigidbody>().velocity);

		}
		else{
			//someones player
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
            realVelocity = (Vector3)stream.ReceiveNext();
		}
	}

	IEnumerator PostDeath() {
		WWWForm wwwForm = new WWWForm ();
		wwwForm.AddField ("ID", GetComponent<ChatManager> ().id.ToString ());
		wwwForm.AddField ("killed", "1");
		WWW hs_post = new WWW (url, wwwForm);
		yield return hs_post;
		if (hs_post.error != null) {
			Debug.Log ("Error posting data to database: " + hs_post.error);
		}
	}

	IEnumerator PostKill() {
		WWWForm wwwForm = new WWWForm ();
		wwwForm.AddField ("ID", GetComponent<ChatManager> ().id.ToString ());
		wwwForm.AddField ("kills", "1");
		WWW hs_post = new WWW (url, wwwForm);
		yield return hs_post;
		if (hs_post.error != null) {
			Debug.Log ("Error posting data to database: " + hs_post.error);
		}
	}
	
}
