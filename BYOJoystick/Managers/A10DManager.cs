using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using VTOLVR.Multiplayer;
using MFDButtons = MFD.MFDButtons;

namespace BYOJoystick.Managers
{
    public class A10DManager : Manager
    {
        public override string GameName    => "A-10D";
        public override string ShortName   => "A10D";
        public override bool   IsMulticrew => false;

        // Common cockpit paths - may need adjustment after diagnostic dump
        private static string Dash         => "Local/DashCanvas";
        private static string SideJoystick => "Local/SideStickObjects";
        private static string CenterStick  => "Local/CenterStickObjects";
        private static string ThrottleRoot => "Local/Throttle";

        private CJoystick Joysticks(string name, string root, bool nullable, bool checkName, int idx)
        {
            return GetJoysticksByPaths(name, SideJoystick, CenterStick);
        }
        protected override void CreateFlightControls()
        {
            // Axes
            FlightAxisC("Joystick Pitch", "Joystick", Joysticks, CJoystick.SetPitch);
            FlightAxisC("Joystick Yaw", "Joystick", Joysticks, CJoystick.SetYaw);
            FlightAxisC("Joystick Roll", "Joystick", Joysticks, CJoystick.SetRoll);

            // Throttle (A-10 typically has single throttle)
            FlightAxis("Throttle", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Set);
            FlightButton("Throttle Increase", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Increase);
            FlightButton("Throttle Decrease", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Decrease);

            // Brakes
            FlightAxis("Brakes/Airbrakes Axis", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.TriggerAxis);
            FlightButton("Brakes/Airbrakes", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);

            // Flaps
            FlightButton("Flaps Cycle", "Flaps", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Flaps 0", "Flaps", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 9);
            FlightButton("Flaps 1", "Flaps", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 9);
            FlightButton("Flaps 2", "Flaps", ByManifest<VRLever, CLever>, CLever.Set, 2, i: 9);

            // Gear
            FlightButton("Landing Gear Toggle", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Cycle, i: 13);
            FlightButton("Landing Gear Up", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 13);
            FlightButton("Landing Gear Down", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 13);

            // Brake locks
            FlightButton("Brake Lock Toggle", "Brake Locks", ByManifest<VRLever, CLever>, CLever.Cycle, i: 4);
            FlightButton("Brake Lock On", "Brake Locks", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 4);
            FlightButton("Brake Lock Off", "Brake Locks", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 4);

            // Weapons
            FlightButton("Fire Weapon", "Joystick", Joysticks, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", Joysticks, CJoystick.MenuButton);
            
            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
            AssistButton("Master Toggle", "Toggle Flight Assist", ByManifest<VRLever, CLever>, CLever.Cycle, i: 2);
            AssistButton("Master On", "Toggle Flight Assist", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 2);
            AssistButton("Master Off", "Toggle Flight Assist", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 2);

            AssistButton("Pitch Assist Toggle", "Toggle Pitch Assist", ByManifest<VRLever, CLever>, CLever.Cycle, i: 26);
            AssistButton("Yaw Assist Toggle", "Toggle Yaw Assist", ByManifest<VRLever, CLever>, CLever.Cycle, i: 32);
            AssistButton("Roll Assist Toggle", "Toggle Roll Assist", ByManifest<VRLever, CLever>, CLever.Cycle, i: 28);

            // Pitch trim mapping: search for the more specific token to avoid matching generic 'Toggle' controls
            AssistButton("Toggle Pitch Trim", "Pitch Trim", ByNamePartial<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            AssistButton("Toggle G-Limiter", "Toggle G-Limiter", ByManifest<VRLever, CLever>, CLever.Cycle, i: 14);
            AssistButton("G-Limiter On", "Toggle G-Limiter", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 14);
            AssistButton("G-Limiter Off", "Toggle G-Limiter", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 14);
        }

        protected override void CreateNavigationControls()
        {
            NavButton("A/P Nav Mode", "Navigation Mode", ByManifest<VRButton, CButton>, CButton.Use, i: 68);
            NavButton("A/P Spd Hold", "Airspeed Hold", ByManifest<VRButton, CButton>, CButton.Use, i: 89);
            NavButton("A/P Hdg Hold", "Heading Hold", ByManifest<VRButton, CButton>, CButton.Use, i: 18);
            NavButton("A/P Alt Hold", "Altitude Hold", ByManifest<VRButton, CButton>, CButton.Use, i: 6);
            NavButton("A/P Off", "All AP Off", ByManifest<VRButton, CButton>, CButton.Use, i: 81);

            // Clear waypoint button (manifest)
            NavButton("Clear Waypoint", "Clear Waypoint", ByManifest<VRButton, CButton>, CButton.Use, i: 10);

            // Altitude mode (baro/radar) lever
            NavButton("Altitude Mode Toggle", "Altiude Mode", ByManifest<VRLever, CLever>, CLever.Cycle, i: 0);
            NavButton("Altitude Mode Baro", "Altiude Mode", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 0);
            NavButton("Altitude Mode Radar", "Altiude Mode", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 0);

            // Use name-based lookup for AP altitude to avoid manifest index changes
            NavAxisC("A/P Altitude", "Adjust AP Altitude", ByNamePartial<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            AddPostUpdateControl("Adjust AP Altitude");
        }

        protected override void CreateSystemsControls()
        {
            SystemsButton("Clear Cautions", "Clear Cautions", ByManifest<VRButton, CButton>, CButton.Use, i: 25);

            // Master arm
            SystemsButton("Master Arm Toggle", "Master Arm", ByManifest<VRLever, CLever>, CLever.Cycle, i: 23);
            SystemsButton("Master Arm On", "Master Arm", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 23);
            SystemsButton("Master Arm Off", "Master Arm", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 23);

            // Engines
            SystemsButton("Left Engine Toggle", "Left Engine", ByManifest<VRLever, CLever>, CLever.Cycle, i: 22);
            SystemsButton("Left Engine On", "Left Engine", ByManifest<VRLever, CLever>, CLever.Set, 2, i: 22);
            SystemsButton("Left Engine Off", "Left Engine", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 22);

            SystemsButton("Right Engine Toggle", "Right Engine", ByManifest<VRLever, CLever>, CLever.Cycle, i: 27);
            SystemsButton("Right Engine On", "Right Engine", ByManifest<VRLever, CLever>, CLever.Set, 2, i: 27);
            SystemsButton("Right Engine Off", "Right Engine", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 27);

            // APU
            SystemsButton("APU Toggle", "APU Start", ByManifest<VRLever, CLever>, CLever.Cycle, i: 1);
            SystemsButton("APU On", "APU Start", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 1);
            SystemsButton("APU Off", "APU Start", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 1);

            // Battery
            SystemsButton("Battery Toggle", "Battery", ByManifest<VRLever, CLever>, CLever.Cycle, i: 3);
            SystemsButton("Battery On", "Battery", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 3);
            SystemsButton("Battery Off", "Battery", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 3);

            // Cycle Jamming Mode (button)
            SystemsButton("Cycle Jamming Mode", "Cycle Jamming Mode", ByManifest<VRButton, CButton>, CButton.Use, i: 23);

            // Countermeasures: provide deploy and discrete toggles with name-based fallbacks
            SystemsButton("Fire Countermeasures", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.MenuButton);
            SystemsButton("Flares Toggle", "Toggle Flares", ByNamePartial<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            SystemsButton("Chaff Toggle", "Toggle Chaff", ByNamePartial<VRInteractable, CInteractable>, CInteractable.Use, n: true);

            // Radar / TGP / RWR / Jammer

            SystemsButton("TGP Power Toggle", "TGP Power", ByManifest<VRLever, CLever>, CLever.Cycle, i: 31);
            SystemsButton("TGP Power On", "TGP Power", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 31);
            SystemsButton("TGP Power Off", "TGP Power", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 31);

            SystemsButton("Jammer Power Toggle", "Jammer Power", ByManifest<VRLever, CLever>, CLever.Cycle, i: 20);
            SystemsButton("Jammer Power On", "Jammer Power", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 20);
            SystemsButton("Jammer Power Off", "Jammer Power", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 20);

            // HMCS power (toggle)
            SystemsButton("HMCS Power Toggle", "HMCS Power", ByManifest<VRLever, CLever>, CLever.Cycle, i: 15);
            SystemsButton("HMCS Power On", "HMCS Power", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 15);
            SystemsButton("HMCS Power Off", "HMCS Power", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 15);

            AddPostUpdateControl("Throttle");
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Toggle Visor", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet NV Toggle", "Toggle NVG", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);

            HUDButton("HUD Power Toggle", "HUD Power", ByNamePartial<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            HUDButton("HUD Power On", "HUD Power", ByNamePartial<VRInteractable, CInteractable>, CInteractable.Use,  n: true);
            HUDButton("HUD Power Off", "HUD Power", ByNamePartial<VRInteractable, CInteractable>, CInteractable.Use, n: true);
 
            HUDButton("HUD Tint", "HUD Tint", ByNamePartial<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            HUDButton("HUD Brightness", "HUD Brightness", ByNamePartial<VRTwistKnob, CKnob>, CKnob.Set, n: true);

            // HUD color mode (day/night)
            HUDButton("HUD Color Mode Toggle", "HUD Color Mode", ByManifest<VRLever, CLever>, CLever.Cycle, i: 16);
            HUDButton("HUD Color Mode Day", "HUD Color Mode", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 16);
            HUDButton("HUD Color Mode Night", "HUD Color Mode", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 16);
        }

        protected override void CreateNumPadControls()
        {
            // Use manifest indices so names that include extra text map correctly
            NumPadButton("1", "1 / Swap Radio Frequency", ByManifest<VRButton, CButton>, CButton.Use, i: 71);
            NumPadButton("2", "2 / Set Standby Frequency", ByManifest<VRButton, CButton>, CButton.Use, i: 72);
            NumPadButton("3", "3 / Set ILS Frequency", ByManifest<VRButton, CButton>, CButton.Use, i: 73);
            NumPadButton("4", "4 / Set AP Altitude", ByManifest<VRButton, CButton>, CButton.Use, i: 74);
            NumPadButton("5", "5 / Set AP Heading", ByManifest<VRButton, CButton>, CButton.Use, i: 75);
            NumPadButton("6", "6 / Set AP Speed", ByManifest<VRButton, CButton>, CButton.Use, i: 76);
            NumPadButton("7", "7 / Swap MFDs", ByManifest<VRButton, CButton>, CButton.Use, i: 77);
            NumPadButton("8", "8 / Set TGP Laser Code", ByManifest<VRButton, CButton>, CButton.Use, i: 78);
            NumPadButton("9", "9 / Set Seeker Code", ByManifest<VRButton, CButton>, CButton.Use, i: 79);
            NumPadButton("0", "0", ByManifest<VRButton, CButton>, CButton.Use, i: 70);
            NumPadButton("Enter", "Enter", ByManifest<VRButton, CButton>, CButton.Use, i: 12);
            NumPadButton("Clear", "Clear", ByManifest<VRButton, CButton>, CButton.Use, i: 9);
        }

        protected override void CreateDisplayControls()
        {
            // MFD / SOI
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

            DisplayAxis("MFD Brightness", "MFD Brightness", ByNamePartial<VRTwistKnob, CKnob>, CKnob.Set, n: true);

            // Left/right MFD power and full button sets
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

            DisplayButton("MFD Right Toggle", "MFD Right", MFD, CMFD.PowerToggle, i: 1);
            DisplayButton("MFD Right On", "MFD Right", MFD, CMFD.PowerOn, i: 1);
            DisplayButton("MFD Right Off", "MFD Right", MFD, CMFD.PowerOff, i: 1);
            DisplayButton("MFD Right L1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L1, i: 1);
            DisplayButton("MFD Right L2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L2, i: 1);
            DisplayButton("MFD Right L3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L3, i: 1);
            DisplayButton("MFD Right L4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L4, i: 1);
            DisplayButton("MFD Right L5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L5, i: 1);
            DisplayButton("MFD Right R1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R1, i: 1);
            DisplayButton("MFD Right R2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R2, i: 1);
            DisplayButton("MFD Right R3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R3, i: 1);
            DisplayButton("MFD Right R4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R4, i: 1);
            DisplayButton("MFD Right R5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R5, i: 1);
            DisplayButton("MFD Right T1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 1);
            DisplayButton("MFD Right T2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T2, i: 1);
            DisplayButton("MFD Right T3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T3, i: 1);
            DisplayButton("MFD Right T4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T4, i: 1);
            DisplayButton("MFD Right T5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T5, i: 1);
            DisplayButton("MFD Right B1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B1, i: 1);
            DisplayButton("MFD Right B2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B2, i: 1);
            DisplayButton("MFD Right B3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B3, i: 1);
            DisplayButton("MFD Right B4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B4, i: 1);
            DisplayButton("MFD Right B5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B5, i: 1);

            // Swap MFDs (manifest)
            DisplayButton("Swap MFDs", "Swap MFDs", ByManifest<VRButton, CButton>, CButton.Use, i: 67);

            AddPostUpdateControl("MFD Brightness");
            AddPostUpdateControl("SOI");
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

        protected override void CreateMusicControls()
        {
        }

        protected override void CreateLightsControls()
        {
            // Instrument lights are a lever in A-10D manifest
            LightsButton("Instrument Lights Toggle", "Instrument Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 18);
            LightsButton("Instrument Lights On", "Instrument Lights", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 18);
            LightsButton("Instrument Lights Off", "Instrument Lights", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 18);

            LightsAxis("Instrument Brightness", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);

            LightsButton("Interior Lights Toggle", "Interior Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 19);
            LightsButton("Landing Lights Toggle", "Landing Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 21);
            LightsButton("Nav Lights Toggle", "Nav Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 25);
            LightsButton("Strobe Lights Toggle", "Strobe Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 30);
            // Formation lights
            LightsButton("Formation Lights Toggle", "Formation Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 10);
            LightsButton("Formation Lights On", "Formation Lights", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 10);
            LightsButton("Formation Lights Off", "Formation Lights", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 10);

            AddPostUpdateControl("Instrument Brightness");
        }

        protected override void CreateMiscControls()
        {
            MiscButton("Canopy Toggle", "Canopy", ByManifest<VRLever, CLever>, CLever.Cycle, i: 5);
            MiscButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, s: -1, n: true);

            MiscButton("Fuel Port Toggle", "Fuel Port", ByManifest<VRLever, CLever>, CLever.Cycle, i: 12);
            MiscButton("Fuel Dump Toggle", "Fuel Dump Switch", ByManifest<VRLever, CLever>, CLever.Cycle, i: 11);

            // Jettison controls
            MiscButton("Switch Cover (Jettison)", "Switch Cover (Jettison)", ByNamePartial<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            MiscButton("Jettison Execute", "Jettison", ByManifest<VRButton, CButton>, CButton.Use, i: 26);
            MiscButton("Jettison All", "Jettison All", ByManifest<VRButton, CButton>, CButton.Use, i: 20);
            MiscButton("Jettison Empty", "Jettison Empty", ByManifest<VRButton, CButton>, CButton.Use, i: 21);
            MiscButton("Jettison Ext Tanks", "Jettison Ext Tanks", ByManifest<VRButton, CButton>, CButton.Use, i: 22);
            MiscButton("Clear Jettison Marks", "Clear Jettison Marks", ByManifest<VRButton, CButton>, CButton.Use, i: 8);
        }
    }
}
