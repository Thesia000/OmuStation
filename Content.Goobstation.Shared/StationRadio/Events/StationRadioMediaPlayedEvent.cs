using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Goobstation.Shared.StationRadio.Events;

[Serializable, NetSerializable]
public sealed class StationRadioMediaPlayedEvent : EntityEventArgs
{
    public SoundPathSpecifier MediaPlayed { get; }
    public float Volume { get; }//omu
    public StationRadioMediaPlayedEvent(SoundPathSpecifier Media, float volume)//omu
    {
        MediaPlayed = Media;
        Volume = volume; //omu
    }
}
