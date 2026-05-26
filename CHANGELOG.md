# Changelog

## v1.3 — AQ-39 Attack Drone support & config UI fix

Adds full BYOJ support for the [AQ-39 Attack Drone](https://steamcommunity.com/sharedfiles/filedetails/?id=3572803786) mod aircraft, mapped from in-game manifest/interactable dumps. Fixes the config menu breaking when duplicate action names were registered during manager init.

### AQ-39 Attack Drone (new)
- **New manager:** flight (side + center stick, throttle, brakes axis, carrier controls), systems (master arm, engine/APU/battery, CMS chaff/flares, radar, jettison), navigation (MFD autopilot buttons, clear waypoint), displays (SOI, MFD power/brightness, full MFD edge keys), numpad, radio, lights, helmet visor/NVG
- **Vehicle names:** recognises both `AQ-39` and `AQ-39 Attack Drone`
- **Fire Countermeasures:** throttle menu button (Flight category)

### Fixes
- **Config UI crash:** duplicate `Fire Weapon` action key during `AQ39Manager` init aborted manager setup and left the binding screen showing only partial Flight rows — resolved by keeping weapon bindings in Flight only
- **Config UI layout:** BYOJ prefab canvas scale/anchors restored so the configuration screen fills the display correctly

### Dev
- Per-aircraft verbose logging unchanged — set `Plugin.VerboseInteractableDiscoveryForShortName` to `"AQ39"` (or `"ALL"`) in source before building to dump interactables

---

## v1.2 — HOTAS brake axis, F-22 / SU-47M / F-45 fixes & mapping tools

Fixed brake/airbrake axis bindings across all supported jets (axis inputs were ignored because *GetAsBool()* never fires on joystick axes). Major mapping fixes for F-22, SU-47M, F-45A throttle tilt, AH-6 & EF-24G SOI, plus per-aircraft debug logging for modders.

### All aircraft
- **Brakes/Airbrakes axis:** new *CThrottle.TriggerAxis* reads axis float + deadzone (not *GetAsBool*); all brake-axis bindings updated (A-10D, AV-42C, F/A-26B, F-45A, F-22, F-5E, SU-47M, T-55, EF-24G)
- **Logs:** interactable/manifest dumps no longer spam every spawn — set *Plugin.VerboseInteractableDiscoveryForShortName* to a jet short name (e.g. `"F22"`, `"SU47M"`, or `"ALL"`) in source before building

### F-22
- **Joystick:** *CockpitJoystick* from *VehicleControlManifest* (replaces broken path-based *Joysticks()*)
- **Radar / RWR:** knobs resolved by object name (*RadarPowerInteractable*, *RWRModeInteractable*) — avoids manifest index collisions with MFD buttons
- **SOI:** full slew / next / prev / zoom bindings + post-update
- **HUD/HMCS power:** removed non-working ICP roller *VRInteractable.Use* bindings (rollers need helmet API — *CHelmet* helpers added for future wiring)

### SU-47M
- **Joystick:** pitch/yaw/roll + fire/cycle weapons on manifest stick (was throttle-only)
- **A/P Alt Hold:** maps *AltitudeAPButton* (game mislabels it near Heading Hold)
- **Countermeasures:** moved to throttle *MenuButton*; weapons stay on stick
- **SOI:** slew / zoom / next / prev added to HUD category

### F-45A
- **Throttle / AB:** *CThrottleTilt* always applies latest throttle + button smoothing delta — fixes afterburner stuck ~90% while holding throttle-up/down

### AH-6
- **SOI:** slew axes/buttons, next/prev, zoom in/out (Display category)
- **MFD Brightness:** post-update enabled

### EF-24G (Front & Rear)
- **SOI:** *Radar Elev Up* / *Radar Elev Down* bindings added

### Dev / repo
- *TryCreateFieldGetter/Setter* in *CompiledExpressions* for optional helmet fields
- *tools/TypeInspect/* gitignored

---

## v1.1 — SU-47M, F-5E & AV-42 Replica Fixes

Fixed joystick binding crash that caused AV duplication and broken controls on both aircraft.

- **SU-47M:** corrected Nav Lights & Strobe Lights to *VRLever* (were incorrectly *VRTwistKnobInt*)
- **F-5E:** corrected *Catapult TO Trim* lever name to *Cato Trim* (actual in-game name)
- **Both:** removed broken *Joysticks()* method — weapon fire/cycle now correctly bound to throttle manifest
