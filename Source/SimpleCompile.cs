using System;
using System.Collections.Generic;
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
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }
        
        // Centralized method to cancel designations at a cell
        public static void CancelDesignationsAt(IntVec3 cell)
        {
            Map map = Find.CurrentMap;
            if (map == null) return;

            Log.Message($"CtrlClickCancel: Attempting to cancel at {cell}");
            bool anythingCanceled = false;

            // Get all designations at the cell
            List<Designation> designationsAtCell = map.designationManager.AllDesignationsAt(cell).ToList();
            
            // Cancel all designations at the cell
            foreach (Designation designation in designationsAtCell)
            {
                Log.Message($"CtrlClickCancel: Removing designation {designation.def} at {cell}");
                map.designationManager.RemoveDesignation(designation);
                anythingCanceled = true;
            }

            // Also check for blueprint/frame at the cell
            List<Thing> thingsAtCell = map.thingGrid.ThingsListAt(cell).ToList();
            foreach (Thing thing in thingsAtCell)
            {
                // Check if it's a blueprint or frame
                if (thing is Blueprint || thing is Frame)
                {
                    Log.Message($"CtrlClickCancel: Destroying {thing} at {cell}");
                    thing.Destroy();
                    anythingCanceled = true;
                }
            }
            
            if (anythingCanceled)
            {
                // Play a sound to indicate successful cancellation
                SoundDefOf.Designate_Cancel.PlayOneShotOnCamera();
            }
        }
    }

    // Patch for Designator.ProcessInput to handle mouse clicks
    [HarmonyPatch(typeof(Designator), "ProcessInput")]
    public static class Designator_ProcessInput_Patch
    {
        public static bool Prefix(Designator __instance, Event ev)
        {
            // Only handle mouse down events when Ctrl is pressed
            if (CtrlClickCancel.IsCtrlPressed() && ev.type == EventType.MouseDown && ev.button == 0)
            {
                Log.Message("CtrlClickCancel: Ctrl+Click detected in ProcessInput");
                
                // Get the cell under the mouse
                IntVec3 cell = UI.MouseCell();
                if (cell.InBounds(Find.CurrentMap))
                {
                    CtrlClickCancel.CancelDesignationsAt(cell);
                    // Consume the event
                    Event.current.Use();
                    return false;
                }
            }
            return true;
        }
    }

    // Patch for Designator.DesignateSingleCell
    [HarmonyPatch(typeof(Designator), "DesignateSingleCell")]
    public static class Designator_DesignateSingleCell_Patch
    {
        public static bool Prefix(Designator __instance, IntVec3 c)
        {
            if (CtrlClickCancel.IsCtrlPressed())
            {
                Log.Message("CtrlClickCancel: Intercepted DesignateSingleCell with Ctrl pressed");
                CtrlClickCancel.CancelDesignationsAt(c);
                return false; // Skip original method
            }
            return true; // Continue with original method
        }
    }

    // Patch for Designator.DesignateMultiCell
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
                    CtrlClickCancel.CancelDesignationsAt(cell);
                }
                return false; // Skip original method
            }
            return true; // Continue with original method
        }
    }
    
    // Patch for Designator_Build.DesignateSingleCell to ensure it works with building designators
    [HarmonyPatch(typeof(Designator_Build), "DesignateSingleCell")]
    public static class Designator_Build_DesignateSingleCell_Patch
    {
        public static bool Prefix(Designator_Build __instance, IntVec3 c)
        {
            if (CtrlClickCancel.IsCtrlPressed())
            {
                Log.Message("CtrlClickCancel: Intercepted Designator_Build.DesignateSingleCell with Ctrl pressed");
                CtrlClickCancel.CancelDesignationsAt(c);
                return false; // Skip original method
            }
            return true; // Continue with original method
        }
    }
    
    // Patch to handle mouse input directly
    [HarmonyPatch(typeof(MapInterface), "HandleMapClicks")]
    public static class MapInterface_HandleMapClicks_Patch
    {
        public static bool Prefix()
        {
            // If Ctrl is pressed and left mouse button is clicked
            if (CtrlClickCancel.IsCtrlPressed() && Input.GetMouseButtonDown(0))
            {
                Log.Message("CtrlClickCancel: Ctrl+Click detected in HandleMapClicks");
                
                // Get the cell under the mouse
                IntVec3 cell = UI.MouseCell();
                if (cell.InBounds(Find.CurrentMap))
                {
                    CtrlClickCancel.CancelDesignationsAt(cell);
                    return false; // Skip original method
                }
            }
            return true; // Continue with original method
        }
    }
}
