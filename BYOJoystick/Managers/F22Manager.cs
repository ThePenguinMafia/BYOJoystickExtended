using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using MFDButtons = MFD.MFDButtons;

namespace BYOJoystick.Managers
{
    public class F22Manager : Manager
    {
        public override string GameName    => "F-22";
        public override string ShortName   => "F22";
        public override bool   IsMulticrew => false;

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

        private CKnobInt DashTwistKnobInt(string registryName, string interactableObjectName, string root, bool nullable, int idx)
        {
            if (TryGetExistingControl<CKnobInt>(registryName, out var existingControl))
                return existingControl;

            var rootObject = root == null ? Vehicle : GetGameObject(root, nullable);
            if (rootObject == null)
                return null;

            var interactables = rootObject == Vehicle ? Interactables : rootObject.GetComponentsInChildren<VRInteractable>(true);
            foreach (var interactable in interactables)
            {
                if (interactable.gameObject.name != interactableObjectName)
                    continue;
                var knob = interactable.GetComponent<VRTwistKnobInt>();
                if (knob == null)
                    continue;
                return ToControl<VRTwistKnobInt, CKnobInt>(registryName, interactable, knob);
            }

            if (nullable)
                return null;
            throw new System.InvalidOperationException($"VRTwistKnobInt {interactableObjectName} not found.");
        }

        private CKnobInt RadarPowerKnob(string name, string root, bool nullable, bool checkName, int idx)
            => DashTwistKnobInt("Radar Power", "RadarPowerInteractable", root, nullable, idx);

        private CKnobInt RwrModeKnob(string name, string root, bool nullable, bool checkName, int idx)
            => DashTwistKnobInt("RWR Mode", "RWRModeInteractable", root, nullable, idx);

        protected override void PreMapping()
        {
            LogInteractablesIfEnabled("F22");
        }

        protected override void CreateFlightControls()
        {
            FlightAxisC("Joystick Pitch", "Joystick", CockpitJoystick, CJoystick.SetPitch);
            FlightAxisC("Joystick Yaw", "Joystick", CockpitJoystick, CJoystick.SetYaw);
            FlightAxisC("Joystick Roll", "Joystick", CockpitJoystick, CJoystick.SetRoll);

            FlightAxis("Throttle", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Set);
            FlightButton("Throttle Increase", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Increase);
            FlightButton("Throttle Decrease", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Decrease);

            FlightAxis("Brakes/Airbrakes Axis", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.TriggerAxis);
            FlightButton("Brakes/Airbrakes", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.Trigger);

            FlightButton("Landing Gear Toggle", "Landing Gear", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Landing Gear Up", "Landing Gear", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Landing Gear Down", "Landing Gear", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Parking Brake Toggle", "Parking Brake", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Parking Brake On", "Parking Brake", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Parking Brake Off", "Parking Brake", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Arrestor Hook Toggle", "Arrestor Hook", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Arrestor Hook Down", "Arrestor Hook", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Arrestor Hook Up", "Arrestor Hook", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            FlightButton("Fire Weapon", "Joystick", CockpitJoystick, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Joystick", CockpitJoystick, CJoystick.MenuButton);

            FlightButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, s: -1, n: true);

            AddPostUpdateControl("Joystick");
            AddPostUpdateControl("Throttle");
        }

        protected override void CreateAssistControls()
        {
            AssistButton("TVC Power Toggle", "TVC Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("TVC Power On", "TVC Power", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            AssistButton("TVC Power Off", "TVC Power", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // flaps override ref has trailing space
            AssistButton("Flaps Override Toggle", "Flaps Override ", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            AssistButton("Flaps Override On", "Flaps Override ", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            AssistButton("Flaps Override Off", "Flaps Override ", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
        }

        protected override void CreateNavigationControls()
        {
            NavButton("A/P Nav Mode", "Navigation Mode", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Spd Hold", "Airspeed Hold", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Hdg Hold", "Heading Hold", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Alt Hold", "Altitude Hold", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Off", "All AP Off", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            NavAxisC("A/P Altitude", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Alt Increase", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Alt Decrease", "Adjust AP Altitude", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavAxisC("A/P Speed", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            NavButton("A/P Speed Increase", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            NavButton("A/P Speed Decrease", "Auto Speed Set", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            NavButton("Clear Waypoint", "Clear Waypoint", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            NavButton("Toggle Altitude Mode", "Toggle Altitude Mode", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            AddPostUpdateControl("Adjust AP Altitude");
            AddPostUpdateControl("Auto Speed Set");
        }

        protected override void CreateSystemsControls()
        {
            SystemsButton("Clear Cautions", "Clear Cautions", ByManifest<VRButton, CButton>, CButton.Use, i: 6);

            SystemsButton("Master Arm Toggle", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Master Arm On", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Master Arm Off", "Master Arm Switch", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Battery Toggle", "Main Battery", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Battery On", "Main Battery", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Battery Off", "Main Battery", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("APU Toggle", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("APU On", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("APU Off", "Auxilliary Power", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Engine Select Cycle", "Engine Select", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Motor Toggle", "Motor Toggle", ByManifest<VRButton, CButton>, CButton.Use, i: 29);

            SystemsButton("Radar Power Toggle", "Radar Power", RadarPowerKnob, CKnobInt.Cycle, r: "Local/DashCanvas/Dash", s: -1, n: true);
            SystemsButton("Radar Power On", "Radar Power", RadarPowerKnob, CKnobInt.Set, r: "Local/DashCanvas/Dash", s: 1, n: true);
            SystemsButton("Radar Power Off", "Radar Power", RadarPowerKnob, CKnobInt.Set, r: "Local/DashCanvas/Dash", s: 0, n: true);

            SystemsButton("RWR Mode Cycle", "RWR Mode", RwrModeKnob, CKnobInt.Cycle, r: "Local/DashCanvas/Dash", s: -1, n: true);
            SystemsButton("RWR Mode Next", "RWR Mode", RwrModeKnob, CKnobInt.Next, r: "Local/DashCanvas/Dash", s: -1, n: true);
            SystemsButton("RWR Mode Prev", "RWR Mode", RwrModeKnob, CKnobInt.Prev, r: "Local/DashCanvas/Dash", s: -1, n: true);

            SystemsButton("Toggle Chaff", "Toggle Chaff", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Toggle Flares", "Toggle Flares", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Fire Countermeasures", "Throttle", ByManifest<VRThrottle, CThrottle>, CThrottle.MenuButton);

            SystemsButton("Jettison Switch Cycle", "Jettison Switch", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("Jettison Switch Next", "Jettison Switch", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            SystemsButton("Jettison Switch Prev", "Jettison Switch", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);
            SystemsButton("Jettison Execute", "Jettison", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Jettison All", "Jettison All", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Jettison Empty", "Jettison Empty", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Clear Jettison Marks", "Clear Jettison Marks", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Jettison Ext Tanks", "Jettison Ext Tanks", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            SystemsButton("Fuel Dump Toggle", "Fuel Dump Switch", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Fuel Dump On", "Fuel Dump Switch", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Fuel Dump Off", "Fuel Dump Switch", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("AAR Door Toggle", "AAR Door", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("AAR Door Open", "AAR Door", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("AAR Door Close", "AAR Door", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
        }

        protected override void CreateHUDControls()
        {
            HUDButton("Helmet Visor Toggle", "Toggle Visor", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet NV Toggle", "Toggle NVG", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);

            HUDAxis("HUD Brightness", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("HUD Brightness +", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("HUD Brightness -", "HUD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            HUDAxis("HUD Tint", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("HUD Tint +", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("HUD Tint -", "HUD Tint", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            HUDButton("HUD Declutter Cycle", "HUD Declutter", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            HUDButton("HUD Declutter Next", "HUD Declutter", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            HUDButton("HUD Declutter Prev", "HUD Declutter", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            HUDButton("HUD Settings", "HUD Settings", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            AddPostUpdateControl("HUD Brightness");
            AddPostUpdateControl("HUD Tint");
        }

        protected override void CreateNumPadControls()
        {
            NumPadButton("1 / Swap Radio Frequency", "1 / Swap Radio Frequency", ByManifest<VRButton, CButton>, CButton.Use, i: 7);
            NumPadButton("2 / Set Standby Frequency", "2 / Set Standby Frequency", ByManifest<VRButton, CButton>, CButton.Use, i: 8);
            NumPadButton("3 / Set ILS Frequency", "3 / Set ILS Frequency", ByManifest<VRButton, CButton>, CButton.Use, i: 9);
            NumPadButton("4 / Set AP Altitude", "4 / Set AP Altitude", ByManifest<VRButton, CButton>, CButton.Use, i: 10);
            NumPadButton("5 / Set AP Heading", "5 / Set AP Heading", ByManifest<VRButton, CButton>, CButton.Use, i: 11);
            NumPadButton("6 / Set AP Speed", "6 / Set AP Speed", ByManifest<VRButton, CButton>, CButton.Use, i: 12);
            NumPadButton("7 / Swap MFDs", "7 / Swap MFDs", ByManifest<VRButton, CButton>, CButton.Use, i: 13);
            NumPadButton("8 / Set TGP Laser Code", "8 / Set TGP Laser Code", ByManifest<VRButton, CButton>, CButton.Use, i: 14);
            NumPadButton("9 / Set Seeker Code", "9 / Set Seeker Code", ByManifest<VRButton, CButton>, CButton.Use, i: 15);
            NumPadButton("0", "0", ByManifest<VRButton, CButton>, CButton.Use, i: 16);
            NumPadButton("Enter", "Enter", ByManifest<VRButton, CButton>, CButton.Use, i: 17);
            NumPadButton("Clear", "Clear", ByManifest<VRButton, CButton>, CButton.Use, i: 18);
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

            DisplayAxis("MFD Brightness", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            DisplayButton("MFD Brightness +", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            DisplayButton("MFD Brightness -", "MFD Brightness", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            DisplayButton("Swap MFDs", "Swap MFDs", ByManifest<VRButton, CButton>, CButton.Use, i: 19);

            DisplayButton("MFD Left Power Cycle", "MFD Left Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            DisplayButton("MFD Center Power Cycle", "MFD Center Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            DisplayButton("MFD Right Power Cycle", "MFD Right Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            DisplayButton("MFD Bottom Power Cycle", "MFD Bottom Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);

            DisplayButton("MMFD Left Power Cycle", "MMFD Left Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            DisplayButton("MMFD Right Power Cycle", "MMFD Right Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);

            DisplayButton("MPD Preset 1", "MPD Preset 1", ByManifest<VRButton, CButton>, CButton.Use, i: 27);
            DisplayButton("MPD Preset 2", "MPD Preset 2", ByManifest<VRButton, CButton>, CButton.Use, i: 28);

            // mfd slots 0 1 2 3
            DisplayButton("MFD1 L1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L1, i: 0);
            DisplayButton("MFD1 L2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L2, i: 0);
            DisplayButton("MFD1 L3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L3, i: 0);
            DisplayButton("MFD1 L4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L4, i: 0);
            DisplayButton("MFD1 L5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.L5, i: 0);
            DisplayButton("MFD1 R1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R1, i: 0);
            DisplayButton("MFD1 R2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R2, i: 0);
            DisplayButton("MFD1 R3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R3, i: 0);
            DisplayButton("MFD1 R4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R4, i: 0);
            DisplayButton("MFD1 R5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.R5, i: 0);
            DisplayButton("MFD1 T1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 0);
            DisplayButton("MFD1 T2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T2, i: 0);
            DisplayButton("MFD1 T3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T3, i: 0);
            DisplayButton("MFD1 T4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T4, i: 0);
            DisplayButton("MFD1 T5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.T5, i: 0);
            DisplayButton("MFD1 B1", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B1, i: 0);
            DisplayButton("MFD1 B2", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B2, i: 0);
            DisplayButton("MFD1 B3", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B3, i: 0);
            DisplayButton("MFD1 B4", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B4, i: 0);
            DisplayButton("MFD1 B5", "MFD Left", MFD, CMFD.Press, (int)MFDButtons.B5, i: 0);

            DisplayButton("MFD2 L1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L1, i: 1);
            DisplayButton("MFD2 L2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L2, i: 1);
            DisplayButton("MFD2 L3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L3, i: 1);
            DisplayButton("MFD2 L4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L4, i: 1);
            DisplayButton("MFD2 L5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.L5, i: 1);
            DisplayButton("MFD2 R1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R1, i: 1);
            DisplayButton("MFD2 R2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R2, i: 1);
            DisplayButton("MFD2 R3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R3, i: 1);
            DisplayButton("MFD2 R4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R4, i: 1);
            DisplayButton("MFD2 R5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.R5, i: 1);
            DisplayButton("MFD2 T1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 1);
            DisplayButton("MFD2 T2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T2, i: 1);
            DisplayButton("MFD2 T3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T3, i: 1);
            DisplayButton("MFD2 T4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T4, i: 1);
            DisplayButton("MFD2 T5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.T5, i: 1);
            DisplayButton("MFD2 B1", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B1, i: 1);
            DisplayButton("MFD2 B2", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B2, i: 1);
            DisplayButton("MFD2 B3", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B3, i: 1);
            DisplayButton("MFD2 B4", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B4, i: 1);
            DisplayButton("MFD2 B5", "MFD Center", MFD, CMFD.Press, (int)MFDButtons.B5, i: 1);

            DisplayButton("MFD3 L1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L1, i: 2);
            DisplayButton("MFD3 L2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L2, i: 2);
            DisplayButton("MFD3 L3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L3, i: 2);
            DisplayButton("MFD3 L4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L4, i: 2);
            DisplayButton("MFD3 L5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.L5, i: 2);
            DisplayButton("MFD3 R1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R1, i: 2);
            DisplayButton("MFD3 R2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R2, i: 2);
            DisplayButton("MFD3 R3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R3, i: 2);
            DisplayButton("MFD3 R4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R4, i: 2);
            DisplayButton("MFD3 R5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.R5, i: 2);
            DisplayButton("MFD3 T1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 2);
            DisplayButton("MFD3 T2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T2, i: 2);
            DisplayButton("MFD3 T3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T3, i: 2);
            DisplayButton("MFD3 T4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T4, i: 2);
            DisplayButton("MFD3 T5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.T5, i: 2);
            DisplayButton("MFD3 B1", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B1, i: 2);
            DisplayButton("MFD3 B2", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B2, i: 2);
            DisplayButton("MFD3 B3", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B3, i: 2);
            DisplayButton("MFD3 B4", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B4, i: 2);
            DisplayButton("MFD3 B5", "MFD Right", MFD, CMFD.Press, (int)MFDButtons.B5, i: 2);

            DisplayButton("MFD4 L1", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.L1, i: 3);
            DisplayButton("MFD4 L2", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.L2, i: 3);
            DisplayButton("MFD4 L3", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.L3, i: 3);
            DisplayButton("MFD4 L4", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.L4, i: 3);
            DisplayButton("MFD4 L5", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.L5, i: 3);
            DisplayButton("MFD4 R1", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.R1, i: 3);
            DisplayButton("MFD4 R2", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.R2, i: 3);
            DisplayButton("MFD4 R3", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.R3, i: 3);
            DisplayButton("MFD4 R4", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.R4, i: 3);
            DisplayButton("MFD4 R5", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.R5, i: 3);
            DisplayButton("MFD4 T1", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.T1Home, i: 3);
            DisplayButton("MFD4 T2", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.T2, i: 3);
            DisplayButton("MFD4 T3", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.T3, i: 3);
            DisplayButton("MFD4 T4", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.T4, i: 3);
            DisplayButton("MFD4 T5", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.T5, i: 3);
            DisplayButton("MFD4 B1", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.B1, i: 3);
            DisplayButton("MFD4 B2", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.B2, i: 3);
            DisplayButton("MFD4 B3", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.B3, i: 3);
            DisplayButton("MFD4 B4", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.B4, i: 3);
            DisplayButton("MFD4 B5", "MFD Bottom", MFD, CMFD.Press, (int)MFDButtons.B5, i: 3);

            AddPostUpdateControl("MFD Brightness");
            AddPostUpdateControl("SOI");
        }

        protected override void CreateRadioControls()
        {
            RadioAxis("Comm Volume", "Comm Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            RadioButton("Comm Volume +", "Comm Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            RadioButton("Comm Volume -", "Comm Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            RadioButton("Radio Mode Cycle", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioButton("Radio Mode Next", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Mode Prev", "Radio Mode", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            RadioButton("Radio Channel Cycle", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioButton("Radio Channel Next", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Channel Prev", "Radio Channel", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            AddPostUpdateControl("Comm Radio Volume");
        }

        protected override void CreateMusicControls()
        {
            MusicAxis("Radio Volume", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            MusicButton("Radio Volume +", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            MusicButton("Radio Volume -", "Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            MusicButton("Music Play/Pause", "Play/Pause", ByManifest<VRButton, CButton>, CButton.Use, i: 116);
            MusicButton("Music Next", "Next Song", ByManifest<VRButton, CButton>, CButton.Use, i: 115);
            MusicButton("Music Previous", "Prev Song", ByManifest<VRButton, CButton>, CButton.Use, i: 114);

            AddPostUpdateControl("Radio Volume");
        }

        protected override void CreateLightsControls()
        {
            // game typo is Formation LIghts
            LightsButton("Formation Lights Cycle", "Formation LIghts", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            LightsButton("Formation Lights Next", "Formation LIghts", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            LightsButton("Formation Lights Prev", "Formation LIghts", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            LightsButton("Nav Lights Cycle", "Nav Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            LightsButton("Nav Lights Next", "Nav Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            LightsButton("Nav Lights Prev", "Nav Lights", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            LightsButton("Landing Light Toggle", "Landing Light", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Landing Light On", "Landing Light", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            LightsButton("Landing Light Off", "Landing Light", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            LightsAxis("Console Lights", "Console Lights", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            LightsButton("Console Lights +", "Console Lights", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            LightsButton("Console Lights -", "Console Lights", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            LightsButton("Instrument Lights Night Mode", "Instrument Lights Night Mode", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            AddPostUpdateControl("Console Lights");
        }

        protected override void CreateMiscControls()
        {
            MiscButton("Canopy Toggle", "Canopy", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            MiscButton("Canopy Open", "Canopy", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            MiscButton("Canopy Close", "Canopy", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            MiscButton("Raise Seat", "Raise Seat", ByManifest<VRButton, CButton>, CButton.Use, i: 30);
            MiscButton("Lower Seat", "Lower Seat", ByManifest<VRButton, CButton>, CButton.Use, i: 31);

            MiscButton("Quickstart Engines", "Quickstart Engines", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MiscButton("QuickFly", "QuickFly", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }
    }
}
