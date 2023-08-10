using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace FollowersAreSneaky
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "FollowersAreSneaky.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            int count = 0;

            // For every NPC, check for the follower faction
            foreach(var NPC in state.LoadOrder.PriorityOrder.WinningOverrides<INpcGetter>())
            {
                foreach (var Faction in NPC.Factions)
                {
                    // If the NPC is not a potential follower, skip them
                    if (!Faction.Faction.TryResolve(state.LinkCache, out var fac) || fac.EditorID != "PotentialFollowerFaction")
                    {
                        continue;
                    }

                    state.PatchMod.Npcs.GetOrAddAsOverride(NPC).Configuration.Flags = NPC.Configuration.Flags.SetFlag(NpcConfiguration.Flag.DoesntAffectStealthMeter, true);
                    count++;
                }
            }

            System.Console.WriteLine("Done! Patched " + count.ToString() + " NPCs.");
        }
    }
}
