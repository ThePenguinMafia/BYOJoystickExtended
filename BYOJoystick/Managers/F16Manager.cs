using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using VTOLVR.Multiplayer;
using MFDButtons = MFD.MFDButtons;

namespace BYOJoystick.Managers
{
    public class F16Manager : Manager
    {
        public override string GameName => "F-16";
        public override string ShortName => "F16";
        public override bool IsMulticrew => false;

        private static string Dash => "Local/DashCanvas";
        private static string SideJoystick => "Local/SideStickObjects";
        private static string CenterJoystick => "Local/CenterStickObjects";

        private CJoystick Joysticks(string name, string root, bool nullable, bool checkName, int idx)
        {
            return GetJoysticksByPaths(name, SideJoystick, CenterJoystick);
        }

        protected override void PreMapping()
        {
            var interactables = Vehicle.GetComponentsInChildren<VRInteractable>(true);
            foreach (var i in interactables)
                Plugin.Log($"[F16C] {i.GetType().Name} : \"{i.name}\"");
        }

        protected override void CreateFlightControls()
        {
            FlightAxisC("Joystick Pitch", "Joystick", Joysticks, CJoystick.SetPitch);
            FlightAxisC("Joystick Yaw", "Joystick", Joysticks, CJoystick.SetYaw);
            FlightAxisC("Joystick Roll", "Joystick", Joysticks, CJoystick.SetRoll);

            FlightAxis("Throttle", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Set);
            FlightButton("Throttle Increase", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Increase);
            FlightButton("Throttle Decrease", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Decrease);
            FlightAxis("Brakes/Airbrakes Axis", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);
            FlightButton("Brakes/Airbrakes", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);

            FlightButton("Flaps Cycle", "FlapsInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            FlightButton("Landing Gear Toggle", "GearInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Parking brake — exact interactable name from log
            FlightButton("Parking Brake Toggle", "viperBrakeLockInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Arrestor hook — exact name from log: "HookInteractable"
            FlightButton("Arrestor Hook Toggle", "HookInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            // Front hook variant also confirmed: "HookInteractable_front"
            FlightButton("Arrestor Hook Toggle (Front)", "HookInteractable_front", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Launch/cat bar — exact name from log: "CatHookInteractable"
            FlightButton("Launch Bar Toggle", "CatHookInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            FlightButton("Fire Weapon", "Joystick", Joysticks, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", Joysticks, CJoystick.MenuButton);
            FlightButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, s: -1, n: true);

            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
            // Exact log name: "viperlandingGLimitInteractable"
            AssistButton("G-Limiter Toggle", "viperlandingGLimitInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
        }

        protected override void CreateNavigationControls()
        {
            // Exact log names for all AP buttons
            NavButton("A/P Nav Mode", "navAPButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NavButton("A/P Spd Hold", "SpeedHoldButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NavButton("A/P Hdg Hold", "HDGHoldButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NavButton("A/P Alt Hold", "AltHoldButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NavButton("A/P Off", "offAPButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Clear waypoint — exact log name: "ClrWptButton"
            NavButton("Clear Waypoint", "ClrWptButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            NavAxisC("A/P Altitude", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Alt Increase", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Alt Decrease", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavButton("A/P Heading Set Right", "Course Right", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Heading Set Left", "Course Left", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            AddPostUpdateControl("Adjust AP Altitude");
        }

        protected override void CreateSystemsControls()
        {
            // Master Caution — exact log name: "MasterCautionInteractable"
            SystemsButton("Clear Cautions", "MasterCautionInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Master Arm — exact log name: "MasterArmInteractable"
            SystemsButton("Master Arm Toggle", "MasterArmInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Engine — exact log name: "leftEngineSwitchInteractable"
            SystemsButton("Engine Toggle", "leftEngineSwitchInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Engine switch cover — exact log name: "coverSwitchInteractable_leftEngine"
            SystemsButton("Engine Switch Cover Toggle", "coverSwitchInteractable_leftEngine", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // APU — exact log name: "apuSwitchInteractable"
            SystemsButton("APU Toggle", "apuSwitchInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // APU switch cover — exact log name: "coverSwitchInteractable_apuSwitch"
            SystemsButton("APU Switch Cover Toggle", "coverSwitchInteractable_apuSwitch", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Battery — exact log name: "mainBattSwitchInteractable"
            SystemsButton("Battery Toggle", "mainBattSwitchInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // JFS starter
            SystemsButton("JFS Off", "JFS Starter", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            SystemsButton("JFS Start 1", "JFS Starter", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("JFS Start 2", "JFS Starter", ByName<VRLever, CLever>, CLever.Set, s: 2, n: true);

            // Radar — exact log name: "viperRadarInteractable"
            SystemsButton("Radar Power Toggle", "viperRadarInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // RWR — two controls confirmed: "RearRWRPowerKnob" and "RWRButton"
            SystemsButton("RWR Power Knob Cycle", "RearRWRPowerKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
            SystemsButton("RWR Power Knob Next", "RearRWRPowerKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, n: true);
            SystemsButton("RWR Power Knob Prev", "RearRWRPowerKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, n: true);
            SystemsButton("RWR Button", "RWRButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            SystemsButton("TGP Power Toggle", "TGP Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("DED Power Toggle", "DED Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Fire Countermeasures", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.MenuButton);

            // Jettison — exact log name: "JettisonSwitchInteractable"
            SystemsButton("Jettison Execute", "JettisonSwitchInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            SystemsButton("Flares Toggle", "Toggle Flares", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Chaff Toggle", "Toggle Chaff", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Jammer Toggle", "Toggle Jammer", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Toggle Visor", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet NV Toggle", "Toggle NVG", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);

            // Exact log name: "hudPowerInteractable"
            HUDButton("HUD Power Toggle", "hudPowerInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Exact log name: "hmcsPowerInteractable"
            HUDButton("HMCS Power Toggle", "hmcsPowerInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // HUD tint knob: "HUDTintKnob" + roller "HUDTintRoller"
            HUDAxisC("HUD Tint", "HUDTintKnob", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            HUDButton("HUD Tint Up", "HUDTintKnob", ByName<VRTwistKnob, CKnob>, CKnob.Increase, n: true);
            HUDButton("HUD Tint Down", "HUDTintKnob", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, n: true);
            HUDButton("HUD Tint Roller Up", "HUDTintRoller", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // HUD brightness: "HUDBrightKnob" + roller "HUDRoller"
            HUDAxisC("HUD Brightness", "HUDBrightKnob", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            HUDButton("HUD Brightness Up", "HUDBrightKnob", ByName<VRTwistKnob, CKnob>, CKnob.Increase, n: true);
            HUDButton("HUD Brightness Down", "HUDBrightKnob", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, n: true);
            HUDButton("HUD Brightness Roller Up", "HUDRoller", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // HMCS brightness roller: "HMCSRoller"
            HUDButton("HMCS Brightness Roller Up", "HMCSRoller", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // ICP Master Modes — exact log names
            HUDButton("COM1", "COM1", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            HUDButton("COM2", "COM2", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            HUDButton("A/A Master Mode", "AirMasterMode", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            HUDButton("A/G Master Mode", "GroundMasterMode", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            AddPostUpdateControl("HUDTintKnob");
            AddPostUpdateControl("HUDBrightKnob");
        }

        protected override void CreateNumPadControls()
        {
            // Exact log names: "NumInteractable (1)" through "NumInteractable (0)"
            NumPadButton("1", "NumInteractable (1)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("2", "NumInteractable (2)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("3", "NumInteractable (3)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("4", "NumInteractable (4)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("5", "NumInteractable (5)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("6", "NumInteractable (6)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("7", "NumInteractable (7)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("8", "NumInteractable (8)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("9", "NumInteractable (9)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("0", "NumInteractable (0)", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("Decimal", "DecimalInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("Enter", "EnterInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NumPadButton("Clear", "ClrInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
        }

        protected override void CreateDisplayControls()
        {
            // Exact log names: "powButtonMMFDLeft", "powButtonMMFDRight"
            DisplayButton("MFD Left Power", "powButtonMMFDLeft", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            DisplayButton("MFD Right Power", "powButtonMMFDRight", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Swap MFDs — exact log name: "mfdSwapButton"
            DisplayButton("Swap MFDs", "mfdSwapButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // MFD Brightness knob + roller
            DisplayAxis("MFD Brightness", "MFDBrightnessKnob", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            DisplayButton("MFD Brightness Increase", "MFDBrightnessKnob", ByName<VRTwistKnob, CKnob>, CKnob.Increase, n: true);
            DisplayButton("MFD Brightness Decrease", "MFDBrightnessKnob", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, n: true);
            DisplayButton("MFD Brightness Roller Up", "MFDBrightnessRoller", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // SOI
            DisplayButton("SOI Slew Button", "SOI", SOI, CSOI.SlewButton);
            DisplayAxisC("SOI Slew X", "SOI", SOI, CSOI.SlewX);
            DisplayAxisC("SOI Slew Y", "SOI", SOI, CSOI.SlewY);
            DisplayButton("SOI Slew Up", "SOI", SOI, CSOI.SlewUp);
            DisplayButton("SOI Slew Right", "SOI", SOI, CSOI.SlewRight);
            DisplayButton("SOI Slew Down", "SOI", SOI, CSOI.SlewDown);
            DisplayButton("SOI Slew Left", "SOI", SOI, CSOI.SlewLeft);
            DisplayButton("SOI Next", "SOI", SOI, CSOI.Next);
            DisplayButton("SOI Prev", "SOI", SOI, CSOI.Prev);
            DisplayButton("SOI Zoom In", "SOI", SOI, CSOI.ZoomIn);
            DisplayButton("SOI Zoom Out", "SOI", SOI, CSOI.ZoomOut);

            // MFD1 = Left (i:0), MFD2 = Center (i:1) — log shows MFD1 and MFD2 interactables
            // MFD power interactables confirmed: "MFD1PowerInteractable", "MFD2PowerInteractable"
            DisplayButton("MFD Left Toggle", "MFD Left", MFD, CMFD.PowerToggle, i: 0);
            DisplayButton("MFD Left On", "MFD Left", MFD, CMFD.PowerOn, i: 0);
            DisplayButton("MFD Left Off", "MFD Left", MFD, CMFD.PowerOff, i: 0);
            DisplayButton("MFD Left L1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L1, i: 0);
            DisplayButton("MFD Left L2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L2, i: 0);
            DisplayButton("MFD Left L3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L3, i: 0);
            DisplayButton("MFD Left L4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L4, i: 0);
            DisplayButton("MFD Left L5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L5, i: 0);
            DisplayButton("MFD Left R1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R1, i: 0);
            DisplayButton("MFD Left R2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R2, i: 0);
            DisplayButton("MFD Left R3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R3, i: 0);
            DisplayButton("MFD Left R4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R4, i: 0);
            DisplayButton("MFD Left R5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R5, i: 0);
            DisplayButton("MFD Left T1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 0);
            DisplayButton("MFD Left T2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T2, i: 0);
            DisplayButton("MFD Left T3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T3, i: 0);
            DisplayButton("MFD Left T4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T4, i: 0);
            DisplayButton("MFD Left T5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T5, i: 0);
            DisplayButton("MFD Left B1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B1, i: 0);
            DisplayButton("MFD Left B2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B2, i: 0);
            DisplayButton("MFD Left B3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B3, i: 0);
            DisplayButton("MFD Left B4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B4, i: 0);
            DisplayButton("MFD Left B5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B5, i: 0);

            DisplayButton("MFD Center Toggle", "MFD Center", MFD, CMFD.PowerToggle, i: 1);
            DisplayButton("MFD Center On", "MFD Center", MFD, CMFD.PowerOn, i: 1);
            DisplayButton("MFD Center Off", "MFD Center", MFD, CMFD.PowerOff, i: 1);
            DisplayButton("MFD Center L1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L1, i: 1);
            DisplayButton("MFD Center L2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L2, i: 1);
            DisplayButton("MFD Center L3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L3, i: 1);
            DisplayButton("MFD Center L4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L4, i: 1);
            DisplayButton("MFD Center L5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L5, i: 1);
            DisplayButton("MFD Center R1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R1, i: 1);
            DisplayButton("MFD Center R2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R2, i: 1);
            DisplayButton("MFD Center R3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R3, i: 1);
            DisplayButton("MFD Center R4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R4, i: 1);
            DisplayButton("MFD Center R5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R5, i: 1);
            DisplayButton("MFD Center T1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 1);
            DisplayButton("MFD Center T2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T2, i: 1);
            DisplayButton("MFD Center T3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T3, i: 1);
            DisplayButton("MFD Center T4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T4, i: 1);
            DisplayButton("MFD Center T5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T5, i: 1);
            DisplayButton("MFD Center B1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B1, i: 1);
            DisplayButton("MFD Center B2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B2, i: 1);
            DisplayButton("MFD Center B3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B3, i: 1);
            DisplayButton("MFD Center B4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B4, i: 1);
            DisplayButton("MFD Center B5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B5, i: 1);

            DisplayButton("MFD Right Toggle", "MFD Right", MFD, CMFD.PowerToggle, i: 2);
            DisplayButton("MFD Right On", "MFD Right", MFD, CMFD.PowerOn, i: 2);
            DisplayButton("MFD Right Off", "MFD Right", MFD, CMFD.PowerOff, i: 2);
            DisplayButton("MFD Right L1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L1, i: 2);
            DisplayButton("MFD Right L2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L2, i: 2);
            DisplayButton("MFD Right L3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L3, i: 2);
            DisplayButton("MFD Right L4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L4, i: 2);
            DisplayButton("MFD Right L5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L5, i: 2);
            DisplayButton("MFD Right R1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R1, i: 2);
            DisplayButton("MFD Right R2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R2, i: 2);
            DisplayButton("MFD Right R3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R3, i: 2);
            DisplayButton("MFD Right R4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R4, i: 2);
            DisplayButton("MFD Right R5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R5, i: 2);
            DisplayButton("MFD Right T1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 2);
            DisplayButton("MFD Right T2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T2, i: 2);
            DisplayButton("MFD Right T3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T3, i: 2);
            DisplayButton("MFD Right T4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T4, i: 2);
            DisplayButton("MFD Right T5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T5, i: 2);
            DisplayButton("MFD Right B1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B1, i: 2);
            DisplayButton("MFD Right B2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B2, i: 2);
            DisplayButton("MFD Right B3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B3, i: 2);
            DisplayButton("MFD Right B4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B4, i: 2);
            DisplayButton("MFD Right B5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B5, i: 2);

            AddPostUpdateControl("MFDBrightnessKnob");
            AddPostUpdateControl("SOI");
            AddPostUpdateControl("MFD Left");
            AddPostUpdateControl("MFD Center");
            AddPostUpdateControl("MFD Right");
        }

        protected override void CreateRadioControls()
        {
            RadioButton("Radio Transmit", "Radio", ByType<CockpitTeamRadioManager, CRadio>, CRadio.Transmit, s: -1, n: true);

            RadioAxis("Radio Volume", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            RadioButton("Prev Song", "Prev Song", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            RadioButton("Next Song", "Next Song", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            RadioButton("Play/Pause", "Play/Pause", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            RadioButton("Radio Mode Cycle", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioButton("Radio Channel Cycle", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
        }

        protected override void CreateMusicControls() { }

        protected override void CreateLightsControls()
        {
            LightsButton("Instrument Lights Toggle", "Instrument Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
            LightsButton("Instrument Lights On", "Instrument Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 1, n: true);
            LightsButton("Instrument Lights Off", "Instrument Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 0, n: true);

            LightsAxis("Instrument Brightness", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);

            LightsButton("Nav Lights Toggle", "Nav Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
            LightsButton("Nav Lights On", "Nav Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 1, n: true);
            LightsButton("Nav Lights Off", "Nav Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 0, n: true);

            LightsButton("Strobe Lights Toggle", "Strobe Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
            LightsButton("Strobe Lights On", "Strobe Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 1, n: true);
            LightsButton("Strobe Lights Off", "Strobe Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 0, n: true);

            // Exact log name: "viperlandingLightInteractable"
            LightsButton("Landing Lights Toggle", "viperlandingLightInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
        }

        protected override void CreateMiscControls()
        {
            MiscButton("Canopy Toggle", "Canopy", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            // Exact log names: "raiseSeatInter", "lowerSeatInter"
            MiscButton("Raise Seat", "raiseSeatInter", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            MiscButton("Lower Seat", "lowerSeatInter", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Exact log names: "fuelButton", "fuelDrainButton"
            MiscButton("Fuel Port Toggle", "fuelButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            MiscButton("Fuel Dump Toggle", "fuelDrainButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            MiscButton("Fuel Dump Cover Toggle", "Switch Cover (Fuel Dump)", ByName<VRSwitchCover, CLeverCovered>, CLeverCovered.Cycle, s: -1, n: true);
        }
    }
}