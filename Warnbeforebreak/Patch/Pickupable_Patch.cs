using HarmonyLib;

//transpiller
using System.Collections.Generic;
using System.Linq;
//using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Warnbeforebreak.Patches
{
    [HarmonyPatch(typeof(Pickupable))]
    [HarmonyPatch(nameof(Pickupable.OnHandHover))]
    public static class Pickupable_OnHandHover_Patch
    {
        //Debug Logging - Deactivate before shipping
        private static bool debuglogging = false;
        //Deep Logging
        private static bool deeplogging = false;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            string classnamefordocu = "Pickupable_OnHandHover_Patch";
            if ((debuglogging && !deeplogging))
            {
                Plugin.Logger.LogDebug( "Deeploging deactivated");
            }

            if(debuglogging)
            {
                Plugin.Logger.LogDebug($"Start Transpiler - {classnamefordocu}");
            }

            //var replacefunc = typeof(Pickupable_OnHandHover_Patch).GetMethod("replacefunc", BindingFlags.Public | BindingFlags.Static);
            bool found = false;
            var Index = -1;
            var codes = new List<CodeInstruction>(instructions);

            //logging before
            if (debuglogging && deeplogging)
            {
                Plugin.Logger.LogDebug( "Deep Logging pre-transpiler:");
                for (int k = 0; k < codes.Count; k++)
                {
                    Plugin.Logger.LogDebug( (string.Format("0x{0:X4}", k) + $" : {codes[k].opcode.ToString()}	{(codes[k].operand != null ? codes[k].operand.ToString() : "")}"));
                }
            }

            //analyse the code to find the right place for injection
            if (debuglogging)
            {
                Plugin.Logger.LogDebug( "Start code analyses");
            }

            for (var i = 0; i < codes.Count; i++)
            {

                /*
IL_00F9: ldloc.3        <-------------------------------------------------------------------------------------------------------------- 0
IL_00FA: ldc.i4.0                                                                                                                       1
IL_00FB: ldc.i4.s  44                                                                                                                   2
IL_00FD: callvirt instance void HandReticle::SetText(valuetype HandReticle / TextType, string, bool, valuetype GameInput / Button)      3 
IL_0102: ret                                                                                                                            4
IL_0103: ldarg.0                                                                                                                        5
                
//---------------------

0x004D : callvirt	Void SetText(TextType, System.String, Boolean, Button)      -16
0x004E : ldsfld	HandReticle main                                                -15
0x004F : ldc.i4.1	                                                            -14
0x0050 : ldloc.3	                                                            -13 HandReticle.main.SetText(HandReticle.TextType.HandSubscript, text2, false, GameInput.Button.None);
0x0051 : ldc.i4.0	                                                            -12
0x0052 : ldc.i4.s	44                                                          -11
0x0053 : callvirt	Void SetText(TextType, System.String, Boolean, Button)      -10
0x0054 : ret	                                                                -9
0x0055 : ldsfld	HandReticle main                                                -8
0x0056 : ldc.i4.0	                                                            -7
0x0057 : ldloc.2	                                                            -6
0x0058 : ldc.i4.0	                                                            -5
0x0059 : ldc.i4.4	                                                            -4
0x005A : callvirt	Void SetText(TextType, System.String, Boolean, Button)      -3
0x005B : ldsfld	HandReticle main                                                -2
0x005C : ldc.i4.1	                                                            -1
0x005D : ldloc.3	<---------------------------------------------------------- 0 HandReticle.main.SetText(HandReticle.TextType.HandSubscript, text2, false, GameInput.Button.None);
0x005E : ldc.i4.0	                                                            1
0x005F : ldc.i4.s	44                                                          2
0x0060 : callvirt	Void SetText(TextType, System.String, Boolean, Button)      3
0x0061 : ret	                                                                4
0x0062 : ldarg.0	                                                            5
0x0063 : ldfld	System.Boolean isPickupable                                     6
0x0064 : brfalse	System.Reflection.Emit.Label                                7
0x0065 : ldsfld	Player main                                                     8
0x0066 : ldarg.0	                                                            9
0x0067 : callvirt	Boolean HasInventoryRoom(Pickupable)                        10
0x0068 : brtrue	System.Reflection.Emit.Label                                    11
0x0069 : ldloc.0	                                                            12
0x006A : ldc.i4.0	                                                            13
0x006B : ldloc.1	                                                            14
0x006C : ldc.i4.0	                                                            15
0x006D : call	System.String AsString(TechType, Boolean)                       16
0x006E : ldc.i4.1	                                                            17
0x006F : ldc.i4.s	44                                                          18
0x0070 : callvirt	Void SetText(TextType, System.String, Boolean, Button)      19
0x0071 : ldloc.0	                                                            20
0x0072 : ldc.i4.1	                                                            21
0x0073 : ldstr	InventoryFull                                                   22 main.SetText(HandReticle.TextType.HandSubscript, "InventoryFull", true, GameInput.Button.None);
0x0074 : ldc.i4.1	                                                            23
0x0075 : ldc.i4.s	44                                                          24
0x0076 : callvirt	Void SetText(TextType, System.String, Boolean, Button)      25
0x0077 : ret	                                                                26
0x0078 : ldloc.0	                                                            27
0x0079 : ldc.i4.0	                                                            28
0x007A : ldloc.1	                                                            29
0x007B : ldc.i4.0	                                                            30
0x007C : call	System.String AsString(TechType, Boolean)                       31
0x007D : ldc.i4.1	                                                            32
0x007E : ldc.i4.s	44                                                          33
0x007F : callvirt	Void SetText(TextType, System.String, Boolean, Button)      34
0x0080 : ldloc.0	                                                            35
0x0081 : ldc.i4.1	                                                            36
0x0082 : ldsfld	System.String Empty                                             37 main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
0x0083 : ldc.i4.0	                                                            38
0x0084 : ldc.i4.s	44                                                          39
0x0085 : callvirt	Void SetText(TextType, System.String, Boolean, Button)      40
0x0086 : ret	                                                                41                   
                 */

                if (codes[i].opcode == OpCodes.Ldloc_3 && codes[i + 4].opcode == OpCodes.Ret && codes[i + 5].opcode == OpCodes.Ldarg_0)  //adjust
                {
                    if (debuglogging)
                    {
                        Plugin.Logger.LogDebug( "Found IL Code Line for Index");
                        Plugin.Logger.LogDebug( $"Index = {Index.ToString()}");
                    }
                    found = true;
                    Index = i;
                    break;
                }
            }

            if (debuglogging)
            {
                if (found)
                {
                    Plugin.Logger.LogDebug( "found true");
                }
                else
                {
                    Plugin.Logger.LogDebug( "found false");
                }
            }

            if (Index > -1)
            {
                if (debuglogging)
                {
                    Plugin.Logger.LogDebug("Index1 > -1");
                }
                Plugin.Logger.LogInfo($"Transpiler injectection position found - {classnamefordocu}");


                codes.Insert(Index - 12, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pickupable_OnHandHover_Patch), nameof(Pickupable_OnHandHover_Patch.GetFreeExoSuitStorage))));
                codes.Insert(Index + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pickupable_OnHandHover_Patch), nameof(Pickupable_OnHandHover_Patch.GetFreeSpacewithOriginal))));
                codes.Insert(Index + 40, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pickupable_OnHandHover_Patch), nameof(Pickupable_OnHandHover_Patch.GetFreeSpace))));
                codes.RemoveRange(Index + 39, 1);
            }
            else
            {
                Plugin.Logger.LogError("Index was not found");
            }

            //logging after
            if (debuglogging && deeplogging)
            {
                Plugin.Logger.LogDebug( "Deep Logging after-transpiler:");
                for (int k = 0; k < codes.Count; k++)
                {
                    Plugin.Logger.LogDebug( (string.Format("0x{0:X4}", k) + $" : {codes[k].opcode.ToString()}	{(codes[k].operand != null ? codes[k].operand.ToString() : "")}"));
                }
            }

            if (debuglogging)
            {
                Plugin.Logger.LogDebug( "Transpiler end going to return");
            }
            return codes.AsEnumerable();
        }

        public static string GetFreeSpacewithOriginal(string original)
        {
            string returnstring = GetFreeSpace();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(original);
            stringBuilder.AppendLine(returnstring);
            return stringBuilder.ToString();
        }

        public static string GetFreeSpace()
        {
            string returnstring = "";
            if (!(Player.main.HasInventoryRoom(1, 1)))
            {
                HandReticle.main.SetIcon(HandReticle.IconType.HandDeny, 1f);
                returnstring = "Inventory Full !";
            }
            else
            {
                Inventory Inv = Player.main.GetComponent<Inventory>();
                int Size_x = Inv.container.sizeX;
                int Size_y = Inv.container.sizeY;
                int Size_xy = Size_y * Size_x;
                var Items = Inv.container.GetItemTypes();
                int usedSize = 0;
                foreach (var i in Items)
                {
                    var size = CraftData.GetItemSize(i);
                    int numberofsingletechtype = (Inv.container.GetItems(i)).Count;
                    if (debuglogging)
                    {
                        Plugin.Logger.LogDebug( $"Techtype = {i.ToString()}");
                        Plugin.Logger.LogDebug( $"Number of items in this Techtype = {numberofsingletechtype.ToString()}");
                    }
                    usedSize += size.x * size.y * numberofsingletechtype;
                    if (debuglogging)
                    {
                        Plugin.Logger.LogDebug( $"Used Space of this Techtype = {(size.x * size.y * numberofsingletechtype).ToString()}");
                    }
                }
                var sizeLeft = Size_xy - usedSize;
                returnstring = sizeLeft.ToString() + " of " + Size_xy + " free";

            }
            return returnstring;
        }

        public static string GetFreeExoSuitStorage(string original)
        {
            string returnstring = "";

            if (Player.main.GetVehicle() is Exosuit exosuit)
            {
                if (exosuit.storageContainer.container.HasRoomFor(1, 1))
                {
                    int Size_x = exosuit.storageContainer.container.sizeX;
                    int Size_y = exosuit.storageContainer.container.sizeY;
                    int Size_xy = Size_y * Size_x;
                    var Items = exosuit.storageContainer.container.GetItemTypes();
                    int usedSize = 0;
                    foreach (var i in Items)
                    {
                        var size = CraftData.GetItemSize(i);
                        int numberofsingletechtype = (exosuit.storageContainer.container.GetItems(i)).Count;
                        if (debuglogging)
                        {
                            Plugin.Logger.LogDebug( $"Techtype = {i.ToString()}");
                            Plugin.Logger.LogDebug( $"Number of items in this Techtype = {numberofsingletechtype.ToString()}");
                        }
                        usedSize += size.x * size.y * numberofsingletechtype;
                        if (debuglogging)
                        {
                            Plugin.Logger.LogDebug( $"Used Space of this Techtype = {(size.x * size.y * numberofsingletechtype).ToString()}");
                        }
                    }
                    var sizeLeft = Size_xy - usedSize;
                    returnstring = sizeLeft.ToString() + " of " + Size_xy + " free";
                }
                else
                {
                    HandReticle.main.SetIcon(HandReticle.IconType.HandDeny, 1f);
                    returnstring = "Inventory Full !";
                }
            }
            else
            {
                if (debuglogging)
                {
                    Plugin.Logger.LogDebug( $"Called but not in a Exosuit ?");
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(original);
            stringBuilder.AppendLine(returnstring);
            return stringBuilder.ToString();
        }
    }
}