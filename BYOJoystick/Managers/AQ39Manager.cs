using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using VTOLVR.Multiplayer;
using MFDButtons = MFD.MFDButtons;

namespace BYOJoystick.Managers
{
    public class AQ39Manager : Manager
    {
        public override string GameName    => "AQ-39";
        public override string ShortName => "AQ39";
        public override bool   IsMulticrew => false;

        private static string Dash           => "Local/CockpitLining/Cockpit/Displays";
        private static string SideJoystick   => "Local/JoystickObjects/SideStickObjects";
        private static string CenterJoystick => "Local/JoystickObjects/CenterStickObjects";

        private CJoystick Joysticks(string name, string root, bool nullable, bool checkName, int idx)
        {
            return GetJoysticksByPaths(name, SideJoystick, CenterJoystick);
        }

        protected override void PreMapping()
        {
            LogInteractablesIfEnabled(ShortName);
        }

        protected override void CreateFlightControls()
        {
            FlightAxisC("Joystick Pitch", "Joystick", Joysticks, CJoystick.SetPitch);
            FlightAxisC("Joystick Yaw", "Joystick", Joysticks, CJoystick.SetYaw);
            FlightAxisC("Joystick Roll", "Joystick", Joysticks, CJoystick.SetRoll);

            FlightAxis("Throttle", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Set);
            FlightButton("Throttle Increase", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Increase);
            FlightButton("Throttle Decrease", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Decrease);

            FlightAxis("Brakes/Airbrakes Axis", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.TriggerAxis);
            FlightButton("Brakes/Airbrakes", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);

            FlightButton("Landing Gear Toggle", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Cycle, i: 2);
            FlightButton("Landing Gear Up", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 2);
            FlightButton("Landing Gear Down", "Landing Gear", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 2);

            FlightButton("Parking Brake Toggle", "Parking Brake", ByManifest<VRLever, CLever>, CLever.Cycle, i: 4);
            FlightButton("Parking Brake On", "Parking Brake", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 4);
            FlightButton("Parking Brake Off", "Parking Brake", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 4);

            FlightButton("Wing Fold Toggle", "Wing Fold", ByManifest<VRLever, CLever>, CLever.Cycle, i: 3);
            FlightButton("Wing Fold Up", "Wing Fold", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 3);
            FlightButton("Wing Fold Down", "Wing Fold", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 3);

            FlightButton("Launch Bar Toggle", "Launch Bar", ByManifest<VRLever, CLever>, CLever.Cycle, i: 11);
            FlightButton("Launch Bar Extend", "Launch Bar", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 11);
            FlightButton("Launch Bar Retract", "Launch Bar", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 11);

            FlightButton("Arrestor Hook Toggle", "Arrestor Hook", ByManifest<VRLever, CLever>, CLever.Cycle, i: 13);
            FlightButton("Arrestor Hook Extend", "Arrestor Hook", ByManifest<VRLever, CLever>, CLever.Set, s: 1, i: 13);
            FlightButton("Arrestor Hook Retract", "Arrestor Hook", ByManifest<VRLever, CLever>, CLever.Set, s: 0, i: 13);

            FlightButton("Fire Weapon", "Joystick", Joysticks, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", Joysticks, CJoystick.MenuButton);
            FlightButton("Fire Countermeasures", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.MenuButton);

            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
        }

        protected override void CreateNavigationControls()
        {
            NavButton("A/P Nav Mode", "Nav Waypoint AP", ByName<VRInteractable, CInteractable>, CInteractable.Use);
            NavButton("A/P Hdg Hold", "Heading AP", ByName<VRInteractable, CInteractable>, CInteractable.Use);
            NavButton("A/P Alt Hold", "Altitude AP", ByName<VRInteractable, CInteractable>, CInteractable.Use);
            NavButton("A/P Spd Hold", "Speed AP", ByName<VRInteractable, CInteractable>, CInteractable.Use);
            NavButton("A/P Off", "AP Off", ByName<VRInteractable, CInteractable>, CInteractable.Use);

            NavButton("Altitude Mode Toggle", "Altitude Mode", ByName<VRInteractable, CInteractable>, CInteractable.Use);
            NavButton("Clear Waypoint", "Clear Waypoint", ByManifest<VRButton, CButton>, CButton.Use, i: 19);
        }

        protected override void CreateSystemsControls()
        {
            SystemsButton("Clear Cautions", "Dismiss", ByManifest<VRButton, CButton>, CButton.Use, i: 11);

            SystemsButton("Master Arm Toggle", "Master Arm", ByName<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false);
            SystemsButton("Master Arm On", "Master Arm", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 1, c: false);
            SystemsButton("Master Arm Off", "Master Arm", ByName<VRLever, CLeverCovered>, CLeverCovered.Set, 0, c: false);

            SystemsButton("Chaff Toggle", "CMSConfigUI", ByType<CMSConfigUI, CCMS>, CCMS.ToggleChaff);
            SystemsButton("Chaff On", "CMSConfigUI", ByType<CMSConfigUI, CCMS>, CCMS.ChaffOn);
            SystemsButton("Chaff Off", "CMSConfigUI", ByType<CMSConfigUI, CCMS>, CCMS.ChaffOff);

            SystemsButton("Flares Toggle", "CMSConfigUI", ByType<CMSConfigUI, CCMS>, CCMS.ToggleFlares);
            SystemsButton("Flares On", "CMSConfigUI", ByType<CMSConfigUI, CCMS>, CCMS.FlaresOn);
            SystemsButton("Flares Off", "CMSConfigUI", ByType<CMSConfigUI, CCMS>, CCMS.FlaresOff);

            SystemsButton("Engine Toggle", "Switch Cover", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false, i: 14);
            SystemsButton("Engine On", "Switch Cover", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, 1, c: false, i: 14);
            SystemsButton("Engine Off", "Switch Cover", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, 0, c: false, i: 14);

            SystemsButton("Main Battery Toggle", "Main Battery", ByManifest<VRLever, CLever>, CLever.Cycle, i: 16);
            SystemsButton("Main Battery On", "Main Battery", ByManifest<VRLever, CLever>, CLever.Set, 1, i: 16);
            SystemsButton("Main Battery Off", "Main Battery", ByManifest<VRLever, CLever>, CLever.Set, 0, i: 16);

            SystemsButton("APU Toggle", "Switch Cover", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Cycle, c: false, i: 17);
            SystemsButton("APU On", "Switch Cover", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, 1, c: false, i: 17);
            SystemsButton("APU Off", "Switch Cover", ByManifest<VRLever, CLeverCovered>, CLeverCovered.Set, 0, c: false, i: 17);

            SystemsButton("Radar Power Toggle", "Radar Power", ByName<VRInteractable, CInteractable>, CInteractable.Use, n: true);
            SystemsButton("Jettison Toggle", "Jettison Switch", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Helmet", HelmetController, CHelmet.ToggleVisor);
            HUDButton("Helmet NV Toggle", "Helmet", HelmetController, CHelmet.ToggleNightVision);
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
            NumPadButton("0", "0 / Bingo", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("Enter", "Enter", ByName<VRButton, CButton>, CButton.Use, r: Dash);
            NumPadButton("Clear", "Clear", ByName<VRButton, CButton>, CButton.Use, r: Dash);
        }

        protected override void CreateDisplayControls()
        {
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
            DisplayButton("SOI Radar Elev Up", "SOI", SOI, CSOI.RadarElevUp);
            DisplayButton("SOI Radar Elev Down", "SOI", SOI, CSOI.RadarElevDown);

            DisplayButton("TSD Slew TGP/EOTS", "Slew EOTS", TSDInteractable, CInteractable.Use);
            DisplayButton("TSD GPS-S", "GPS Send", TSDInteractable, CInteractable.Use);

            DisplayAxis("MFD Brightness", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            DisplayButton("MFD Brightness +", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, n: true);
            DisplayButton("MFD Brightness -", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, n: true);

            DisplayButton("MFD Power Toggle", "MFD Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
            DisplayButton("MFD Power On", "MFD Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 1, n: true);
            DisplayButton("MFD Power Off", "MFD Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 0, n: true);

            DisplayButton("MFD1 L1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L1, i: 0, n: true);
            DisplayButton("MFD1 L2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L2, i: 0, n: true);
            DisplayButton("MFD1 L3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L3, i: 0, n: true);
            DisplayButton("MFD1 L4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L4, i: 0, n: true);
            DisplayButton("MFD1 L5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L5, i: 0, n: true);
            DisplayButton("MFD1 R1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R1, i: 0, n: true);
            DisplayButton("MFD1 R2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R2, i: 0, n: true);
            DisplayButton("MFD1 R3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R3, i: 0, n: true);
            DisplayButton("MFD1 R4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R4, i: 0, n: true);
            DisplayButton("MFD1 R5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R5, i: 0, n: true);
            DisplayButton("MFD1 T1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 0, n: true);
            DisplayButton("MFD1 T2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T2, i: 0, n: true);
            DisplayButton("MFD1 T3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T3, i: 0, n: true);
            DisplayButton("MFD1 T4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T4, i: 0, n: true);
            DisplayButton("MFD1 T5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T5, i: 0, n: true);
            DisplayButton("MFD1 B1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B1, i: 0, n: true);
            DisplayButton("MFD1 B2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B2, i: 0, n: true);
            DisplayButton("MFD1 B3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B3, i: 0, n: true);
            DisplayButton("MFD1 B4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B4, i: 0, n: true);
            DisplayButton("MFD1 B5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B5, i: 0, n: true);

            DisplayButton("MFD2 L1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L1, i: 1, n: true);
            DisplayButton("MFD2 L2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L2, i: 1, n: true);
            DisplayButton("MFD2 L3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L3, i: 1, n: true);
            DisplayButton("MFD2 L4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L4, i: 1, n: true);
            DisplayButton("MFD2 L5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L5, i: 1, n: true);
            DisplayButton("MFD2 R1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R1, i: 1, n: true);
            DisplayButton("MFD2 R2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R2, i: 1, n: true);
            DisplayButton("MFD2 R3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R3, i: 1, n: true);
            DisplayButton("MFD2 R4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R4, i: 1, n: true);
            DisplayButton("MFD2 R5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R5, i: 1, n: true);
            DisplayButton("MFD2 T1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 1, n: true);
            DisplayButton("MFD2 T2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T2, i: 1, n: true);
            DisplayButton("MFD2 T3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T3, i: 1, n: true);
            DisplayButton("MFD2 T4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T4, i: 1, n: true);
            DisplayButton("MFD2 T5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T5, i: 1, n: true);
            DisplayButton("MFD2 B1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B1, i: 1, n: true);
            DisplayButton("MFD2 B2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B2, i: 1, n: true);
            DisplayButton("MFD2 B3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B3, i: 1, n: true);
            DisplayButton("MFD2 B4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B4, i: 1, n: true);
            DisplayButton("MFD2 B5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B5, i: 1, n: true);

            DisplayButton("MFD3 L1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L1, i: 2, n: true);
            DisplayButton("MFD3 L2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L2, i: 2, n: true);
            DisplayButton("MFD3 L3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L3, i: 2, n: true);
            DisplayButton("MFD3 L4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L4, i: 2, n: true);
            DisplayButton("MFD3 L5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L5, i: 2, n: true);
            DisplayButton("MFD3 R1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R1, i: 2, n: true);
            DisplayButton("MFD3 R2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R2, i: 2, n: true);
            DisplayButton("MFD3 R3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R3, i: 2, n: true);
            DisplayButton("MFD3 R4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R4, i: 2, n: true);
            DisplayButton("MFD3 R5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R5, i: 2, n: true);
            DisplayButton("MFD3 T1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 2, n: true);
            DisplayButton("MFD3 T2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T2, i: 2, n: true);
            DisplayButton("MFD3 T3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T3, i: 2, n: true);
            DisplayButton("MFD3 T4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T4, i: 2, n: true);
            DisplayButton("MFD3 T5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T5, i: 2, n: true);
            DisplayButton("MFD3 B1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B1, i: 2, n: true);
            DisplayButton("MFD3 B2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B2, i: 2, n: true);
            DisplayButton("MFD3 B3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B3, i: 2, n: true);
            DisplayButton("MFD3 B4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B4, i: 2, n: true);
            DisplayButton("MFD3 B5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B5, i: 2, n: true);

            AddPostUpdateControl("MFD Brightness");
            AddPostUpdateControl("SOI");
        }

        protected override void CreateRadioControls()
        {
            RadioButton("Radio Transmit", "Radio", ByType<CockpitTeamRadioManager, CRadio>, CRadio.Transmit);

            RadioButton("Radio Channel Cycle", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
            RadioButton("Radio Channel Team", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 0, n: true);
            RadioButton("Radio Channel Freq", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 1, n: true);
            RadioButton("Radio Channel Global", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 2, n: true);

            RadioButton("Radio Mode Cycle", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, n: true);
            RadioButton("Radio Mode Next", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, n: true);
            RadioButton("Radio Mode Prev", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, n: true);
            RadioButton("Radio Mode Hot Mic", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 0, n: true);
            RadioButton("Radio Mode PTT", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 1, n: true);
            RadioButton("Radio Mode Off", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Set, 2, n: true);
        }

        protected override void CreateMusicControls()
        {
        }

        protected override void CreateLightsControls()
        {
            LightsButton("Landing Lights Toggle", "Landing Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 5);
            LightsButton("Strobe Lights Toggle", "Strobe Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 6);
            LightsButton("Nav Lights Toggle", "Nav Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 7);
            LightsButton("Interior Lights Toggle", "Interior Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 8);
            LightsButton("Formation Lights Toggle", "Formation Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 9);
            LightsButton("Instrument Lights Toggle", "Instrument Lights", ByManifest<VRLever, CLever>, CLever.Cycle, i: 10);

            LightsAxis("Instrument Brightness", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, n: true);
            LightsButton("Instrument Brightness +", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, n: true);
            LightsButton("Instrument Brightness -", "Instrument Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, n: true);

            AddPostUpdateControl("Instrument Brightness");
        }

        protected override void CreateMiscControls()
        {
        }
    }
}
