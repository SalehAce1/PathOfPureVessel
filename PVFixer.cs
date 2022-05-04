using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Logger = Modding.Logger;
using Satchel;
namespace PVPoP
{
    internal class PVFixer : MonoBehaviour
    {
        PlayMakerFSM _control;
        private GameObject blasts;
        public GameObject guard;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }
        private void Start()
        {
            gameObject.transform.SetPosition2D(guard.transform.position.x, guard.transform.position.y);
            blasts = Instantiate(PVPoP.preloadedGO["blasts"]);
            blasts.SetActive(true);
            foreach (Transform i in blasts.transform)
            {
                i.gameObject.SetActive(true);
                i.position = i.position + new Vector3(186f, 115f, 0f);
                PlayMakerFSM fsm = i.gameObject.LocateMyFSM("Control");
                fsm.GetAction<RandomFloat>("Pos Low", 0).min.Value = 120f;
                fsm.GetAction<RandomFloat>("Pos High", 0).min.Value = 125f;
                fsm.GetAction<RandomFloat>("Pos Low", 0).max.Value = 125f;
                fsm.GetAction<RandomFloat>("Pos High", 0).max.Value = 130f;
                fsm.SetState("Init");
                fsm.enabled = true; 
            }
            
            Log("Done load");
            _control.InsertCustomAction("Plume Gen",(() =>
            {
                GameObject go = _control.GetAction<SpawnObjectFromGlobalPool>("Plume Gen", 0).storeObject.Value;
                PlayMakerFSM fsm = go.LocateMyFSM("FSM");
                fsm.GetAction<FloatCompare>("Outside Arena?", 2).float2.Value = Mathf.Infinity;
                fsm.GetAction<FloatCompare>("Outside Arena?", 3).float2.Value = -Mathf.Infinity;
            }),3);
            _control.InsertCustomAction("Plume Gen",(() =>
            {
                GameObject go = _control.GetAction<SpawnObjectFromGlobalPool>("Plume Gen", 4).storeObject.Value;
                PlayMakerFSM fsm = go.LocateMyFSM("FSM");
                fsm.GetAction<FloatCompare>("Outside Arena?", 2).float2.Value = Mathf.Infinity;
                fsm.GetAction<FloatCompare>("Outside Arena?", 3).float2.Value = -Mathf.Infinity;
            }),5);
            _control.InsertCustomAction("Focus Recover", (() =>
            {
                foreach (Transform i in blasts.transform)
                {
                    i.gameObject.LocateMyFSM("Control").SendEvent("BLAST");
                }
            }),1);
            _control.FsmVariables.FindFsmFloat("Stun Land Y").Value = 125.5f;
            _control.FsmVariables.FindFsmFloat("Left X").Value = 214.2f;
            _control.FsmVariables.FindFsmFloat("Right X").Value = 244.2f;
            _control.FsmVariables.FindFsmFloat("TeleRange Max").Value = 242f;
            _control.FsmVariables.FindFsmFloat("TeleRange Min").Value = 216f;
            _control.FsmVariables.FindFsmFloat("Plume Y").Value = 120.15f; //129f
            _control.GetAction<FloatCompare>("Pos Check", 2).float2.Value = 224f;
            _control.GetAction<FloatCompare>("Pos Check", 3).float2.Value = 234f;
        }

        private void Log(object o)
        {
            Logger.Log("[PV Fixer] " + o);
        }
    }
}