using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using VTOLVR.Multiplayer;
using MFDButtons = MFD.MFDButtons;

namespace BYOJoystick.Managers
{
    public class SU47MManager : Manager
    {
        public override string GameName => "SU-47M";
        public override string ShortName => "SU47M";
        public override bool IsMulticrew => false;

        private CJoystick CockpitJoystick(string name, string root, bool nullable, bool checkName, int idx)
        {
            if (TryGetExistingControl<CJoystick>(name, out var existingControl))
                return existingControl;

            var sticks = VehicleControlManifest.joysticks;
            VRJoystick side = sticks != null && sticks.Length > 0 ? sticks[0] : null;
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

        // altitude ap button is mislabeled as heading hold in game
        private CButton ApAltitudeHoldButton(string name, string root, bool nullable, bool checkName, int idx)
        {
            if (TryGetExistingControl<CButton>(name, out var existingControl))
                return existingControl;

            foreach (var interactable in Interactables)
            {
                if (interactable.gameObject.name != "AltitudeAPButton")
                    continue;
                var button = interactable.GetComponent<VRButton>();
                if (button == null)
                {
                    if (nullable)
                        return null;
                    throw new System.InvalidOperationException("AltitudeAPButton requires VRButton.");
                }
                return ToControl<VRButton, CButton>(name, interactable, button);
            }

            if (nullable)
                return null;
            throw new System.InvalidOperationException("AltitudeAPButton not found.");
        }
        protected override void CreateFlightControls()
        {
            FlightAxisC("Joystick Pitch", "Joystick", CockpitJoystick, CJoystick.SetPitch);
            FlightAxisC("Joystick Yaw", "Joystick", CockpitJoystick, CJoystick.SetYaw);
            FlightAxisC("Joystick Roll", "Joystick", CockpitJoystick, CJoystick.SetRoll);

            FlightAxis("Throttle", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Set);
            FlightButton("Throttle Increase", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Increase);
            FlightButton("Throttle Decrease", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Decrease);
            FlightAxis("Brakes Axis", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.TriggerAxis);
            FlightButton("Brakes", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);

            FlightButton("Landing Gear Toggle", "Landing Gear", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Landing Gear Up", "Landing Gear", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Landing Gear Down", "Landing Gear", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Parking Brake Toggle", "Brake Locks", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Parking Brake On", "Brake Locks", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Parking Brake Off", "Brake Locks", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Flaps Cycle", "Flaps", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Flaps Up", "Flaps", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            FlightButton("Flaps Down", "Flaps", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);

            FlightButton("Wing Fold Toggle", "Wing Fold", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Wing Fold Open", "Wing Fold", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Wing Fold Close", "Wing Fold", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Canopy Toggle", "Canopy", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Canopy Open", "Canopy", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Canopy Close", "Canopy", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Fire Weapon", "Joystick", CockpitJoystick, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", CockpitJoystick, CJoystick.MenuButton);
            FlightButton("Fire Countermeasures", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.MenuButton);
            FlightButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, s: -1, n: true);

            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
            AssistButton("Flight Assist Toggle", "Toggle Flight Assist", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Flight Assist On", "Toggle Flight Assist", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            AssistButton("Flight Assist Off", "Toggle Flight Assist", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            AssistButton("Pitch SAS Toggle", "Toggle Pitch Assist", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Yaw SAS Toggle", "Toggle Yaw Assist", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Roll SAS Toggle", "Toggle Roll Assist", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            AssistButton("G-Limiter Toggle", "Toggle G-Limiter", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("G-Limiter On", "Toggle G-Limiter", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            AssistButton("G-Limiter Off", "Toggle G-Limiter", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            AssistButton("Pitch Trim Toggle", "Toggle Pitch Trim", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateNavigationControls()
        {
            NavButton("A/P Nav Mode", "Navigation Mode", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Spd Hold", "Airspeed Hold", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Hdg Hold", "Heading Hold", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Alt Hold", "A/P Alt Hold", ApAltitudeHoldButton, CButton.Use, s: -1, n: true);
            NavButton("A/P Off", "All AP Off", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("Toggle Altitude Mode", "Toggle Altitude Mode", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("Clear Waypoint", "Clear Waypoint", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            NavAxisC("AP Heading Set", "AP Heading Set", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("AP Heading Up", "AP Heading Set", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("AP Heading Down", "AP Heading Set", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("Auto Speed Set", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("Auto Speed Up", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("Auto Speed Down", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("Course Set", "Course Set", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("Course Up", "Course Set", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("Course Down", "Course Set", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("AP Heading Set");
            AddPostUpdateControl("Auto Speed Set");
            AddPostUpdateControl("Course Set");
        }

        protected override void CreateSystemsControls()
        {
            SystemsButton("Clear Cautions", "Clear Cautions", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Master Arm Toggle", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Master Arm On", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Master Arm Off", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Battery Toggle", "Main Battery", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Battery On", "Main Battery", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Battery Off", "Main Battery", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("APU Toggle", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("APU On", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("APU Off", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Left Engine Toggle", "Left Engine", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Left Engine On", "Left Engine", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Left Engine Off", "Left Engine", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            SystemsButton("Right Engine Toggle", "Right Engine", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Right Engine On", "Right Engine", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Right Engine Off", "Right Engine", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Radar Power Toggle", "Radar Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Radar Power On", "Radar Power", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Radar Power Off", "Radar Power", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("RWR Toggle", "RWR Toggle", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            SystemsButton("Toggle Chaff", "Toggle Chaff", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Toggle Flares", "Toggle Flares", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            SystemsButton("Jettison Execute", "Jettison", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Jettison All", "Jettison All", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Jettison Empty", "Jettison Empty", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Jettison Ext Hardpoints", "Jettison Ext Hardpoints", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Clear Jettison Marks", "Clear Jettison Marks", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            SystemsButton("Arrestor Hook Toggle", "Arrestor Hook", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Launch Bar Toggle", "Launch Bar", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Fuel Dump Toggle", "Fuel Dump Switch", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Toggle Visor", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet NV Toggle", "Toggle NVG", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);

            HUDButton("HUD Power Toggle", "HUD Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            HUDButton("HMCS Power Toggle", "HMCS Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            HUDButton("Night HUD Toggle", "Night HUD", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            HUDAxisC("HUD Tint", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("HUD Tint Up", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("HUD Tint Down", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            HUDAxisC("MFD Brightness", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("MFD Brightness Up", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("MFD Brightness Down", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            HUDButton("HUD Declutter Cycle", "HUD Declutter", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);

            // soi slew
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

            AddPostUpdateControl("HUD Tint");
            AddPostUpdateControl("MFD Brightness");
            AddPostUpdateControl("SOI");
        }

        protected override void CreateNumPadControls()
        {
            NumPadButton("Swap Radio Frequency", "1 Swap Radio Frequency", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Set Standby Frequency", "2 Set Standby Frequency", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Set ILS Frequency", "3 Set ILS Frequency", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Set AP Altitude", "4 Set AP Altitude", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Set AP Heading", "5 Set AP Heading", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Set AP Speed", "6 Set AP Speed", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Swap MFDs", "7 Swap MFDs", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Set TGP Laser Code", "8 Set TGP Laser Code", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Set Seeker Code", "9 Set Seeker Code", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Numpad 0", "0", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Numpad Enter", "Enter", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NumPadButton("Numpad Clear", "Clear", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }

        protected override void CreateDisplayControls()
        {
            DisplayButton("MFD Left Toggle", "MFD Left", MFD, CMFD.PowerToggle, i: 0);
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

            AddPostUpdateControl("MFD Left");
            AddPostUpdateControl("MFD Right");
        }

        protected override void CreateRadioControls()
        {
            RadioButton("Radio Transmit", "Radio", ByType<CockpitTeamRadioManager, CRadio>, CRadio.Transmit, s: -1, n: true);
        }

        protected override void CreateMusicControls()
        {
            MusicButton("Music Play/Pause", "PlayPause", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MusicButton("Music Next", "Next Song", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MusicButton("Music Prev", "Prev Song", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            MusicAxisC("Radio Volume", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            MusicButton("Radio Volume Up", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            MusicButton("Radio Volume Down", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);
        }

        protected override void CreateLightsControls()
        {
            // nav and strobe are vrlever here not knobint
            LightsButton("Nav Lights Toggle", "Nav Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Strobe Lights Toggle", "Strobe Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Landing Lights Toggle", "Landing Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Formation Lights Toggle", "Formation Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Interior Lights Toggle", "Interior Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Instrument Lights Toggle", "Instrument Lights", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateMiscControls()
        {
            MiscButton("Fuel Port Toggle", "Fuel Port", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            MiscButton("Fuel Port Open", "Fuel Port", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            MiscButton("Fuel Port Close", "Fuel Port", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
        }
    }
}