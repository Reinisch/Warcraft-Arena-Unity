using UnityEngine;
using System.Collections;

public class Lighting : MonoBehaviour {

public float lighting = 1;
public Light lightPower;
public bool  flashFlg = false;
public float flashTimer = 0.3f;

private bool  lightKeepFlg = false;
public float revOnTime = 0;
public float keepOnTime = 0;
public float keepTime = 0;

public bool  flashingFlg = false;
public float minLight = 0;
public float maxLight = 1;
private bool  lightOffFlg = false;
public float flashingOff = 0;
public float flashingOffPower = 0;
public float flashingOffIntensity = 1;

void Start (){
	lightPower = this.GetComponent<Light>();
	
	flash();
	setRev();
	keepOn();
	setFlashingOff();
}

void Update (){
	
	if( flashingFlg ){
		if( lightOffFlg ){
			lightPower.intensity -= lighting * Time.deltaTime;
			if( lightPower.intensity <= minLight)lightOffFlg = false;
		}else{
			lightPower.intensity += lighting * Time.deltaTime;
			if( lightPower.intensity > maxLight )lightOffFlg = true;
		}
	}else	if( lightPower.intensity > 0 && lightPower.enabled && !lightKeepFlg){
		lightPower.intensity -= lighting * Time.deltaTime;
	}
	
	if( lightKeepFlg && keepTime > 0){
		keepTime -= Time.deltaTime;
		if( keepTime <= 0 )lightKeepFlg = false;
	}
}

	
	IEnumerator flash (){
		if( flashFlg ){
			lightPower.enabled = false;
			yield return new WaitForSeconds( flashTimer );
			lightPower.enabled = true;
		}
	}

	IEnumerator setRev (){
		if( revOnTime > 0){
			yield return new WaitForSeconds( revOnTime );
			lighting *= -1; 
		}
	}

	IEnumerator keepOn (){
		if(  keepOnTime > 0){
			yield return new WaitForSeconds( keepOnTime );
			lightKeepFlg = true;
		}
	}

	IEnumerator setFlashingOff (){
		if(  flashingOff > 0){
			yield return new WaitForSeconds( flashingOff );
			flashingFlg = false;
			if( flashingOffPower > 0 ){
				lightPower.intensity = flashingOffIntensity;
				lighting = flashingOffPower;
			}
		}
	}
}