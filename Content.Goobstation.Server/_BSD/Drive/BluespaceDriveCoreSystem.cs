using Content.Goobstation.Shared._BSD.Drive.Components;


namespace Content.Goobstation.Server._BSD.Drive;

public abstract class SharedBluespaceStationDriveCoreSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        //SubscribeLocalEvent<BluespaceStationDriveCoreComponent, ItemSlotInsertAttemptEvent>(SaveDriveId);
    }
    /*
    public void SaveDriveId(EntityUid uid, BluespaceStationDriveCoreComponent component, ref ItemSlotInsertAttemptEvent args)
    {
        component.DriveId = args.SlotEntity;
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
    }
    */
}