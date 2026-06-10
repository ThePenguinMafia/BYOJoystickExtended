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
            FlightButton("Landing Gear Up", "GearInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Landing Gear Down", "GearInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // brake lock uses this ref name
            FlightButton("Parking Brake Toggle", "viperBrakeLockInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            FlightButton("Parking Brake On", "viperBrakeLockInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Parking Brake Off", "viperBrakeLockInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Arrestor Hook Toggle", "HookInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            FlightButton("Arrestor Hook Toggle (Front)", "HookInteractable_front", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            FlightButton("Arrestor Hook Down", "HookInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Arrestor Hook Up", "HookInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            FlightButton("Arrestor Hook Down (Front)", "HookInteractable_front", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Arrestor Hook Up (Front)", "HookInteractable_front", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Launch Bar Toggle", "CatHookInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            FlightButton("Launch Bar Extend", "CatHookInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Launch Bar Retract", "CatHookInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Fire Weapon", "Joystick", Joysticks, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", Joysticks, CJoystick.MenuButton);
            FlightButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, s: -1, n: true);

            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
            AssistButton("G-Limiter Toggle", "viperlandingGLimitInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
        }

        protected override void CreateNavigationControls()
        {
            NavButton("A/P Nav Mode", "navAPButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NavButton("A/P Spd Hold", "SpeedHoldButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NavButton("A/P Hdg Hold", "HDGHoldButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NavButton("A/P Alt Hold", "AltHoldButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            NavButton("A/P Off", "offAPButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

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
            SystemsButton("Clear Cautions", "Clear Cautions", ByName<VRButton, CButton>, CButton.Use, n: true);

            SystemsButton("Master Arm Toggle", "MasterArmInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            SystemsButton("Engine Toggle", "leftEngineSwitchInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            SystemsButton("Engine Switch Cover Toggle", "coverSwitchInteractable_leftEngine", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            SystemsButton("APU Toggle", "apuSwitchInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            SystemsButton("APU Switch Cover Toggle", "coverSwitchInteractable_apuSwitch", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            SystemsButton("Battery Toggle", "mainBattSwitchInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // jfs is a 3 position lever
            SystemsButton("JFS Off", "JFS Starter", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            SystemsButton("JFS Start 1", "JFS Starter", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("JFS Start 2", "JFS Starter", ByName<VRLever, CLever>, CLever.Set, s: 2, n: true);

            SystemsButton("Radar Power Toggle", "viperRadarInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // rwr has knob and push button
            SystemsButton("RWR Power Knob Cycle", "RearRWRPowerKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
            SystemsButton("RWR Power Knob Next", "RearRWRPowerKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, n: true);
            SystemsButton("RWR Power Knob Prev", "RearRWRPowerKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, n: true);
            SystemsButton("RWR Button", "RWRButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            SystemsButton("TGP Power Toggle", "TGP Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("DED Power Toggle", "DED Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Fire Countermeasures", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.MenuButton);

            SystemsButton("Jettison Execute", "JettisonSwitchInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            SystemsButton("Flares Toggle", "Toggle Flares", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Chaff Toggle", "Toggle Chaff", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Jammer Toggle", "Toggle Jammer", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Toggle Visor", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet NV Toggle", "Toggle NVG", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);

            HUDButton("HUD Power Toggle", "HUD Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            HUDButton("HMCS Power Toggle", "HMCS Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            HUDAxisC("HUD Tint", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            HUDButton("HUD Tint Up", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Increase, n: true);
            HUDButton("HUD Tint Down", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, n: true);
            HUDButton("HUD Tint Roller Up", "HUDTintRoller", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            HUDAxisC("HUD Brightness", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            HUDButton("HUD Brightness Up", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, n: true);
            HUDButton("HUD Brightness Down", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, n: true);
            HUDButton("HUD Brightness Roller Up", "HUDRoller", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            HUDButton("HMCS Brightness Roller Up", "HMCSRoller", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // icp master mode buttons
            HUDButton("COM1", "COM1", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            HUDButton("COM2", "COM2", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            HUDButton("A/A Master Mode", "AirMasterMode", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            HUDButton("A/G Master Mode", "GroundMasterMode", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            AddPostUpdateControl("HUD Tint");
            AddPostUpdateControl("HUD Brightness");
        }

        protected override void CreateNumPadControls()
        {
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
            DisplayButton("MFD Left Power", "powButtonMMFDLeft", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            DisplayButton("MFD Right Power", "powButtonMMFDRight", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            DisplayButton("Swap MFDs", "mfdSwapButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            DisplayAxis("MFD Brightness", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            DisplayButton("MFD Brightness Increase", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, n: true);
            DisplayButton("MFD Brightness Decrease", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, n: true);
            DisplayButton("MFD Brightness Roller Up", "MFDBrightnessRoller", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

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

            // mfd slots 0 1 2
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

            AddPostUpdateControl("MFD Brightness");
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

            LightsButton("Landing Lights Toggle", "viperlandingLightInteractable", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
        }

        protected override void CreateMiscControls()
        {
            MiscButton("Canopy Toggle", "Canopy", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            MiscButton("Raise Seat", "raiseSeatInter", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            MiscButton("Lower Seat", "lowerSeatInter", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            MiscButton("Fuel Port Toggle", "fuelButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            MiscButton("Fuel Dump Toggle", "fuelDrainButton", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            MiscButton("Fuel Dump Cover Toggle", "Switch Cover (Fuel Dump)", ByName<VRLever, CLeverCovered>, CLeverCovered.Cycle, s: -1, n: true);
        }
    }
}
