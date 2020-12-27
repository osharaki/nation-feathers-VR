using UnityEngine;
using System.Collections;

public class GeroBeamHit : MonoBehaviour {
	private GameObject ParticleA;
	private GameObject ParticleB;
	private GameObject HitFlash;
	
	private float PatA_rate;
	private float PatB_rate;

	private ParticleSystem PatA;
	private ParticleSystem PatB;
    public Color col;
	public void SetViewPat(bool b)
	{
		if(b){
            //PatA.emissionRate = PatA_rate;
            SetEmissionRate(PatA, PatA_rate); //Added by osharaki
            //PatB.emissionRate = PatB_rate;
            SetEmissionRate(PatB, PatB_rate); //Added by osharaki
            HitFlash.GetComponent<Renderer>().enabled = true;
		}else{
            //PatA.emissionRate = 0;
            SetEmissionRate(PatA, 0); //Added by osharaki
            //PatB.emissionRate = 0;
            SetEmissionRate(PatB, 0); //Added by osharaki
            HitFlash.GetComponent<Renderer>().enabled = false;
		}
	}    

    // Use this for initialization
    void Start () {
        col = new Color(1, 1, 1);
        /*ParticleA = transform.FindChild("GeroParticleA").gameObject;
		ParticleB = transform.FindChild("GeroParticleB").gameObject;
		HitFlash = transform.FindChild("BeamFlash").gameObject;*/

        //added by osharaki
        ParticleA = transform.Find("GeroParticleA").gameObject;
        ParticleB = transform.Find("GeroParticleB").gameObject;
        HitFlash = transform.Find("BeamFlash").gameObject;

        PatA = ParticleA.gameObject.GetComponent<ParticleSystem>();
        //PatA_rate = PatA.emissionRate;
        PatA_rate = GetEmissionRate(PatA); //added by osharaki
        //PatA.emissionRate = 0;
        SetEmissionRate(PatA, 0); //added by osharaki
		PatB = ParticleB.gameObject.GetComponent<ParticleSystem>();
        //PatB_rate = PatB.emissionRate;
        PatB_rate = GetEmissionRate(PatB); //added by osharaki
        SetEmissionRate(PatB, GetEmissionRate(PatB)); //added by osharaki
		//PatB.emissionRate = 0;
        SetEmissionRate(PatB, 0); //added by osharaki

		HitFlash.GetComponent<Renderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        //PatA.startColor = col;
        //PatB.startColor = col;
        var mainPatA = PatA.main; //added by osharaki
        mainPatA.startColor = col;//added by osharaki
        var mainPatB = PatB.main; //added by osharaki
        mainPatB.startColor = col;//added by osharaki
        HitFlash.GetComponent<Renderer>().material.SetColor("_Color", col*1.5f);
    }

    /// <summary>
    /// Added by osharaki. Source https://forum.unity3d.com/threads/what-is-the-unity-5-3-equivalent-of-the-old-particlesystem-emissionrate.373106/
    /// </summary>
    /// <param name="particleSystem"></param>
    /// <param name="emissionRate"></param>
    private void SetEmissionRate(ParticleSystem particleSystem, float emissionRate)
    {
        var emission = particleSystem.emission;
        //var rate = emission.rate;
        var rate = emission.rateOverTime; //added by osharaki
        rate.constantMax = emissionRate;
        //emission.rate = rate;
        emission.rateOverTime = rate; //added by osharaki
    }

    /// <summary>
    /// Added by osharaki. Source https://forum.unity3d.com/threads/what-is-the-unity-5-3-equivalent-of-the-old-particlesystem-emissionrate.373106/
    /// </summary>
    /// <param name="particleSystem"></param>
    /// <returns></returns>
    private float GetEmissionRate(ParticleSystem particleSystem)
    {
        //return particleSystem.emission.rate.constantMax;
        return particleSystem.emission.rateOverTime.constantMax;
    }
}
