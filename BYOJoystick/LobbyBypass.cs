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
    /// (before any mods load). Instead of reverting the version at startup
    /// (which breaks modded lobby visibility), this class keeps the version
    /// as Modded and only spoofs it at the moment of joining a public lobby.
    /// </summary>
    public static class LobbyBypass
    {
        private static HarmonyLib.Harmony _harmony;

        /// <summary>
        /// Called from Plugin.Awake(). Removes mod loader lobby patches and
        /// installs our own patches for the join/lobby flow. The game version
        /// is left as Modded so modded lobbies remain visible and joinable.
        /// </summary>
        public static void Apply()
        {
            Plugin.Log("LobbyBypass: Applying lobby bypass...");
            _harmony = new HarmonyLib.Harmony("byoj.lobbybypass");

            Plugin.Log($"LobbyBypass: Current version = '{GameStartup.versionString}' (keeping as-is)");

            try { UnpatchModLoaderLobbyIntercept(); }
            catch (Exception ex) { Plugin.Log($"LobbyBypass: UnpatchModLoader failed - {ex}"); }

            try { ApplyBypassPatches(); }
            catch (Exception ex) { Plugin.Log($"LobbyBypass: ApplyBypassPatches failed - {ex}"); }
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
    }
}
