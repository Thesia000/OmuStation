



namespace Content.Omu.Server._BSD.SignalSalv;


public sealed partial class SignalSalvSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        //SubscribeLocalEvent<SignalSciDishComponent, MultiStructChangeEvent>(UpdateValues);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

    }

}