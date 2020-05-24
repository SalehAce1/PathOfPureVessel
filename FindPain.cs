using System.Collections;
using System.Linq;
using System.Reflection;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Modding.Logger;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace PVPoP
{
    internal class FindPain : MonoBehaviour
    {
        private GameObject newPaleLurk;
        private Texture oldPLTex;
        private void Start()
        {
            USceneManager.activeSceneChanged += SceneChanged;
            PlayerData.instance.killedBindingSeal = false;
            PlayerData.instance.killsBindingSeal = 0;
            PlayerData.instance.newDataBindingSeal = false;
        }

        private void SceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name != "White_Palace_20") return;
            StartCoroutine(AddComponent());
        }

        private IEnumerator AddComponent()
        {
            yield return null;
            Log("H");
            GameObject g1 = new GameObject();
            GameObject g2 = new GameObject();
            foreach (var i in FindObjectsOfType<GameObject>().Where(x => x.name.Contains("Royal Gaurd")))
            {
                if (i.name.Contains("(1)")) g2 = i;
                else g1 = i;
                i.SetActive(false);
            }
            GameObject pv = Instantiate(PVPoP.preloadedGO["pv"]);
            PlayMakerFSM _control = pv.LocateMyFSM("Control");
            _control.FsmVariables.FindFsmFloat("Left X").Value = 214.2f;
            _control.FsmVariables.FindFsmFloat("Right X").Value = 244.2f;
            _control.FsmVariables.FindFsmFloat("TeleRange Max").Value = 242f;
            _control.FsmVariables.FindFsmFloat("TeleRange Min").Value = 216f;
            _control.FsmVariables.FindFsmFloat("Plume Y").Value = 135f; //129f
            ConstrainPosition cp = pv.GetComponent<ConstrainPosition>();
            //214 244
            cp.xMax = 243f;
            cp.xMin = 215.2f;
            pv.transform.SetPosition2D(g2.transform.position.x, g2.transform.position.y);
            pv.SetActive(false);
            yield return new WaitWhile(() => HeroController.instance.transform.GetPositionX() < 227f || HeroController.instance.transform.GetPositionY() > 139.6f);
            Log("H2");
            pv.SetActive(true);
            Log("Done");
            pv.LocateMyFSM("Control").enabled = true;
            pv.AddComponent<PVFixer>().guard = g2;
            pv.GetComponent<HealthManager>().OnDeath += DoOnDeath;
        }

        private void DoOnDeath()
        {
            StartCoroutine(End());
        }
        private IEnumerator End()
        {
            yield return new WaitForSeconds(2.5f);
            GameObject gate = GameObject.Find("Battle Scene");
            PlayMakerFSM pfsm = gate.LocateMyFSM("Battle Control");
            pfsm.SetState("End Pause");
            Log("DONE");
            PlayMakerFSM hud = GameObject.Find("Hud Canvas").LocateMyFSM("Slide Out");
            Log(hud == null);
            hud.SendEvent("IN");
            Log("DONE2");
        }
        private void OnDestroy()
        {
            USceneManager.activeSceneChanged -= SceneChanged;
        }

        public static void Log(object o)
        {
            Logger.Log("[Pain Finder] " + o);
        }
    }
}