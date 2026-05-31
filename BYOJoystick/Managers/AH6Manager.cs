using BYOJoystick.Controls;
using BYOJoystick.Managers.Base;
using VTOLVR.Multiplayer;

namespace BYOJoystick.Managers
{
    public class AH6Manager : Manager
    {
        public override string GameName    => "AH-6 Little Bird";
        public override string ShortName   => "AH6";
        public override bool   IsMulticrew => false;

       // private static string SideJoystick => "PassengerOnlyObjects/localCockpit/controls.001/vtol4adjustableJoystick_rear";
        private static string CenterJoystick => "AH-6_V1/Fuselage/Front_Interior/StickMount/PilotStickParent/Pi_Pitch/Pi_Roll/centerJoystickBar.001/ergo/cYaw/centerJoystick.001/centerJoyInteractable";

        private static string BailHandle => "PassengerOnlyObjects/bailBase_rear";

        // Custom method to get the cyclic (joystick) by finding it in the interactables
        private CJoystick GetCyclic(string name, string root, bool nullable, bool checkName, int idx)
        {
            if (TryGetExistingControl<CJoystick>(name, out var existingControl))
                return existingControl;

            // Find the VRJoystick by name in the interactables
            var interactable = FindInteractable(name, Interactables, nullable);
            if (interactable == null)
                return null;

            var vrJoystick = interactable.GetComponent<VRJoystick>();
            if (vrJoystick == null)
            {
                if (nullable)
                    return null;
                throw new System.InvalidOperationException($"Interactable {name} does not have VRJoystick component.");
            }

            // Create CJoystick control - for helicopter, we use null for the center stick since there's only one cyclic
            var control = new CJoystick(Vehicle, vrJoystick, null, IsMulticrew, false);
            Controls.Add(name, control);
            return control;
        }

        private CJoystick Joysticks(string name, string root, bool nullable, bool checkName, int idx)
        {
            return GetJoysticksByPaths(name, CenterJoystick, null);
        }

        protected override void PreMapping()
        {
            LogInteractablesIfEnabled(ShortName);
        }

        protected override void CreateFlightControls()
        {
            // Cyclic (helicopter flight stick) - using custom GetCyclic method
            FlightAxisC("Cyclic Pitch", "Cyclic (Rear)", Joysticks, CJoystick.SetPitch);
            FlightAxisC("Cyclic Roll", "Cyclic (Rear)", Joysticks, CJoystick.SetRoll);
            FlightAxisC("Cyclic Yaw", "Cyclic (Rear)", Joysticks, CJoystick.SetYaw);

            // Collective (helicopter throttle/altitude control)
            // Manifest shows collectives array with 3 entries - try accessing by manifest
            FlightAxis("Collective", "Flight Collective (Rear)", ByName<VRThrottle, CThrottle>, CThrottle.Set, s: -1, n: true);
            FlightButton("Collective Increase", "Flight Collective (Rear)", ByName<VRThrottle, CThrottle>, CThrottle.Increase, s: -1, n: true);
            FlightButton("Collective Decrease", "Flight Collective (Rear)", ByName<VRThrottle, CThrottle>, CThrottle.Decrease, s: -1, n: true);

            // Power Lever (engine throttle)
            FlightAxis("Power Lever", "Power Lever (Rear)", ByName<VRThrottle, CThrottle>, CThrottle.Set, s: -1, n: true);
            FlightButton("Power Lever Increase", "Power Lever (Rear)", ByName<VRThrottle, CThrottle>, CThrottle.Increase, s: -1, n: true);
            FlightButton("Power Lever Decrease", "Power Lever (Rear)", ByName<VRThrottle, CThrottle>, CThrottle.Decrease, s: -1, n: true);

            // Landing Gear
            FlightButton("Landing Gear Toggle", "Landing Gear (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Landing Gear Up", "Landing Gear (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Landing Gear Down", "Landing Gear (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // Parking Brake
            FlightButton("Parking Brake Toggle", "Parking Brake", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Parking Brake On", "Parking Brake", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Parking Brake Off", "Parking Brake", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // Rotor Controls (helicopter-specific)
            FlightButton("Rotor Brake Toggle", "Rotor Brake", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            FlightButton("Rotor Brake On", "Rotor Brake", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            FlightButton("Rotor Brake Off", "Rotor Brake", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
            
           // FlightButton("Rotor Fold Toggle", "Rotor Fold", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);

            // Weapons - using the same cyclic control
            FlightButton("Fire Weapon", "Cyclic (Rear)", GetCyclic, CJoystick.Trigger);
            FlightButton("Cycle Weapons", "Cyclic (Rear)", GetCyclic, CJoystick.MenuButton);

            AddPostUpdateControl("Cyclic (Rear)");
            AddPostUpdateControl("Flight Collective (Rear)");
            AddPostUpdateControl("Power Lever (Rear)");
        }

        protected override void CreateAssistControls()
        {
            // SAS (Stability Augmentation System) - helicopter flight assist
            AssistAxis("SAS Level", "SAS Level (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            AssistButton("SAS Increase", "SAS Level (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            AssistButton("SAS Decrease", "SAS Level (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            // Pitch Trim
            AssistButton("Toggle Pitch Trim", "Toggle Pitch Trim", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
        }

        protected override void CreateNavigationControls()
        {
            // Autopilot controls (helicopter-specific modes)
            NavButton("A/P Nav Mode", "Nav AP (Rear)", ByName<VRInteractable, CInteractable>, CInteractable.Use, s: -1, n: true);
            NavButton("A/P Alt Hold", "Altitude AP (Rear)", ByName<VRInteractable, CInteractable>, CInteractable.Use, s: -1, n: true);
            NavButton("A/P Hdg Hold", "Heading AP (Rear)", ByName<VRInteractable, CInteractable>, CInteractable.Use, s: -1, n: true);
            NavButton("A/P Hover", "Hover AP (Rear)", ByName<VRInteractable, CInteractable>, CInteractable.Use, s: -1, n: true);
            NavButton("A/P Off", "AP Off (Rear)", ByName<VRInteractable, CInteractable>, CInteractable.Use, s: -1, n: true);

            // AP adjustments
            NavButton("A/P Heading Right", "Heading Right (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Heading Left", "Heading Left (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Alt +", "AP Alt + (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            NavButton("A/P Alt -", "AP Alt - (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // Waypoint
            NavButton("Clear Waypoint", "Clear Waypoint", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }

        protected override void CreateSystemsControls()
        {
            // Battery & Power
            SystemsButton("Battery Toggle", "Main Battery (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Battery On", "Main Battery (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Battery Off", "Main Battery (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("APU Toggle", "Auxilliary Power (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("APU On", "Auxilliary Power (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("APU Off", "Auxilliary Power (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // Engine Starters (two engines)
            SystemsButton("Starter Left Toggle", "Starter Left (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Starter Left On", "Starter Left (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Starter Left Off", "Starter Left (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            SystemsButton("Starter Right Toggle", "Starter Right (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            SystemsButton("Starter Right On", "Starter Right (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            SystemsButton("Starter Right Off", "Starter Right (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // Master Arm
            SystemsButton("Master Arm", "Master Arm (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Master Safe", "Master Safe (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // Countermeasures
            SystemsButton("Toggle Chaff", "Toggle Chaff (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Toggle Flares", "Toggle Flares (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("CM Release Mode", "Countermeasures Release Mode", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("CM Rate +", "Countermeasures Release Rate +", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("CM Rate -", "Countermeasures Release Rate -", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // Combat collective grip button fires countermeasures
            SystemsButton("Fire Countermeasures", "Combat Collective (Rear)", ByName<VRThrottle, CThrottle>, CThrottle.MenuButton, s: -1, n: true);

            // Jettison
            SystemsButton("Jettison", "Jettison (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Mark Jett L TIP", "Mark Jett L TIP (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            SystemsButton("Mark Jett R TIP", "Mark Jett R TIP (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // Caution
            SystemsButton("Master Caution", "Master Caution (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // MFD Power
            SystemsButton("MFD1 Power Cycle", "MFD1 Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("MFD2 Power Cycle", "MFD2 Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("MFD3 Power Cycle", "MFD3 Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);

            // RWR
            SystemsButton("RWR Power Cycle", "RWR Power (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            SystemsButton("ADI Power Cycle", "ADI Power", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
        }

        protected override void CreateHUDControls()
        {
            // Helmet controls
            HUDButton("Helmet Visor Toggle", "Visor", HelmetController, CHelmet.ToggleVisor, s: -1, n: true);
            HUDButton("Helmet NV Toggle", "NVG", HelmetController, CHelmet.ToggleNightVision, s: -1, n: true);

            // HMD (Helmet Mounted Display)
            HUDButton("HMD Power Cycle", "HMD Power (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            HUDAxis("HMD Brightness", "HMD Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            HUDButton("HMD Brightness +", "HMD Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            HUDButton("HMD Brightness -", "HMD Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);
        }

        protected override void CreateNumPadControls()
        {
            // UFD Numpad controls (if accessible via VRButton)
            // These appear to be "Unknown" type in the manifest, so we'll skip for now
        }

        protected override void CreateDisplayControls()
        {
            // SOI Slew
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

            // UFD (Upfront Display) buttons
            DisplayButton("UFD Fuel", "UFD Fuel (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("UFD Autopilot", "UFD Autopilot (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("UFD Power", "UFD Power (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            DisplayButton("UFD Status", "UFD Status (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // MFD Swap
            DisplayButton("Swap MFDs", "Swap MFDs (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // MFD Brightness
            DisplayAxis("MFD Brightness", "MFD Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            DisplayButton("MFD Brightness +", "MFD Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            DisplayButton("MFD Brightness -", "MFD Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            AddPostUpdateControl("MFD Brightness (Rear)");
            AddPostUpdateControl("SOI");
        }

        protected override void CreateRadioControls()
        {
            RadioButton("Radio Transmit", "Radio", ByType<CockpitTeamRadioManager, CRadio>, CRadio.Transmit, s: -1, n: true);

            // Comm Radio
            RadioAxis("Comm Volume", "Comm Radio Volume (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            RadioButton("Comm Volume +", "Comm Radio Volume (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            RadioButton("Comm Volume -", "Comm Radio Volume (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            RadioButton("Radio Mode Cycle", "Radio Mode (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioButton("Radio Mode Next", "Radio Mode (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Mode Prev", "Radio Mode (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            RadioButton("Radio Channel Cycle", "Radio Channel (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioButton("Radio Channel Next", "Radio Channel (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Next, s: -1, n: true);
            RadioButton("Radio Channel Prev", "Radio Channel (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Prev, s: -1, n: true);

            // Intra Comms (intercom)
            RadioButton("Intra Comms Cycle", "Intra Comms (Rear)", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
            RadioAxis("Intra Comm Volume", "Intra Comm Volume (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            RadioButton("Intra Comm Volume +", "Intra Comm Volume (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            RadioButton("Intra Comm Volume -", "Intra Comm Volume (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);
        }

        protected override void CreateMusicControls()
        {
            // MP3 Player
            MusicAxis("MP3 Volume", "MP3 Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            MusicButton("MP3 Volume +", "MP3 Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            MusicButton("MP3 Volume -", "MP3 Radio Volume", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            MusicButton("Prev Song", "Prev Song", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MusicButton("Next Song", "Next Song", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MusicButton("Play/Pause", "Play/Pause", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
        }

        protected override void CreateLightsControls()
        {
            // Interior Lights
            LightsButton("Interior Lights Toggle", "Interior Lights (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Interior Lights On", "Interior Lights (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            LightsButton("Interior Lights Off", "Interior Lights (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // Instrument Lights
            LightsButton("Instrument Lights Toggle", "Instrument Lights (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsAxis("Instrument Brightness", "Instrument Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Set, s: -1, n: true);
            LightsButton("Instrument Brightness +", "Instrument Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Increase, s: -1, n: true);
            LightsButton("Instrument Brightness -", "Instrument Brightness (Rear)", ByName<VRTwistKnob, CKnob>, CKnob.Decrease, s: -1, n: true);

            // Landing Lights
            LightsButton("Landing Lights Toggle", "Landing Lights (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Landing Lights On", "Landing Lights (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            LightsButton("Landing Lights Off", "Landing Lights (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // Strobe Lights
            LightsButton("Strobe Lights Toggle", "Strobe Lights (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Strobe Lights On", "Strobe Lights (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            LightsButton("Strobe Lights Off", "Strobe Lights (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);

            // Nav Lights
            LightsButton("Nav Lights Toggle", "Nav Lights (Rear)", ByName<VRLever, CLever>, CLever.Cycle, s: -1, n: true);
            LightsButton("Nav Lights On", "Nav Lights (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 1, n: true);
            LightsButton("Nav Lights Off", "Nav Lights (Rear)", ByName<VRLever, CLever>, CLever.Set, s: 0, n: true);
        }

        protected override void CreateMiscControls()
        {
            // Seat adjustment
            MiscButton("Raise Seat", "Raise Seat", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);
            MiscButton("Lower Seat", "Lower Seat", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            // Seat switching (unique to side-by-side helicopters)
            MiscButton("Switch Seat", "Switch Seat (Rear)", ByName<VRButton, CButton>, CButton.Use, s: -1, n: true);

            MiscButton("Eject", "Eject", ByType<EjectHandle, CEject>, CEject.Pull, r: BailHandle, n: true);

            // Control Override
            MiscButton("Control Override Cycle", "Control Override", ByName<VRTwistKnobInt, CKnobInt>, CKnobInt.Cycle, s: -1, n: true);
        }
    }
}