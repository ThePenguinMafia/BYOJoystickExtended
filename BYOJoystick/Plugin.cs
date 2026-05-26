using System;
using System.Collections;
using System.IO;
using System.Reflection;
using ModLoader.Framework;
using ModLoader.Framework.Attributes;
using UnityEngine;

namespace BYOJoystick
{
    [ItemId("BYOJ")]
    public class Plugin : VtolMod
    {
        public BYOJ BYOJ;

        /// <summary>
        /// Set this to the manager <c>ShortName</c> you want to log (e.g. <c>"F22"</c>, <c>"SU47M"</c>).
        /// When empty, interactable/manifest discovery dumps are disabled.
        ///
        /// Use <c>"ALL"</c> (or <c>"*"</c>) to enable for all aircraft.
        /// You can also use comma/semicolon separated values (e.g. <c>"F22;SU47M"</c>).
        /// </summary>
        public static string VerboseInteractableDiscoveryForShortName = "";

        public static bool IsVerboseInteractableDiscoveryEnabledFor(string shortName)
        {
            if (string.IsNullOrWhiteSpace(shortName))
                return false;

            var spec = VerboseInteractableDiscoveryForShortName;
            if (string.IsNullOrWhiteSpace(spec))
                return false;

            spec = spec.Trim();
            if (spec.Equals("ALL", StringComparison.OrdinalIgnoreCase) || spec.Equals("*"))
                return true;

            var parts = spec.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (part.Trim().Equals(shortName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
        
        public static void Log(object msg)
        {
            var message = $"[BYOJ] {msg}";
            Debug.Log(message);
            TryWriteLogFile(message);
        }

        private void Awake()
        {
            Log("Loading BYO Joystick Plugin");
            
            StartCoroutine(LoadAssetBundle());
        }

        private IEnumerator LoadAssetBundle()
        {
            Log("Loading Asset Bundle...");
            string path = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\byoj";

            if (!File.Exists(path))
            {
                Log("Asset Bundle not found");
                yield break;
            }

            var request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            Log("Asset Bundle Loaded");
            Initialise(request.assetBundle);
        }

        private void Initialise(AssetBundle bundle)
        {
            Log("Loading prefab from Asset Bundle...");

            var assetRequest = bundle.LoadAssetAsync<GameObject>("BYOJ");
            var prefab       = assetRequest.asset as GameObject;

            if (prefab != null)
            {
                Log("Loaded prefab from Asset Bundle");
                BYOJ = Instantiate(prefab, transform.position, transform.rotation).GetComponent<BYOJ>();
                DontDestroyOnLoad(BYOJ);
                Log("BYOJ Started!");

                if (BYOJ == null)
                {
                    Log("ERROR: Instantiated prefab did not contain a BYOJ component.");
                }
                else
                {
                    if (BYOJ.BYOJUI == null)
                    {
                        Log("WARNING: BYOJ prefab's BYOJUI reference is null. UI will not appear.");
                    }
                    else
                    {
                        Log($"BYOJUI present. BYOJUI.gameObject.activeSelf={BYOJ.BYOJUI.gameObject.activeSelf}");
                    }
                }
            }
            else
                Log("Failed to load prefab from Asset Bundle");

            bundle.Unload(false);
        }

        public override void UnLoad()
        {
            Log("Unloading BYO Joystick Plugin");
            BYOJ.Unload();
            Destroy(BYOJ.gameObject);
            Destroy(this);
        }

        private static void TryWriteLogFile(string message)
        {
            try
            {
                if (!Directory.Exists(PilotSaveManager.saveDataPath))
                    Directory.CreateDirectory(PilotSaveManager.saveDataPath);

                var logPath = Path.Combine(PilotSaveManager.saveDataPath, "BYOJ_log.txt");
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(logPath, $"[{timestamp}] {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Debug.Log($"[BYOJ] Failed to write log file: {ex.Message}");
            }
        }
    }
}
