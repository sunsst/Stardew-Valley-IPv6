using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace IPv6.Patch;

internal static partial class MyPatch
{
    public static HarmonyMethod UpdateLocalOnlyFlagTranspiler = new HarmonyMethod(typeof(MyPatch), "UpdateLocalOnlyFlagTranspilerMethod");

    public static IEnumerable<CodeInstruction> UpdateLocalOnlyFlagTranspilerMethod(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
    {
        LogInfo($"\n========== UpdateLocalOnlyFlag | {__originalMethod.DeclaringType}:{__originalMethod.Name}() ==========");

        foreach (CodeInstruction code in new CodeInstruction[] {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Call, AccessTools.Method(typeof(Methods.GameServer), "UpdateLocalOnlyFlag")),
            new(OpCodes.Ret)
        })
        {
            LogInfo($"! emit | {code}");
            yield return code;
        }
    }
}
