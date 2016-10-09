namespace Conreign.Core.Communication
{
    internal enum CommunicationError
    {
        Base = 50000,
        BusStreamUnexpectedlyCompleted = Base + 1,
        BusStreamUnexpectedException = Base + 2
    }
}
