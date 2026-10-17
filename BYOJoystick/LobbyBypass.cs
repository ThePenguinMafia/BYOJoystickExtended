using System;
using System.Reflection;
using HarmonyLib;
using Steamworks.Data;
using UnityEngine;
using VTOLVR.Multiplayer;

namespace BYOJoystick
{
    /// <summary>
    /// Bypasses the mod loader's lobby segregation so that client-side mods
    /// (like BYOJoystick) can join unmodded/vanilla multiplayer lobbies.
    ///
    /// The mod loader marks the game as "Modded" during SplashSceneController.Start
    /// (before any mods load). This class:
    ///   1. Reverts the version to Public so the MP menu is accessible
    ///   2. Unpatches the mod loader's lobby hooks
    ///   3. Prefixes LobbyQuery.WithEqual to skip the "feature" filter, so the
    ///      Steam query returns ALL lobbies (public + modded) instead of just one type
    ///   4. Patches JoinLobby to spoof version when only the release type letter differs
    /// </summary>
    public static class LobbyBypass
    {
        private static HarmonyLib.Harmony _harmony;

        /// <summary>
        /// Called from Plugin.Awake(). Reverts version to Public so the MP
        /// menu works, removes mod loader lobby patches, and installs our
        /// own patches for the join flow that spoof to match any lobby.
        /// </summary>
        public static void Apply()
        {
            Plugin.Log("LobbyBypass: Applying lobby bypass...");
            _harmony = new HarmonyLib.Harmony("byoj.lobbybypass");

            try { RevertGameVersion(); }
            catch (Exception ex) { Plugin.Log($"LobbyBypass: RevertGameVersion failed - {ex}"); }

            try { UnpatchModLoaderLobbyIntercept(); }
            catch (Exception ex) { Plugin.Log($"LobbyBypass: UnpatchModLoader failed - {ex}"); }

            try { ApplyBypassPatches(); }
            catch (Exception ex) { Plugin.Log($"LobbyBypass: ApplyBypassPatches failed - {ex}"); }

            try { PatchLobbyRefresh(); }
            catch (Exception ex) { Plugin.Log($"LobbyBypass: PatchLobbyRefresh failed - {ex}"); }
        }

        private static void RevertGameVersion()
        {
            var gameStartupType = typeof(GameStartup);

            var versionField = gameStartupType.GetField("_version",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (versionField == null)
            {
                Plugin.Log("LobbyBypass: Could not find GameStartup._version");
                return;
            }

            var version = versionField.GetValue(null);
            var releaseTypeField = version.GetType().GetField("releaseType",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (releaseTypeField == null)
            {
                Plugin.Log("LobbyBypass: Could not find releaseType field");
                return;
            }

            var currentType = releaseTypeField.GetValue(version);
            Plugin.Log($"LobbyBypass: Current releaseType = {currentType}");

            releaseTypeField.SetValue(version, GameVersion.ReleaseTypes.Public);
            versionField.SetValue(null, version);

            var versionSetField = gameStartupType.GetField("_versionSet",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            versionSetField?.SetValue(null, false);

            Plugin.Log($"LobbyBypass: Version reverted to Public => '{GameStartup.versionString}'");
        }

        private static void UnpatchModLoaderLobbyIntercept()
        {
            const string modLoaderId = "vtolvr.modloader";
            var vtmpType = typeof(VTMPMainMenu);

            var joinMethod = AccessTools.Method(vtmpType, "JoinLobbyButton");
            if (joinMethod != null)
            {
                _harmony.Unpatch(joinMethod, HarmonyPatchType.Prefix, modLoaderId);
                Plugin.Log("LobbyBypass: Unpatched JoinLobbyButton prefix");
            }

            var launchMethod = AccessTools.Method(vtmpType, "LaunchMPGameForScenario");
            if (launchMethod != null)
            {
                _harmony.Unpatch(launchMethod, HarmonyPatchType.Postfix, modLoaderId);
                Plugin.Log("LobbyBypass: Unpatched LaunchMPGameForScenario postfix");
            }
        }

        private static void ApplyBypassPatches()
        {
            // Patch VTOLMPLobbyManager.JoinLobby(Lobby, string, bool) to
            // skip the version string mismatch rejection
            var joinLobbyMethod = AccessTools.Method(
                typeof(VTOLMPLobbyManager), "JoinLobby",
                new[] { typeof(Lobby), typeof(string), typeof(bool) });

            if (joinLobbyMethod != null)
            {
                _harmony.Patch(joinLobbyMethod,
                    prefix: new HarmonyMethod(typeof(LobbyBypass), nameof(JoinLobby_Prefix)));
                Plugin.Log("LobbyBypass: Patched VTOLMPLobbyManager.JoinLobby");
            }
            else
            {
                Plugin.Log("LobbyBypass: Could not find JoinLobby(Lobby,string,bool)");
            }
        }

        /// <summary>
        /// Prefix for VTOLMPLobbyManager.JoinLobby. Before the original
        /// method checks "ver", we temporarily set our version to match
        /// the lobby's version (if only the release type letter differs).
        /// </summary>
        public static void JoinLobby_Prefix(Lobby l)
        {
            try
            {
                var lobbyVer = l.GetData("ver");
                if (string.IsNullOrEmpty(lobbyVer)) return;

                var ourVer = GameStartup.version.ToString();
                if (lobbyVer == ourVer) return;

                // Check if only the release type letter differs
                var lobbyNorm = lobbyVer.Replace('f', '_').Replace('m', '_').Replace('p', '_');
                var ourNorm = ourVer.Replace('f', '_').Replace('m', '_').Replace('p', '_');

                if (lobbyNorm == ourNorm)
                {
                    Plugin.Log($"LobbyBypass: Spoofing version '{ourVer}' -> '{lobbyVer}'");
                    ForceVersionString(lobbyVer);
                }
                else
                {
                    Plugin.Log($"LobbyBypass: Genuine mismatch: lobby='{lobbyVer}' ours='{ourVer}'");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log($"LobbyBypass: JoinLobby_Prefix error - {ex.Message}");
            }
        }

        /// <summary>
        /// Forces the game version to match a specific version string by
        /// parsing its release type letter.
        /// </summary>
        private static void ForceVersionString(string targetVer)
        {
            GameVersion.ReleaseTypes targetType;
            if (targetVer.Contains("m"))
                targetType = GameVersion.ReleaseTypes.Modded;
            else if (targetVer.Contains("p"))
                targetType = GameVersion.ReleaseTypes.Testing;
            else
                targetType = GameVersion.ReleaseTypes.Public;

            var gameStartupType = typeof(GameStartup);
            var versionField = gameStartupType.GetField("_version",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (versionField == null) return;

            var version = versionField.GetValue(null);
            var releaseTypeField = version.GetType().GetField("releaseType",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (releaseTypeField == null) return;

            releaseTypeField.SetValue(version, targetType);
            versionField.SetValue(null, version);

            var versionSetField = gameStartupType.GetField("_versionSet",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            versionSetField?.SetValue(null, false);
        }

        /// <summary>
        /// Patches the lobby browser query so that BOTH public and modded
        /// lobbies are visible. The game's RefreshRoutine builds a Steam query
        /// with .WithEqual("feature", releaseType) which limits results to one
        /// release type. We prefix LobbyQuery.WithEqual to skip when the key
        /// is "feature", which removes the filter entirely. Steam then returns
        /// lobbies of all release types (public + modded + testing).
        /// </summary>
        private static void PatchLobbyRefresh()
        {
            var withEqual = AccessTools.Method(typeof(LobbyQuery), "WithEqual",
                new[] { typeof(string), typeof(int) });
            if (withEqual == null)
            {
                Plugin.Log("LobbyBypass: Could not find LobbyQuery.WithEqual(string,int)");
                return;
            }

            _harmony.Patch(withEqual,
                prefix: new HarmonyMethod(typeof(LobbyBypass), nameof(WithEqual_Prefix)));
            Plugin.Log("LobbyBypass: Patched LobbyQuery.WithEqual to skip feature filter");
        }

        /// <summary>
        /// Prefix for LobbyQuery.WithEqual. When the lobby browser queries Steam,
        /// it filters by "feature" (release type int). We skip that specific filter
        /// so all lobbies (public + modded) are returned. Other WithEqual calls
        /// (like "lReady" and "pwh") pass through unchanged.
        /// </summary>
        public static bool WithEqual_Prefix(ref LobbyQuery __instance, ref LobbyQuery __result, string key)
        {
            if (key == "feature")
            {
                Plugin.Log("LobbyBypass: Skipping 'feature' lobby filter - all lobbies visible");
                __result = __instance;
                return false;
            }
            return true;
        }
    }
}
