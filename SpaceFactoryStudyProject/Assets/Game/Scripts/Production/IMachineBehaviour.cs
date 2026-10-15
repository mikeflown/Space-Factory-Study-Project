public interface IMachineBehaviour
{
    void Bind(MachineInstance owner);
    void Tick(float deltaTime, ProductionContext context);
}