# v1.3 — AQ-39 Attack Drone support & config UI fix

Adds full BYOJ support for the [AQ-39 Attack Drone](https://steamcommunity.com/sharedfiles/filedetails/?id=3572803786) mod aircraft, mapped from in-game manifest/interactable dumps.

## AQ-39 Attack Drone (new)
- **New manager:** flight (side + center stick, throttle, brakes axis, carrier controls), systems (master arm, engine/APU/battery, CMS chaff/flares, radar, jettison), navigation (MFD autopilot buttons, clear waypoint), displays (SOI, MFD power/brightness, full MFD edge keys), numpad, radio, lights, helmet visor/NVG
- **Vehicle names:** recognises both `AQ-39` and `AQ-39 Attack Drone`

## Fixes
- **Config UI layout:** BYOJ prefab canvas scale/anchors restored so the configuration screen fills the display correctly

## Dev
- Per-aircraft verbose logging unchanged — set `Plugin.VerboseInteractableDiscoveryForShortName` to `"AQ39"` (or `"ALL"`) in source before building to dump interactables

