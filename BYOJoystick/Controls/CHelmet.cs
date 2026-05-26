using System;
using BYOJoystick.Bindings;
using UnityEngine;

namespace BYOJoystick.Controls
{
    public class CHelmet : IControl
    {
        protected readonly HelmetController               HelmetController;
        protected readonly Action<HelmetController, bool> SetVisorDown;
        protected readonly Action<HelmetController, bool> SetNVGEnabled;
        protected readonly Func<HelmetController, bool>           GetIsPowered;
        protected readonly Func<HelmetController, bool>           GetHmcsEnabled;
        protected readonly Action<HelmetController, bool>         SetHmcsEnabled;
        protected readonly Func<HelmetController, HUDMaskToggler> GetHudMaskToggler;
        protected readonly Func<HelmetController, GameObject>     GetHudPowerObject;
        protected readonly Func<HelmetController, GameObject>     GetHmcsDisplayObject;
        protected readonly Func<HelmetController, Coroutine>        GetHmcsUpdateRoutine;
        protected readonly Action<HelmetController, Coroutine>      SetHmcsUpdateRoutine;

        public CHelmet(HelmetController helmetController)
        {
            HelmetController = helmetController;
            SetVisorDown     = CompiledExpressions.CreateFieldSetter<HelmetController, bool>("visorDown");
            SetNVGEnabled    = CompiledExpressions.CreateFieldSetter<HelmetController, bool>("nvgEnabled");
            GetIsPowered     = CompiledExpressions.TryCreateFieldGetter<HelmetController, bool>("isPowered");
            GetHmcsEnabled   = CompiledExpressions.TryCreateFieldGetter<HelmetController, bool>("hmcsEnabled");
            SetHmcsEnabled   = CompiledExpressions.TryCreateFieldSetter<HelmetController, bool>("hmcsEnabled");
            GetHudMaskToggler    = CompiledExpressions.TryCreateFieldGetter<HelmetController, HUDMaskToggler>("hudMaskToggler");
            GetHudPowerObject    = CompiledExpressions.TryCreateFieldGetter<HelmetController, GameObject>("hudPowerObject");
            GetHmcsDisplayObject = CompiledExpressions.TryCreateFieldGetter<HelmetController, GameObject>("hmcsDisplayObject");
            GetHmcsUpdateRoutine = CompiledExpressions.TryCreateFieldGetter<HelmetController, Coroutine>("hmcsUpdateRoutine");
            SetHmcsUpdateRoutine = CompiledExpressions.TryCreateFieldSetter<HelmetController, Coroutine>("hmcsUpdateRoutine");
        }

        public void PostUpdate()
        {
        }

        public static void ToggleVisor(CHelmet c, Binding binding, int state)
        {
            if (binding.GetAsBool())
                c.HelmetController.ToggleVisor();
        }

        public static void OpenVisor(CHelmet c, Binding binding, int state)
        {
            if (binding.GetAsBool())
            {
                c.SetVisorDown(c.HelmetController, true);
                c.HelmetController.ToggleVisor();
            }
        }

        public static void CloseVisor(CHelmet c, Binding binding, int state)
        {
            if (binding.GetAsBool())
            {
                c.SetVisorDown(c.HelmetController, false);
                c.HelmetController.ToggleVisor();
            }
        }

        public static void ToggleNightVision(CHelmet c, Binding binding, int state)
        {
            if (binding.GetAsBool())
                c.HelmetController.ToggleNVG();
        }

        public static void EnableNightVision(CHelmet c, Binding binding, int state)
        {
            if (binding.GetAsBool())
            {
                c.SetNVGEnabled(c.HelmetController, false);
                c.HelmetController.ToggleNVG();
            }
        }

        public static void DisableNightVision(CHelmet c, Binding binding, int state)
        {
            if (binding.GetAsBool())
            {
                c.SetNVGEnabled(c.HelmetController, true);
                c.HelmetController.ToggleNVG();
            }
        }

        // F-22 ICP rollers use motion-validated VRInteractable; game wires them to HelmetController.SetPower / RefreshHMCSUpdate
        public static void SetHudPower(CHelmet c, Binding binding, int state)
        {
            if (!binding.GetAsBool())
                return;
            // Toggle: if we cannot read current state, default to ON (better than forcing OFF).
            var on = state < 0
                ? (c.GetIsPowered != null ? !c.GetIsPowered(c.HelmetController) : true)
                : state > 0;
            ApplyHudPower(c, on);
        }

        public static void SetHmcsPower(CHelmet c, Binding binding, int state)
        {
            if (!binding.GetAsBool())
                return;
            // Toggle: if we cannot read current state, default to ON (better than forcing OFF).
            var on = state < 0
                ? (c.GetHmcsEnabled != null ? !c.GetHmcsEnabled(c.HelmetController) : true)
                : state > 0;
            ApplyHmcsPower(c, on);
        }

        private static void ApplyHudPower(CHelmet c, bool on)
        {
            c.HelmetController.SetPower(on ? 1 : 0);

            if (c.GetHudPowerObject != null)
            {
                var hudPowerObject = c.GetHudPowerObject(c.HelmetController);
                if (hudPowerObject != null)
                    hudPowerObject.SetActive(on);
            }

            if (c.GetHudMaskToggler != null)
            {
                var hudMask = c.GetHudMaskToggler(c.HelmetController);
                if (hudMask != null && hudMask.hudLayerPower != null)
                    hudMask.hudLayerPower.SetConnection(on ? 1 : 0);
            }
        }

        private static void ApplyHmcsPower(CHelmet c, bool on)
        {
            if (on)
            {
                c.HelmetController.RefreshHMCSUpdate();
                return;
            }

            if (c.GetHmcsUpdateRoutine != null && c.SetHmcsUpdateRoutine != null)
            {
                var routine = c.GetHmcsUpdateRoutine(c.HelmetController);
                if (routine != null)
                    c.HelmetController.StopCoroutine(routine);
                c.SetHmcsUpdateRoutine(c.HelmetController, null);
            }

            if (c.SetHmcsEnabled != null)
                c.SetHmcsEnabled(c.HelmetController, false);

            if (c.GetHmcsDisplayObject != null)
            {
                var hmcsDisplay = c.GetHmcsDisplayObject(c.HelmetController);
                if (hmcsDisplay != null)
                    hmcsDisplay.SetActive(false);
            }
        }
    }
}