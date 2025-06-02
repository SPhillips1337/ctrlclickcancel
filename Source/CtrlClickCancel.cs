using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CtrlClickCancel
{
    [StaticConstructorOnStartup]
    public static class CtrlClickCancel
    {
        static CtrlClickCancel()
        {
            var harmony = new Harmony("Stephen.CtrlClickCancel");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("CtrlClickCancel initialized");
        }

        // Helper method to check if Ctrl key is pressed
        public static bool IsCtrlPressed()
        {
            bool isPressed = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
            if (isPressed)
            {
                Log.Message("CtrlClickCancel: Ctrl key is pressed");
            }
            return isPressed;
        }
    }

    // Patch for handling mouse clicks in the game world
    [HarmonyPatch(typeof(Designator), "DesignateMultiCell")]
    public static class Designator_DesignateMultiCell_Patch
    {
        public static bool Prefix(Designator __instance, IEnumerable<IntVec3> cells)
        {
            if (CtrlClickCancel.IsCtrlPressed())
            {
                Log.Message("CtrlClickCancel: Intercepted DesignateMultiCell with Ctrl pressed");
                foreach (IntVec3 cell in cells)
                {
                    CancelDesignationsAt(cell);
                }
                return false;
            }
            return true;
        }

        private static void CancelDesignationsAt(IntVec3 cell)
        {
            Map map = Find.CurrentMap;
            if (map == null) return;

            Log.Message($"CtrlClickCancel: Attempting to cancel at {cell}");

            // Get all designations at the cell
            List<Designation> designationsAtCell = map.designationManager.AllDesignationsAt(cell).ToList();
            
            // Cancel all designations at the cell
            foreach (Designation designation in designationsAtCell)
            {
                Log.Message($"CtrlClickCancel: Removing designation {designation.def} at {cell}");
                map.designationManager.RemoveDesignation(designation);
            }

            // Also check for blueprint/frame at the cell
            List<Thing> thingsAtCell = map.thingGrid.ThingsListAt(cell);
            foreach (Thing thing in thingsAtCell)
            {
                // Check if it's a blueprint or frame
                if (thing is Blueprint || thing is Frame)
                {
                    Log.Message($"CtrlClickCancel: Destroying {thing} at {cell}");
                    thing.Destroy();
                }
            }
        }
    }

    // Patch for the Designator class to intercept clicks
    [HarmonyPatch(typeof(Designator), "DesignateSingleCell")]
    public static class Designator_DesignateSingleCell_Patch
    {
        public static bool Prefix(Designator __instance, IntVec3 c)
        {
            // Check if Ctrl key is pressed
            if (CtrlClickCancel.IsCtrlPressed())
            {
                Log.Message("CtrlClickCancel: Intercepted DesignateSingleCell with Ctrl pressed");
                // Find and cancel designations at the cell
                CancelDesignationsAt(c);
                return false; // Skip original method
            }
            return true; // Continue with original method
        }

        private static void CancelDesignationsAt(IntVec3 cell)
        {
            Map map = Find.CurrentMap;
            if (map == null) return;

            Log.Message($"CtrlClickCancel: Attempting to cancel at {cell}");

            // Get all designations at the cell
            List<Designation> designationsAtCell = map.designationManager.AllDesignationsAt(cell).ToList();
            
            // Cancel all designations at the cell
            foreach (Designation designation in designationsAtCell)
            {
                Log.Message($"CtrlClickCancel: Removing designation {designation.def} at {cell}");
                map.designationManager.RemoveDesignation(designation);
            }

            // Also check for blueprint/frame at the cell
            List<Thing> thingsAtCell = map.thingGrid.ThingsListAt(cell);
            foreach (Thing thing in thingsAtCell)
            {
                // Check if it's a blueprint or frame
                if (thing is Blueprint || thing is Frame)
                {
                    Log.Message($"CtrlClickCancel: Destroying {thing} at {cell}");
                    thing.Destroy();
                }
            }
        }
    }

    // Patch for the Designator_Build class to intercept clicks
    [HarmonyPatch(typeof(Designator_Build), "DesignateSingleCell")]
    public static class Designator_Build_DesignateSingleCell_Patch
    {
        public static bool Prefix(Designator_Build __instance, IntVec3 c)
        {
            // Check if Ctrl key is pressed
            if (CtrlClickCancel.IsCtrlPressed())
            {
                Log.Message("CtrlClickCancel: Intercepted Designator_Build.DesignateSingleCell with Ctrl pressed");
                // Find and cancel designations at the cell
                CancelDesignationsAt(c);
                return false; // Skip original method
            }
            return true; // Continue with original method
        }

        private static void CancelDesignationsAt(IntVec3 cell)
        {
            Map map = Find.CurrentMap;
            if (map == null) return;

            Log.Message($"CtrlClickCancel: Attempting to cancel at {cell}");

            // Get all designations at the cell
            List<Designation> designationsAtCell = map.designationManager.AllDesignationsAt(cell).ToList();
            
            // Cancel all designations at the cell
            foreach (Designation designation in designationsAtCell)
            {
                Log.Message($"CtrlClickCancel: Removing designation {designation.def} at {cell}");
                map.designationManager.RemoveDesignation(designation);
            }

            // Also check for blueprint/frame at the cell
            List<Thing> thingsAtCell = map.thingGrid.ThingsListAt(cell);
            foreach (Thing thing in thingsAtCell)
            {
                // Check if it's a blueprint or frame
                if (thing is Blueprint || thing is Frame)
                {
                    Log.Message($"CtrlClickCancel: Destroying {thing} at {cell}");
                    thing.Destroy();
                }
            }
        }
    }
}
