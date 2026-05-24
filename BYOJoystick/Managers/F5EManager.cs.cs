using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using VTOLVR.Multiplayer;
using MFDButtons = MFD.MFDButtons;

namespace BYOJoystick.Managers
{
    public class F5EManager : Manager
    {
        public override string GameName => "F-5E";
        public override string ShortName => "F5E";
        public override bool IsMulticrew => false;

        private static string SideJoystick => "Local/SideStickObjects";
        private static string CenterJoystick => "Local/CenterStickObjects";

        private CJoystick Joysticks(string name, string root, bool nullable, bool checkName, int idx)
        {
            return GetJoysticksByPaths(name, SideJoystick, CenterJoystick);
        }

        protected override void PreMapping() { }

        protected override void CreateFlightControls()
        {
            FlightAxisC("Joystick Pitch", "Joystick", Joysticks, CJoystick.SetPitch);
            FlightAxisC("Joystick Yaw", "Joystick", Joysticks, CJoystick.SetYaw);
            FlightAxisC("Joystick Roll", "Joystick", Joysticks, CJoystick.SetRoll);

            FlightAxis("Throttle", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Set);
            FlightButton("Throttle Increase", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Increase);
            FlightButton("Throttle Decrease", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Decrease);
            FlightAxis("Brakes Axis", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);
            FlightButton("Brakes", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);

            FlightButton("Landing Gear Toggle", "Landing Gear", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Landing Gear Up", "Landing Gear", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Landing Gear Down", "Landing Gear", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Flaps Cycle", "Flaps", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Flaps Up", "Flaps", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            FlightButton("Flaps Down", "Flaps", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);

            FlightButton("Arrestor Hook Toggle", "Arrestor Hook", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Arrestor Hook Down", "Arrestor Hook", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Arrestor Hook Up", "Arrestor Hook", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Launch Bar Toggle", "Launch Bar", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Launch Bar Extend", "Launch Bar", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Launch Bar Retract", "Launch Bar", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Parking Brake Toggle", "Brake Locks", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Parking Brake On", "Brake Locks", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Parking Brake Off", "Brake Locks", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Fire Weapon", "Joystick", Joysticks, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", Joysticks, CJoystick.MenuButton);
            FlightButton("Fire Countermeasures", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.MenuButton);
            FlightButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, s: -1, n: true);

            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
            AssistButton("Toggle Flight Assist", "Toggle Flight Assist", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Flight Assist On", "Toggle Flight Assist", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            AssistButton("Flight Assist Off", "Toggle Flight Assist", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            AssistButton("Toggle Pitch Assist", "Toggle Pitch Assist", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Toggle Yaw Assist", "Toggle Yaw Assist", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Toggle Roll Assist", "Toggle Roll Assist", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Toggle G-Limiter", "Toggle G-Limiter", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Toggle Pitch Trim", "Toggle Pitch Trim", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Catapult TO Trim", "Catapult TO Trim", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateNavigationControls()
        {
            NavButton("A/P Nav Mode", "Navigation Mode", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Heading Hold", "Heading Hold", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Altitude Hold", "Altitude Hold", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Airspeed Hold", "Airspeed Hold", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Off", "All AP Off", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("Toggle Altitude Mode", "Toggle Altitude Mode", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("Clear Waypoint", "Clear Waypoint", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            NavAxisC("A/P Altitude", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Altitude Increase", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Altitude Decrease", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("A/P Speed", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Speed Increase", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Speed Decrease", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("A/P Heading", "AP Heading Set", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Heading Increase", "AP Heading Set", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Heading Decrease", "AP Heading Set", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("ILS Course", "Course Set", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("ILS Course Increase", "Course Set", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("ILS Course Decrease", "Course Set", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("Adjust AP Altitude");
            AddPostUpdateControl("Auto Speed Set");
            AddPostUpdateControl("AP Heading Set");
            AddPostUpdateControl("Course Set");
        }

        protected override void CreateSystemsControls()
        {
            SystemsButton("Clear Cautions", "Clear Cautions", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            SystemsButton("Master Arm Toggle", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Master Arm On", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Master Arm Off", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Left Engine Toggle", "Left Engine", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Left Engine On", "Left Engine", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Left Engine Off", "Left Engine", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Right Engine Toggle", "Right Engine", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Right Engine On", "Right Engine", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Right Engine Off", "Right Engine", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("APU Toggle", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("APU On", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("APU Off", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Battery Toggle", "Main Battery", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Battery On", "Main Battery", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Battery Off", "Main Battery", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // CKnobInt uses .Next / .Prev — NOT .Increase / .Decrease
            SystemsButton("Radar Power Cycle", "Radar Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("Radar Power Next", "Radar Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            SystemsButton("Radar Power Prev", "Radar Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            SystemsButton("RWR Mode Cycle", "RWR Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("RWR Mode Next", "RWR Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            SystemsButton("RWR Mode Prev", "RWR Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            SystemsButton("Toggle Chaff", "Toggle Chaff", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Toggle Flares", "Toggle Flares", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Fuel Port Toggle", "Fuel Port", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            SystemsButton("Jettison Execute", "Jettison", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Jettison All Toggle", "Jett All", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Jettison Empty Toggle", "Jett Empty", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Jettison Tanks Toggle", "Jett Tanks", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            SystemsButton("Raise Seat", "Raise Seat", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Lower Seat", "Lower Seat", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            SystemsButton("Swap MFDs", "Swap MFDs", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("MMFD Left Power", "Power MMFD Left", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("MMFD Right Power", "Power MMFD Right", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Toggle Visor", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet NVG Toggle", "Toggle NVG", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);

            HUDButton("HUD Power Toggle", "HUD Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            HUDButton("HUD Power On", "HUD Power", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            HUDButton("HUD Power Off", "HUD Power", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            HUDButton("HMCS Power Toggle", "HMCS Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            HUDButton("HMCS Power On", "HMCS Power", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            HUDButton("HMCS Power Off", "HMCS Power", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            HUDAxisC("HUD Brightness", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("HUD Brightness Up", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("HUD Brightness Down", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            HUDAxisC("MFD Brightness", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("MFD Brightness Up", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("MFD Brightness Down", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("HUD Brightness");
            AddPostUpdateControl("MFD Brightness");
        }

        protected override void CreateNumPadControls()
        {
            NumPadButton("1 (Swap Radio Freq)", "1 Swap Radio Frequency", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("2 (Set Standby Freq)", "2 Set Standby Frequency", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("3 (Set ILS Freq)", "3 Set ILS Frequency", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("4 (Set AP Altitude)", "4 Set AP Altitude", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("5 (Set AP Heading)", "5 Set AP Heading", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("6 (Set AP Speed)", "6 Set AP Speed", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("7 (Swap MFDs)", "7 Swap MFDs", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("8 (Set TGP Laser Code)", "8 Set TGP Laser Code", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("9 (Set Seeker Code)", "9 Set Seeker Code", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Enter", "Enter", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Clear", "Clear", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }

        protected override void CreateDisplayControls()
        {
            // F-5E MFDs are VRButton-based, not CMFD objects — no SOI grab needed
            DisplayButton("MFD1 Home", "Home", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 T2", "MFD1-Button-T2", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 T3", "MFD1-Button-T3", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Radar", "Radar", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Anti-Radar", "Anti-Radar Attack Display", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Navigation", "Navigation", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Objectives", "Objectives", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Ext Cameras", "External Cameras", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Targeting", "Targeting", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 GPS Targets", "GPS Targets", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 L1", "MFD1-Button-L1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Mounted Equip", "Mounted Equipment", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Options", "Options", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Communications", "Communications", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 CMS", "Countermeasure System", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 R1", "MFD1-Button-R1", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("MFD1 Power Cycle", "MFD Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
        }

        protected override void CreateRadioControls()
        {
            RadioButton("Radio PTT", "Radio", ByType<CockpitTeamRadioManager, CRadio>, CRadio.Transmit, s: -1, n: true);
            RadioButton("Radio Mode Next", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Mode Prev", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);
            RadioButton("Radio Channel Next", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Channel Prev", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);
        }

        protected override void CreateMusicControls()
        {
            MusicButton("Music Play/Pause", "PlayPause", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MusicButton("Music Next Song", "Next Song", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MusicButton("Music Prev Song", "Prev Song", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }

        protected override void CreateLightsControls()
        {
            // All confirmed VRLever from log
            LightsButton("Nav Lights Toggle", "Nav Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Nav Lights On", "Nav Lights", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            LightsButton("Nav Lights Off", "Nav Lights", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            LightsButton("Strobe Lights Toggle", "Strobe Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Strobe Lights On", "Strobe Lights", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            LightsButton("Strobe Lights Off", "Strobe Lights", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            LightsButton("Landing Lights Toggle", "Landing Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Interior Lights Toggle", "Interior Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Instrument Lights Toggle", "Instrument Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Beacon Light Toggle", "Beacon Light", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            LightsAxisC("Instrument Brightness", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            LightsButton("Instrument Bright Up", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            LightsButton("Instrument Bright Down", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("Instrument Brightness");
        }

        protected override void CreateMiscControls()
        {
            // Canopy is VRDoor at the handle (exterior), confirmed in log
            MiscButton("Canopy Toggle", "Canopy", ByType<VRDoor, CDoor>, CDoor.Toggle, s: -1, n: true);
            MiscButton("Canopy Open", "Canopy", ByType<VRDoor, CDoor>, CDoor.Open, s: -1, n: true);
            MiscButton("Canopy Close", "Canopy", ByType<VRDoor, CDoor>, CDoor.Close, s: -1, n: true);
        }
    }
}