using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using VTOLVR.Multiplayer;

namespace BYOJoystick.Managers
{
    public class AV8BManager : Manager
    {
        public override string GameName    => "AV-8B Harrier";
        public override string ShortName   => "AV8B";
        public override bool   IsMulticrew => false;

        private static string Dash => "Local/DashCanvas";

        private CJoystick CockpitJoystick(string name, string root, bool nullable, bool checkName, int idx)
        {
            if (TryGetExistingControl<CJoystick>(name, out var existingControl))
                return existingControl;

            var sticks = VehicleControlManifest.joysticks;
            VRJoystick side   = sticks != null && sticks.Length > 0 ? sticks[0] : null;
            VRJoystick center = sticks != null && sticks.Length > 1 ? sticks[1] : null;
            if (side == null && center == null)
            {
                if (nullable)
                    return null;
                throw new System.InvalidOperationException("No joysticks found in VehicleControlManifest.");
            }

            var control = new CJoystick(Vehicle, side ?? center, side != null ? center : null, IsMulticrew, true);
            Controls.Add(name, control);
            return control;
        }

        protected override void CreateFlightControls()
        {
            FlightAxisC("Joystick Pitch", "Joystick", CockpitJoystick, CJoystick.SetPitch);
            FlightAxisC("Joystick Yaw", "Joystick", CockpitJoystick, CJoystick.SetYaw);
            FlightAxisC("Joystick Roll", "Joystick", CockpitJoystick, CJoystick.SetRoll);

            FlightAxis("Throttle", "Throttle", ThrottleTilt, CThrottle.Set);
            FlightButton("Throttle Increase", "Throttle", ThrottleTilt, CThrottle.Increase);
            FlightButton("Throttle Decrease", "Throttle", ThrottleTilt, CThrottle.Decrease);

            FlightAxis("Nozzle", "Throttle", ThrottleTilt, CThrottleTilt.SetTiltTarget);
            FlightButton("Nozzle Forward", "Throttle", ThrottleTilt, CThrottleTilt.TiltUp);
            FlightButton("Nozzle Aft", "Throttle", ThrottleTilt, CThrottleTilt.TiltDown);

            FlightAxis("Brakes/Airbrakes Axis", "Throttle", ThrottleTilt, CThrottle.TriggerAxis);
            FlightButton("Brakes/Airbrakes", "Throttle", ThrottleTilt, CThrottle.Trigger);

            FlightButton("Landing Gear Toggle", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Cycle, i: 11);
            FlightButton("Landing Gear Up", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 11);
            FlightButton("Landing Gear Down", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 11);

            FlightButton("Flaps Cycle", "Flaps", ByManifest<VRLever, CLever>, CLever.Cycle, i: 9);
            FlightButton("Flaps Up", "Flaps", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 9);
            FlightButton("Flaps Down", "Flaps", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 9);

            FlightButton("Flap Mode Cycle", "Flap Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            FlightButton("Flap Mode Prev", "Flap Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);
            FlightButton("Flap Mode Next", "Flap Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);

            FlightButton("Parking Brake Toggle", "Brake Lock", ByManifest<VRLever, CLever>, CLever.Cycle, i: 10);
            FlightButton("Parking Brake On", "Brake Lock", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 10);
            FlightButton("Parking Brake Off", "Brake Lock", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 10);

            FlightButton("Water Injection Toggle", "Water Injection", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Cycle, i: 13);
            FlightButton("Water Injection On", "Water Injection", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 1, i: 13);
            FlightButton("Water Injection Off", "Water Injection", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 0, i: 13);

            FlightButton("Fire Weapon", "Joystick", CockpitJoystick, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", CockpitJoystick, CJoystick.MenuButton);
            FlightButton("Fire Countermeasures", "Throttle", ThrottleTilt, CThrottle.MenuButton);
            FlightButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, s: -1, n: true);

            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
            AssistButton("Master Toggle", "Toggle Flight Assist", ByManifest<VRLever, CLever>, CLever.Cycle, i: 23);
            AssistButton("Master On", "Toggle Flight Assist", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 23);
            AssistButton("Master Off", "Toggle Flight Assist", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 23);

            AssistButton("SAS Roll Toggle", "Toggle Roll Assist", ByManifest<VRLever, CLever>, CLever.Cycle, i: 22);
            AssistButton("SAS Roll On", "Toggle Roll Assist", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 22);
            AssistButton("SAS Roll Off", "Toggle Roll Assist", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 22);

            AssistButton("SAS Yaw Toggle", "Toggle Yaw Assist", ByManifest<VRLever, CLever>, CLever.Cycle, i: 21);
            AssistButton("SAS Yaw On", "Toggle Yaw Assist", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 21);
            AssistButton("SAS Yaw Off", "Toggle Yaw Assist", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 21);

            AssistButton("SAS Pitch Toggle", "Toggle Pitch Assist", ByManifest<VRLever, CLever>, CLever.Cycle, i: 20);
            AssistButton("SAS Pitch On", "Toggle Pitch Assist", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 20);
            AssistButton("SAS Pitch Off", "Toggle Pitch Assist", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 20);

            AssistButton("G-Limiter Toggle", "Toggle G-Limiter", ByManifest<VRLever, CLever>, CLever.Cycle, i: 24);
            AssistButton("G-Limiter On", "Toggle G-Limiter", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 24);
            AssistButton("G-Limiter Off", "Toggle G-Limiter", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 24);

            AssistButton("Auto Pitch Trim Toggle", "Toggle Pitch Trim", ByManifest<VRLever, CLever>, CLever.Cycle, i: 19);
            AssistButton("Auto Pitch Trim On", "Toggle Pitch Trim", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 19);
            AssistButton("Auto Pitch Trim Off", "Toggle Pitch Trim", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 19);
        }

        protected override void CreateNavigationControls()
        {
            NavButton("A/P Nav Mode", "Navigation Mode", ByManifest<VRButton, CButton>, CButton.Use, i: 21);
            NavButton("A/P Hover Hold", "Hover Autopilot", ByManifest<VRButton, CButton>, CButton.Use, i: 22);
            NavButton("A/P Off", "All AP Off", ByManifest<VRButton, CButton>, CButton.Use, i: 23);
            NavButton("A/P Alt Hold", "Altitude Hold", ByManifest<VRButton, CButton>, CButton.Use, i: 24);
            NavButton("A/P Hdg Hold", "Heading Hold", ByManifest<VRButton, CButton>, CButton.Use, i: 25);
            NavButton("A/P Spd Hold", "Speed Hold", ByManifest<VRButton, CButton>, CButton.Use, i: 26);

            NavButton("Altimeter Mode Toggle", "Altimeter Mode", ByManifest<VRLever, CLever>, CLever.Cycle, i: 1);
            NavButton("Clear Waypoint", "Clear Waypoint", ByManifest<VRButton, CButton>, CButton.Use, i: 19);
        }

        protected override void CreateSystemsControls()
        {
            SystemsButton("Clear Cautions", "Clear Cautions", ByManifest<VRButton, CButton>, CButton.Use, i: 20);

            SystemsButton("Master Arm Toggle", "Master Arm Switch", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Cycle, i: 8);
            SystemsButton("Master Arm On", "Master Arm Switch", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 1, i: 8);
            SystemsButton("Master Arm Off", "Master Arm Switch", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 0, i: 8);

            SystemsButton("Engine Toggle", "Engine", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Cycle, i: 15);
            SystemsButton("Engine On", "Engine", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 1, i: 15);
            SystemsButton("Engine Off", "Engine", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 0, i: 15);

            SystemsButton("Main Battery Toggle", "Main Battery", ByManifest<VRLever, CLever>, CLever.Cycle, i: 17);
            SystemsButton("Main Battery On", "Main Battery", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 17);
            SystemsButton("Main Battery Off", "Main Battery", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 17);

            SystemsButton("APU Toggle", "Auxilliary Power", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Cycle, i: 18);
            SystemsButton("APU On", "Auxilliary Power", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 1, i: 18);
            SystemsButton("APU Off", "Auxilliary Power", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 0, i: 18);

            SystemsButton("Chaff Toggle", "Toggle Chaff", ByManifest<VRLever, CLever>, CLever.Cycle, i: 4);
            SystemsButton("Chaff On", "Toggle Chaff", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 4);
            SystemsButton("Chaff Off", "Toggle Chaff", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 4);

            SystemsButton("Flares Toggle", "Toggle Flares", ByManifest<VRLever, CLever>, CLever.Cycle, i: 5);
            SystemsButton("Flares On", "Toggle Flares", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 5);
            SystemsButton("Flares Off", "Toggle Flares", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 5);

            SystemsButton("Flare Salvo", "Flare Salvo", ByManifest<VRButton, CButton>, CButton.Use, i: 47);

            SystemsButton("RWR Mode Cycle", "RWR Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("RWR Mode Prev", "RWR Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);
            SystemsButton("RWR Mode Next", "RWR Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);

            SystemsButton("RCS Mode Cycle", "RCS Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("RCS Mode Prev", "RCS Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);
            SystemsButton("RCS Mode Next", "RCS Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);

            SystemsButton("Fuel Tank Cycle", "Fuel Tank", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("Fuel Tank Prev", "Fuel Tank", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);
            SystemsButton("Fuel Tank Next", "Fuel Tank", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);

            SystemsButton("Refuel Probe Toggle", "Refuel Probe", ByManifest<VRLever, CLever>, CLever.Cycle, i: 27);
            SystemsButton("Refuel Probe Extend", "Refuel Probe", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 27);
            SystemsButton("Refuel Probe Retract", "Refuel Probe", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 27);

            SystemsButton("Fuel Dump Toggle", "Fuel Dump Switch", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Cycle, i: 25);
            SystemsButton("Fuel Dump On", "Fuel Dump Switch", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 1, i: 25);
            SystemsButton("Fuel Dump Off", "Fuel Dump Switch", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, s: 0, i: 25);

            SystemsButton("Jettison Execute", "coverSwitchInteractable_jettisonButton", ByName<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false, s: -1, n: true);
            SystemsButton("Jettison Selected", "Jettison Selected", ByManifest<VRButton, CButton>, CButton.Use, i: 57);
            SystemsButton("Jettison All Pylons", "Jettison All Pylons", ByManifest<VRButton, CButton>, CButton.Use, i: 58);
            SystemsButton("Jettison All", "JettisonAll", ByManifest<VRButton, CButton>, CButton.Use, i: 60);
            SystemsButton("Jettison Empty", "Jettison Empty", ByManifest<VRButton, CButton>, CButton.Use, i: 61);
            SystemsButton("Clear Jettison Marks", "Clear Jettison Marks", ByManifest<VRButton, CButton>, CButton.Use, i: 62);

            SystemsButton("Select HP1", "Select HP1", ByManifest<VRButton, CButton>, CButton.Use, i: 52);
            SystemsButton("Select HP2", "Select HP2", ByManifest<VRButton, CButton>, CButton.Use, i: 53);
            SystemsButton("Select HP3", "Select HP3", ByManifest<VRButton, CButton>, CButton.Use, i: 51);
            SystemsButton("Select HP4", "Select HP4", ByManifest<VRButton, CButton>, CButton.Use, i: 54);
            SystemsButton("Select HP5", "Select HP5", ByManifest<VRButton, CButton>, CButton.Use, i: 50);
            SystemsButton("Select HP6", "Select HP6", ByManifest<VRButton, CButton>, CButton.Use, i: 55);
            SystemsButton("Select HP7", "Select HP7", ByManifest<VRButton, CButton>, CButton.Use, i: 49);
            SystemsButton("Select HP8", "Select HP8", ByManifest<VRButton, CButton>, CButton.Use, i: 56);
            SystemsButton("Select HP9", "Select HP9", ByManifest<VRButton, CButton>, CButton.Use, i: 48);
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Helmet", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet Visor Open", "Helmet", HelmetController, CHelmet.OpenVisor, s: -1, n: true);
            HUDButton("Helmet Visor Closed", "Helmet", HelmetController, CHelmet.CloseVisor, s: -1, n: true);
            HUDButton("Helmet NV Toggle", "Helmet", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);
            HUDButton("Helmet NV On", "Helmet", HelmetController, CHelmet.EnableNightVision, s: -1, n: true);
            HUDButton("Helmet NV Off", "Helmet", HelmetController, CHelmet.DisableNightVision, s: -1, n: true);

            HUDButton("HUD Power Toggle", "HUD Power", ByManifest<VRLever, CLever>, CLever.Cycle, i: 2);
            HUDButton("HUD Power On", "HUD Power", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 2);
            HUDButton("HUD Power Off", "HUD Power", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 2);

            HUDButton("HMCS Power Toggle", "HMCS Power", ByManifest<VRLever, CLever>, CLever.Cycle, i: 3);
            HUDButton("HMCS Power On", "HMCS Power", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 3);
            HUDButton("HMCS Power Off", "HMCS Power", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 3);

            HUDAxisC("HUD Brightness", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("HUD Brightness Up", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("HUD Brightness Down", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            HUDButton("HUD Declutter Cycle", "HUD Declutter", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            HUDButton("HUD Declutter Next", "HUD Declutter", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            HUDButton("HUD Declutter Prev", "HUD Declutter", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

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

            AddPostUpdateControl("HUD Brightness");
            AddPostUpdateControl("SOI");
        }

        protected override void CreateNumPadControls()
        {
            NumPadButton("1", "1 / Swap Radio Frequency", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("2", "2 / Set Standby Frequency", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("3", "3 / Set ILS Frequency", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("4", "4 / Set AP Altitude", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("5", "5 / Set AP Heading", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("6", "6 / Set AP Speed", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("7", "7 / Swap MFDs", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("8", "8 / Set TGP Laser Code", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("9", "9 / Set Seeker Code", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("0", "0", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("Enter", "ENTER", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("Clear", "Clear", ByName<VRButton, CButton>, CButton.Use, r: Dash);
        }

        protected override void CreateDisplayControls()
        {
            DisplayAxisC("MFD Brightness", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            DisplayButton("MFD Brightness Up", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            DisplayButton("MFD Brightness Down", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            DisplayButton("MFD Left Power Cycle", "MFD Power (Left)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            DisplayButton("MFD Left Power On", "MFD Power (Left)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, s: 1, n: true);
            DisplayButton("MFD Left Power Off", "MFD Power (Left)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, s: 0, n: true);

            DisplayButton("MFD Right Power Cycle", "MFD Power (Right)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            DisplayButton("MFD Right Power On", "MFD Power (Right)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, s: 1, n: true);
            DisplayButton("MFD Right Power Off", "MFD Power (Right)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, s: 0, n: true);

            DisplayButton("MMFD Left Power", "RWR Power", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            DisplayButton("MFD1 T1", "MFD1 T1", ByManifest<VRButton, CButton>, CButton.Use, i: 27);
            DisplayButton("MFD1 T2", "MFD1 T2", ByManifest<VRButton, CButton>, CButton.Use, i: 28);
            DisplayButton("MFD1 T3", "MFD1 T3", ByManifest<VRButton, CButton>, CButton.Use, i: 29);
            DisplayButton("MFD1 T4", "MFD1 T4", ByManifest<VRButton, CButton>, CButton.Use, i: 30);
            DisplayButton("MFD1 T5", "MFD1 T5", ByManifest<VRButton, CButton>, CButton.Use, i: 31);
            DisplayButton("MFD1 B1", "MFD1 B1", ByManifest<VRButton, CButton>, CButton.Use, i: 32);
            DisplayButton("MFD1 B2", "MFD1 B2", ByManifest<VRButton, CButton>, CButton.Use, i: 33);
            DisplayButton("MFD1 B3", "MFD1 B3", ByManifest<VRButton, CButton>, CButton.Use, i: 34);
            DisplayButton("MFD1 B4", "MFD1 B4", ByManifest<VRButton, CButton>, CButton.Use, i: 35);
            DisplayButton("MFD1 B5", "MFD1 B5", ByManifest<VRButton, CButton>, CButton.Use, i: 36);
            DisplayButton("MFD1 L1", "MFD1 L1", ByManifest<VRButton, CButton>, CButton.Use, i: 37);
            DisplayButton("MFD1 L2", "MFD1 L2", ByManifest<VRButton, CButton>, CButton.Use, i: 38);
            DisplayButton("MFD1 L3", "MFD1 L3", ByManifest<VRButton, CButton>, CButton.Use, i: 39);
            DisplayButton("MFD1 L4", "MFD1 L4", ByManifest<VRButton, CButton>, CButton.Use, i: 40);
            DisplayButton("MFD1 L5", "MFD1 L5", ByManifest<VRButton, CButton>, CButton.Use, i: 41);
            DisplayButton("MFD1 R1", "MFD1 R1", ByManifest<VRButton, CButton>, CButton.Use, i: 42);
            DisplayButton("MFD1 R2", "MFD1 R2", ByManifest<VRButton, CButton>, CButton.Use, i: 43);
            DisplayButton("MFD1 R3", "MFD1 R3", ByManifest<VRButton, CButton>, CButton.Use, i: 44);
            DisplayButton("MFD1 R4", "MFD1 R4", ByManifest<VRButton, CButton>, CButton.Use, i: 45);
            DisplayButton("MFD1 R5", "MFD1 R5", ByManifest<VRButton, CButton>, CButton.Use, i: 46);

            DisplayButton("MFD2 T1", "MFD2 T1", ByManifest<VRButton, CButton>, CButton.Use, i: 65);
            DisplayButton("MFD2 T2", "MFD2 T2", ByManifest<VRButton, CButton>, CButton.Use, i: 66);
            DisplayButton("MFD2 T3", "MFD2 T3", ByManifest<VRButton, CButton>, CButton.Use, i: 67);
            DisplayButton("MFD2 T4", "MFD2 T4", ByManifest<VRButton, CButton>, CButton.Use, i: 68);
            DisplayButton("MFD2 T5", "MFD2 T5", ByManifest<VRButton, CButton>, CButton.Use, i: 69);
            DisplayButton("MFD2 B1", "MFD2 B1", ByManifest<VRButton, CButton>, CButton.Use, i: 70);
            DisplayButton("MFD2 B2", "MFD2 B2", ByManifest<VRButton, CButton>, CButton.Use, i: 71);
            DisplayButton("MFD2 B3", "MFD2 B3", ByManifest<VRButton, CButton>, CButton.Use, i: 72);
            DisplayButton("MFD2 B4", "MFD2 B4", ByManifest<VRButton, CButton>, CButton.Use, i: 73);
            DisplayButton("MFD2 B5", "MFD2 B5", ByManifest<VRButton, CButton>, CButton.Use, i: 74);
            DisplayButton("MFD2 L1", "MFD2 L1", ByManifest<VRButton, CButton>, CButton.Use, i: 75);
            DisplayButton("MFD2 L2", "MFD2 L2", ByManifest<VRButton, CButton>, CButton.Use, i: 76);
            DisplayButton("MFD2 L3", "MFD2 L3", ByManifest<VRButton, CButton>, CButton.Use, i: 77);
            DisplayButton("MFD2 L4", "MFD2 L4", ByManifest<VRButton, CButton>, CButton.Use, i: 78);
            DisplayButton("MFD2 L5", "MFD2 L5", ByManifest<VRButton, CButton>, CButton.Use, i: 79);
            DisplayButton("MFD2 R1", "MFD2 R1", ByManifest<VRButton, CButton>, CButton.Use, i: 80);
            DisplayButton("MFD2 R2", "MFD2 R2", ByManifest<VRButton, CButton>, CButton.Use, i: 81);
            DisplayButton("MFD2 R3", "MFD2 R3", ByManifest<VRButton, CButton>, CButton.Use, i: 82);
            DisplayButton("MFD2 R4", "MFD2 R4", ByManifest<VRButton, CButton>, CButton.Use, i: 83);
            DisplayButton("MFD2 R5", "MFD2 R5", ByManifest<VRButton, CButton>, CButton.Use, i: 84);

            AddPostUpdateControl("MFD Brightness");
        }

        protected override void CreateRadioControls()
        {
            RadioButton("Radio Transmit", "Radio", ByType<CockpitTeamRadioManager, CRadio>, CRadio.Transmit, s: -1, n: true);

            RadioButton("Radio Channel Cycle", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioButton("Radio Channel Next", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Channel Prev", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            RadioButton("Radio Mode Cycle", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioButton("Radio Mode Next", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Mode Prev", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            RadioAxisC("Comms Volume", "Comm Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            RadioButton("Comms Volume Up", "Comm Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            RadioButton("Comms Volume Down", "Comm Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("Comm Radio Volume");
        }

        protected override void CreateMusicControls()
        {
            MusicButton("Music Play/Pause", "Play/Pause", ByManifest<VRButton, CButton>, CButton.Use, i: 88);
            MusicButton("Music Next", "Next Song", ByManifest<VRButton, CButton>, CButton.Use, i: 87);
            MusicButton("Music Prev", "Prev Song", ByManifest<VRButton, CButton>, CButton.Use, i: 86);

            MusicAxisC("Radio Volume", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            MusicButton("Radio Volume Up", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            MusicButton("Radio Volume Down", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("Radio Volume");
        }

        protected override void CreateLightsControls()
        {
            LightsButton("Interior Lights Toggle", "Internal Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 6);
            LightsButton("Interior Lights On", "Internal Lights", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 6);
            LightsButton("Interior Lights Off", "Internal Lights", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 6);

            LightsButton("Instrument Lights Toggle", "InstrumentLights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 7);
            LightsButton("Instrument Lights On", "InstrumentLights", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 7);
            LightsButton("Instrument Lights Off", "InstrumentLights", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 7);

            LightsAxisC("Instrument Brightness", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            LightsButton("Instrument Brightness Up", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            LightsButton("Instrument Brightness Down", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            LightsButton("Nav Lights Toggle", "Nav Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 30);
            LightsButton("Nav Lights On", "Nav Lights", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 30);
            LightsButton("Nav Lights Off", "Nav Lights", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 30);

            LightsButton("Formation Lights Toggle", "Formation Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 29);
            LightsButton("Formation Lights On", "Formation Lights", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 29);
            LightsButton("Formation Lights Off", "Formation Lights", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 29);

            LightsButton("Strobe Lights Toggle", "Strobe Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 28);
            LightsButton("Strobe Lights On", "Strobe Lights", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 28);
            LightsButton("Strobe Lights Off", "Strobe Lights", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 28);

            LightsButton("Landing Light Toggle", "Landing Light", ByManifest<VRLever, CLever>, CLever.Cycle, i: 12);
            LightsButton("Landing Light On", "Landing Light", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 12);
            LightsButton("Landing Light Off", "Landing Light", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 12);

            AddPostUpdateControl("Instrument Brightness");
        }

        protected override void CreateMiscControls()
        {
            MiscButton("Canopy Toggle", "Canopy", ByManifest<VRLever, CLever>, CLever.Cycle, i: 0);
            MiscButton("Canopy Open", "Canopy", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 0);
            MiscButton("Canopy Close", "Canopy", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 0);

            MiscButton("Raise Seat", "Raise Seat", ByManifest<VRButton, CButton>, CButton.Use, i: 0);
            MiscButton("Lower Seat", "Lower Seat", ByManifest<VRButton, CButton>, CButton.Use, i: 1);
        }
    }
}
