using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using VTOLVR.Multiplayer;

namespace BYOJoystick.Managers
{
    public class F5EManager : Manager
    {
        public override string GameName => "F-5E Tiger";
        public override string ShortName => "F5E";
        public override bool IsMulticrew => false;

        private static string SideJoystick => "Local/SideStickObjects";
        private static string CenterJoystick => "Local/CenterStickObjects";

        private CJoystick Joysticks(string name, string root, bool nullable, bool checkName, int idx)
        {
            return GetJoysticksByPaths(name, SideJoystick, CenterJoystick);
        }

        protected override void PreMapping()
        {
            LogInteractablesIfEnabled("F5E");
        }

        protected override void CreateFlightControls()
        {
            FlightAxisC("Joystick Pitch", "Joystick", Joysticks, CJoystick.SetPitch);
            FlightAxisC("Joystick Yaw", "Joystick", Joysticks, CJoystick.SetYaw);
            FlightAxisC("Joystick Roll", "Joystick", Joysticks, CJoystick.SetRoll);

            FlightAxis("Throttle", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Set);
            FlightButton("Throttle Increase", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Increase);
            FlightButton("Throttle Decrease", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Decrease);
            FlightAxis("Brakes Axis", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.TriggerAxis);
            FlightButton("Brakes", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);

            FlightButton("Landing Gear Toggle", "GearInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Landing Gear Up", "GearInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Landing Gear Down", "GearInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Flaps Cycle", "FlapsInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Flaps Up", "FlapsInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            FlightButton("Flaps Down", "FlapsInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);

            FlightButton("Arrestor Hook Toggle", "HookInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Arrestor Hook Down", "HookInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Arrestor Hook Up", "HookInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Launch Bar Toggle", "CatHookInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Launch Bar Extend", "CatHookInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Launch Bar Retract", "CatHookInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Parking Brake Toggle", "BrakeLockInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Parking Brake On", "BrakeLockInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Parking Brake Off", "BrakeLockInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Fire Weapon", "Joystick", Joysticks, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", Joysticks, CJoystick.MenuButton);
            FlightButton("Fire Countermeasures", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.MenuButton);
            FlightButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, s: -1, n: true);

            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
            AssistButton("Toggle Flight Assist", "AssistMasterInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Flight Assist On", "AssistMasterInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            AssistButton("Flight Assist Off", "AssistMasterInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            AssistButton("Toggle Pitch Assist", "PitchSASInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Toggle Yaw Assist", "YawSASInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Toggle Roll Assist", "RollSASInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            AssistButton("Toggle G-Limiter", "GLimitInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Toggle Pitch Trim", "PitchAutoTrimInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Catapult TO Trim", "CATOTrimInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateNavigationControls()
        {
            NavButton("A/P Nav Mode", "navAPButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Heading Hold", "headingAPButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Altitude Hold", "altitudeAPButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Airspeed Hold", "speedAPButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Off", "offAPButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("Toggle Altitude Mode", "AltitudeModeInteractable", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("Clear Waypoint", "ClrWptButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            NavAxisC("A/P Altitude", "APAltKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Altitude Increase", "APAltKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Altitude Decrease", "APAltKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("A/P Speed", "APSpeedAdjustInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Speed Increase", "APSpeedAdjustInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Speed Decrease", "APSpeedAdjustInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("A/P Heading", "APHeadingKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Heading Increase", "APHeadingKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Heading Decrease", "APHeadingKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("ILS Course", "CrsKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("ILS Course Increase", "CrsKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("ILS Course Decrease", "CrsKnobInteractable", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("APAltKnobInteractable");
            AddPostUpdateControl("APSpeedAdjustInteractable");
            AddPostUpdateControl("APHeadingKnobInteractable");
            AddPostUpdateControl("CrsKnobInteractable");
        }

        protected override void CreateSystemsControls()
        {
            SystemsButton("Clear Cautions", "MasterCautionInteractable", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // Cover switches confirmed in log
            SystemsButton("Master Arm Toggle", "coverSwitchInteractable_masterArm", ByName<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false, s: -1, n: true);
            SystemsButton("Master Arm On", "coverSwitchInteractable_masterArm", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 1, c: false, n: true);
            SystemsButton("Master Arm Off", "coverSwitchInteractable_masterArm", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 0, c: false, n: true);

            SystemsButton("Left Engine Toggle", "coverSwitchInteractable_leftEngine", ByName<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false, s: -1, n: true);
            SystemsButton("Left Engine On", "coverSwitchInteractable_leftEngine", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 1, c: false, n: true);
            SystemsButton("Left Engine Off", "coverSwitchInteractable_leftEngine", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 0, c: false, n: true);

            SystemsButton("Right Engine Toggle", "coverSwitchInteractable_rightEngine", ByName<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false, s: -1, n: true);
            SystemsButton("Right Engine On", "coverSwitchInteractable_rightEngine", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 1, c: false, n: true);
            SystemsButton("Right Engine Off", "coverSwitchInteractable_rightEngine", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 0, c: false, n: true);

            SystemsButton("APU Toggle", "coverSwitchInteractable_apuSwitch", ByName<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false, s: -1, n: true);
            SystemsButton("APU On", "coverSwitchInteractable_apuSwitch", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 1, c: false, n: true);
            SystemsButton("APU Off", "coverSwitchInteractable_apuSwitch", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 0, c: false, n: true);

            SystemsButton("Battery Toggle", "mainBattSwitchInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Battery On", "mainBattSwitchInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Battery Off", "mainBattSwitchInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Radar Power Cycle", "RadarPowerInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("Radar Power Next", "RadarPowerInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            SystemsButton("Radar Power Prev", "RadarPowerInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            SystemsButton("RWR Mode Cycle", "RWRModeInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("RWR Mode Next", "RWRModeInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            SystemsButton("RWR Mode Prev", "RWRModeInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            SystemsButton("Toggle Chaff", "ChaffInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Toggle Flares", "FlareInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            // Jettison — cover switch confirmed in log
            SystemsButton("Jettison Execute", "coverSwitchInteractable_jettisonButton", ByName<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false, s: -1, n: true);
            SystemsButton("Jettison All Toggle", "JettisonAllInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Jettison Empty Toggle", "JettisonEmptyInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Jettison Tanks Toggle", "JettisonTanksInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            SystemsButton("Fuel Port Toggle", "FuelPort", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Fuel Dump Toggle", "fuelDumpSwitchInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            SystemsButton("Raise Seat", "raiseSeatInter", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Lower Seat", "lowerSeatInter", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Swap MFDs", "mfdSwapButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("MMFD Left Power", "powButtonMMFDLeft", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("MMFD Right Power", "powButtonMMFDRight", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // Drag Chute — confirmed VRLever in log
            SystemsButton("Drag Chute Deploy", "Drag Chute", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Drag Chute Jettison", "Drag Chute", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            SystemsButton("Drag Chute Toggle", "Drag Chute", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Helmet", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet NVG Toggle", "Helmet", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);

            HUDButton("HUD Power Toggle", "hudPowerInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            HUDButton("HUD Power On", "hudPowerInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            HUDButton("HUD Power Off", "hudPowerInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            HUDButton("HMCS Power Toggle", "hmcsPowerInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            HUDButton("HMCS Power On", "hmcsPowerInteractable", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            HUDButton("HMCS Power Off", "hmcsPowerInteractable", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            HUDAxisC("HUD Brightness", "HUDBrightKnob", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("HUD Brightness Up", "HUDBrightKnob", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("HUD Brightness Down", "HUDBrightKnob", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            HUDAxisC("MFD Brightness", "MFDBrightnessKnob", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("MFD Brightness Up", "MFDBrightnessKnob", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("MFD Brightness Down", "MFDBrightnessKnob", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            HUDButton("HUD Declutter Cycle", "DeclutterKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            HUDButton("HUD Declutter Next", "DeclutterKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            HUDButton("HUD Declutter Prev", "DeclutterKnob", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            // SOI Slew
            HUDButton("SOI Slew Button", "SOI", SOI, CSOI.SlewButton);
            HUDAxisC("SOI Slew X", "SOI", SOI, CSOI.SlewX);
            HUDAxisC("SOI Slew Y", "SOI", SOI, CSOI.SlewY);
            HUDButton("SOI Slew Up", "SOI", SOI, CSOI.SlewUp);
            HUDButton("SOI Slew Right", "SOI", SOI, CSOI.SlewRight);
            HUDButton("SOI Slew Down", "SOI", SOI, CSOI.SlewDown);
            HUDButton("SOI Slew Left", "SOI", SOI, CSOI.SlewLeft);
            HUDButton("SOI Next", "SOI", SOI, CSOI.Next);
            HUDButton("SOI Prev", "SOI", SOI, CSOI.Prev);
            HUDButton("SOI Zoom In", "SOI", SOI, CSOI.ZoomIn);
            HUDButton("SOI Zoom Out", "SOI", SOI, CSOI.ZoomOut);

            AddPostUpdateControl("HUDBrightKnob");
            AddPostUpdateControl("MFDBrightnessKnob");
            AddPostUpdateControl("SOI");
        }

        protected override void CreateNumPadControls()
        {
            NumPadButton("1", "NumInteractable (1)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("2", "NumInteractable (2)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("3", "NumInteractable (3)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("4", "NumInteractable (4)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("5", "NumInteractable (5)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("6", "NumInteractable (6)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("7", "NumInteractable (7)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("8", "NumInteractable (8)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("9", "NumInteractable (9)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("0", "NumInteractable (0)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Enter", "EnterInteractable", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Clear", "ClrInteractable", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }

        protected override void CreateDisplayControls()
        {
            // MFD1 — all button names confirmed in log
            DisplayButton("MFD1 Power", "MFD1PowerInteractable", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 T1", "MFD1-Button-T1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 T2", "MFD1-Button-T2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 T3", "MFD1-Button-T3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 T4", "MFD1-Button-T4", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 T5", "MFD1-Button-T5", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 B1", "MFD1-Button-B1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 B2", "MFD1-Button-B2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 B3", "MFD1-Button-B3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 B4", "MFD1-Button-B4", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 B5", "MFD1-Button-B5", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 L1", "MFD1-Button-L1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 L2", "MFD1-Button-L2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 L3", "MFD1-Button-L3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 L4", "MFD1-Button-L4", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 L5", "MFD1-Button-L5", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 R1", "MFD1-Button-R1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 R2", "MFD1-Button-R2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 R3", "MFD1-Button-R3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 R4", "MFD1-Button-R4", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 R5", "MFD1-Button-R5", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // MFD2 — all button names confirmed in log
            DisplayButton("MFD2 Power", "MFD2PowerInteractable", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 T1", "MFD2-Button-T1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 T2", "MFD2-Button-T2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 T3", "MFD2-Button-T3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 T4", "MFD2-Button-T4", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 T5", "MFD2-Button-T5", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 B1", "MFD2-Button-B1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 B2", "MFD2-Button-B2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 B3", "MFD2-Button-B3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 B4", "MFD2-Button-B4", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 B5", "MFD2-Button-B5", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 L1", "MFD2-Button-L1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 L2", "MFD2-Button-L2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 L3", "MFD2-Button-L3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 L4", "MFD2-Button-L4", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 L5", "MFD2-Button-L5", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 R1", "MFD2-Button-R1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 R2", "MFD2-Button-R2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 R3", "MFD2-Button-R3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 R4", "MFD2-Button-R4", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD2 R5", "MFD2-Button-R5", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }

        protected override void CreateRadioControls()
        {
            RadioButton("Radio Transmit", "Radio", ByType<CockpitTeamRadioManager, CRadio>, CRadio.Transmit, s: -1, n: true);

            RadioButton("Radio Channel Cycle", "RadioChannelInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioButton("Radio Channel Next", "RadioChannelInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Channel Prev", "RadioChannelInteractable", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            RadioAxisC("Comms Volume", "CommsVolumeKnob", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            RadioButton("Comms Volume Up", "CommsVolumeKnob", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            RadioButton("Comms Volume Down", "CommsVolumeKnob", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("CommsVolumeKnob");
        }

        protected override void CreateMusicControls()
        {
            MusicButton("Music Play/Pause", "radioPlayPauseInteractable", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MusicButton("Music Next", "NextButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MusicButton("Music Prev", "PrevButton", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            MusicAxisC("Radio Volume", "RadioVolumeKnob", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            MusicButton("Radio Volume Up", "RadioVolumeKnob", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            MusicButton("Radio Volume Down", "RadioVolumeKnob", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("RadioVolumeKnob");
        }

        protected override void CreateLightsControls()
        {
            LightsButton("Nav Lights Toggle", "NavLightInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Strobe Lights Toggle", "StrobLightInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Landing Lights Toggle", "LandingLightInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Formation Lights Toggle", "FormLightInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Interior Lights Toggle", "InteriorLightInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            // Note: "InsturmentLightInteractable" — typo is in the game, must match exactly
            LightsButton("Instrument Lights Toggle", "InsturmentLightInteractable", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateMiscControls() { }
    }
}