using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Battlefield:MonoBehaviour
{
    public int ActorNum = 50;
    public GameObject HUDObject;
    public GameObject ActorObject;
    public GameObject Parent;
    private List<BattleActor> actors;
    private List<ActorHUD> actorHUDs;
    private void Start()
    {
        actors = new List<BattleActor>();
        actorHUDs = new List<ActorHUD>();
        for (int i = 0;i< ActorNum; i++)
        {
            BattleActor actor = CreateActor();
            actors.Add(actor);
            GameObject obj = GameObject.Instantiate(HUDObject);
            Transform t = obj.transform;
            t.SetParent(Parent.transform);
            t.localScale = Vector3.one;
            t.localPosition = new Vector3(UnityEngine.Random.Range(1,500), UnityEngine.Random.Range(1, 500), UnityEngine.Random.Range(1, 500));
            obj.SetActive(true);
            ActorHUD hud = obj.GetComponent<ActorHUD>();
            hud.ActorBuild(actor);
            hud.ActorStart();
        }
    }

    private BattleActor CreateActor()
    {
        BattleActor actor = GameObject.Instantiate(ActorObject).GetComponent<BattleActor>();
        actor.gameObject.SetActive(true);
        actor.RandomCreate();
        return actor;
    }
}
