using Content.Omu.Server._BSD.IngameServerClientLinkSystem.Components;

namespace Content.Omu.Server._BSD.IngameServerClientLinkSystem;

public sealed partial class BSDIngameServerClientLinkSystem : EntitySystem
{
    public bool TryEstablishLink(EntityUid startOfConnection, EntityUid targetOfConnection, string channel, bool connectAsClient = true)
    {
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(startOfConnection, out var compStart)) return false;
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(targetOfConnection, out var compTarget)) return false;
        if (connectAsClient && compTarget.ServerNeedsToIniciate[channel]) return false;
        if (!CheckTransmissionRange(startOfConnection, targetOfConnection, channel)) return false;
        compStart.EntityDicServer[channel].Add(targetOfConnection);
        compTarget.EntityDicClient[channel].Add(startOfConnection);
        return true;
    }
    public void TerminateLink(EntityUid startOfConnection, EntityUid targetOfConnection, string channel)
    {
        bool startPresent = true;
        bool targetPresent = true;
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(startOfConnection, out var compStart)) startPresent = false;
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(targetOfConnection, out var compTarget)) targetPresent = false;
        if (startPresent)
        {
            compStart!.EntityDicClient[channel].Remove(targetOfConnection);
            compStart!.EntityDicServer[channel].Remove(targetOfConnection);
        }
        if (targetPresent)
        {
            compTarget!.EntityDicClient[channel].Remove(startOfConnection);
            compTarget!.EntityDicServer[channel].Remove(startOfConnection);
        }
        return;
    }
}