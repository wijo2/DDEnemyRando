using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using UnityEngine.SceneManagement;

namespace DDEnemyRando
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class DDEnemyRando : BaseUnityPlugin
    {
        const string pluginVersion = "1.0.0";
        const string pluginName = "DDEnemyRando";
        const string pluginGuid = "ddoor.wijo.DDEnemyRando";
        
        public DDoorAssetLoader.LoadableAsset[] enemyList = new DDoorAssetLoader.LoadableAsset[]{
            new DDoorAssetLoader.LoadableAsset("lvl_Graveyard", "MainRoom/_CONTENTS/Enemies/_E_HEADROLLER_HEADLESS Variant"),
            new DDoorAssetLoader.LoadableAsset("lvl_Graveyard", "MainRoom/_CONTENTS/Enemies/_E_BAT_Black Variant (4)"),
            new DDoorAssetLoader.LoadableAsset("lvl_Graveyard", "MainRoom/_CONTENTS/Enemies/_E_GRUNT_Redeemer (5)"),
            new DDoorAssetLoader.LoadableAsset("lvl_Graveyard", "R_GravediggerCrypt_Interior/_CONTENTS/BOSS_GraveDigger"),
            new DDoorAssetLoader.LoadableAsset("lvl_Graveyard", "MainRoom/_CONTENTS/NightOnly_Enemies/_E_DODGER"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaGardens", "R_Gardens/_CONTENTS/DayOnly/_E_GRUNT_Pot Variant (3)"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaGardens", "R_Gardens/_CONTENTS/DayOnly/_E_GHOUL (1)"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaGardens", "R_Gardens/_CONTENTS/DayOnly/_E_MAGE"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaGardens", "R_Gardens/_CONTENTS/NightOnly/_E_Slime_big_GREEN Variant"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaGardens", "R_Gardens/_CONTENTS/NightOnly/_E_BRUTE_GOLD Variant"),
            new DDoorAssetLoader.LoadableAsset("lvl_SailorMountain", "R_Outside/_CONTENTS/_E_KNIGHT"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaMansion", "_SCENE_MOVER/GroundFloor/R_Left/_CONTENTS/POT_Mimic_Magic"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaMansion", "_SCENE_MOVER/GroundFloor/R_Left/_CONTENTS/POT_Mimic_Melee"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaMansion", "_SCENE_MOVER/GroundFloor/R_Library/_CONTENTS/POT_Mimic_Explode"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaBasement", "_SCENE_MOVER/LaboratoryWing2/R_Archives/_CONTENTS/_E_PLAGUE_KNIGHT"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaBasement", "_E_Slime_big"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaBasement", "_E_Slime_med"),
            new DDoorAssetLoader.LoadableAsset("lvl_GrandmaBasement", "_E_Slime_small"),
            new DDoorAssetLoader.LoadableAsset("lvl_Forest", "_SceneMover/Room_ForestMain/_CONTENTS/_E_PLANT (1)"),
            new DDoorAssetLoader.LoadableAsset("lvl_Forest", "_SceneMover/Room_ForestMain/_CONTENTS/Enemies_Day/_E_GRUNT (6)"),
            new DDoorAssetLoader.LoadableAsset("lvl_Forest", "_SceneMover/Room_ForestMain/_CONTENTS/_E_FIREPLANT (5)"),
            new DDoorAssetLoader.LoadableAsset("lvl_Forest", "_SceneMover/Room_ForestMain/_CONTENTS/Enemies_Day/_E_DEKU_SCRUB (1)"),
            new DDoorAssetLoader.LoadableAsset("lvl_Forest", "_SceneMover/Dungeon/Room_CoverShootOut/_CONTENTS/_E_LURKER"),
            new DDoorAssetLoader.LoadableAsset("lvl_Forest", "_SceneMover/Room_ForestMain/_CONTENTS/Enemies_Night/_E_GHOUL_Rapid Variant"),
            new DDoorAssetLoader.LoadableAsset("lvl_Forest", "_SceneMover/Room_ForestMain/_CONTENTS/Enemies_Night/_E_MAGE_REDPURPLE"),
            new DDoorAssetLoader.LoadableAsset("lvl_Swamp", "_SceneMover/Room_Swamp/_CONTENTS/Enemies_Day/_E_JUMPER (4)"),
            new DDoorAssetLoader.LoadableAsset("lvl_Swamp", "_SceneMover/Room_Swamp/_CONTENTS/Enemies_Day/_E_BRUTE (1)"),
            new DDoorAssetLoader.LoadableAsset("lvl_mountaintops", "R_Mountaintops/_CONTENTS/Enemies_Day/_E_GRUNT_Yeti Variant"),
            new DDoorAssetLoader.LoadableAsset("lvl_mountaintops", "R_Mountaintops/_CONTENTS/NightOnly/_E_MAGE_Plague Variant"),
            new DDoorAssetLoader.LoadableAsset("boss_betty", "SceneMover/R_Betty/_CONTENTS/BETTY_MAIN/betty_boss"),
        };
        

        public static GameObject[] objectList;
        
        private static ManualLogSource log;

		private void Awake()
		{
            log = base.Logger;
            DDoorAssetLoader.DDoorAssetLoader.AddAsset(enemyList);
            SceneManager.sceneLoaded += OnSceneLoaded;
			new Harmony(pluginGuid).PatchAll(typeof(DDEnemyRando));
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!DDoorAssetLoader.DDoorAssetLoader.isLoadingDone || scene.name == "_GAMEMANAGER" || scene.name == "_PLAYER" || scene.name == "TitleScreen") { return; }
            if (objectList == null || objectList.Length == 0)
            {
                var tmpList = new List<GameObject>();
                foreach (var i in enemyList)
                {
                    if (DDoorAssetLoader.DDoorAssetLoader.loadedAssets.TryGetValue(i, out GameObject e))
                    {
                        e = Instantiate(e, parent:null);
						DontDestroyOnLoad(e);
						var c = e.GetComponent<Betty>();
						if (c != null)
						{
							c.roomCentre = e.transform;
							c.endingSoul = null;
						}
						var c2 = e.GetComponent<AI_Gravedigger>();
						if (c2 != null)
						{
							c2.roomCentre = e.transform;
						}
                        tmpList.Add(e);
                    }
                }
                objectList = tmpList.ToArray();
                return;
            }

			var killKeys = Resources.FindObjectsOfTypeAll<KillKey>();
			foreach (var brain in Resources.FindObjectsOfTypeAll<AI_Brain>())
			{
				var name = brain.gameObject.name;
				if (brain.gameObject.scene.buildIndex == -1 || 
						name == "betty_boss" || 
						name == "FROG_BOSS_MAIN" || 
						name == "FROG_BOSS_FAT" || 
						name == "grandma" || 
						name == "old_crow_boss_fbx" || 
						name == "BOSS_lord_of_doors NEW" ||
						name == "BOSS_lord_of_doors_Garden" ||
						name == "BOSS_lord_of_doors_Forest" ||
						name == "BOSS_lord_of_doors_Betty" ||
						name == "redeemer_BOSS" ||
						name == "_FORESTMOTHER_BOSS" ||
						name == "BOSS_GraveDigger"
						) { continue; }
				var obj = brain.gameObject;
				var newObject = SpawnEnemy(obj.transform);
				foreach (var kk in killKeys)
				{
					var i = Array.FindIndex(kk.enemies, x => ReferenceEquals(x, brain));
					if (i != -1)
					{
						kk.enemies[i] = newObject.GetComponent<AI_Brain>();
						break;
					}
				}
				UnityEngine.Object.Destroy(obj);
			}

        }

		public static GameObject SpawnEnemy(Transform t)
		{
			var replacer = GetRandomListEntry(objectList);
			GameObject newObject = Instantiate(replacer, t.position, t.rotation, t.parent);
			newObject.SetActive(true);
			UnityEngine.Object.Destroy(newObject.GetComponent<DDoorAssetLoader.StayAliveForever>());
			return newObject;
		}

		public static T GetRandomListEntry<T>(IEnumerable<T> list)
		{
			int len = list.Count();
			var index = UnityEngine.Random.Range(0, len);
			return list.ElementAt(index);
		}

		[HarmonyPatch(typeof(Betty), "FixedUpdate")]
		[HarmonyPrefix]
		public static void GetBettyRolling(Betty __instance, CameraFocusObject ___camFocus)
		{
			if (!__instance.active && (__instance.transform.position - PlayerGlobal.instance.transform.position).magnitude < 20)
			{
				__instance.DoActivateTrigger();
			}
			if (___camFocus != null)
			{
				UnityEngine.Object.Destroy(___camFocus);
				___camFocus = null;
			}
		}

		[HarmonyPatch(typeof(EnemyWave), "spawnEnemy")]
		[HarmonyPostfix]
		public static void ReplaceWave(EnemyWave __instance)
		{
			foreach (AI_Brain b in __instance.GetComponentsInChildren<AI_Brain>())
			{
				if (b.name.Contains("WAVE FLAG")) { continue; }
				b.transform.position += Vector3.up * 2;
				var newObject = SpawnEnemy(b.transform);
				var newBrain = newObject.GetComponent<AI_Brain>();
				__instance.AddBrain(newBrain);
				newObject.name += "WAVE FLAG";
				UnityEngine.Object.Destroy(b.gameObject);
				return;
			}
		}

		[HarmonyPatch(typeof(AI_Gravedigger), "spawnZombie")]
		[HarmonyPrefix]
		public static bool StopSoftlock() { return false; }
	}
}
