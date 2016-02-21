namespace Conreign.Api.Framework.ErrorHandling
{
    public class ErrorFactorySettings
    {
        public ErrorFactorySettings()
        {
            SerializeStackTrace = true;
            SerializeSystemErrorMessage = true;
        }

        public bool SerializeStackTrace { get; set; }

        public bool SerializeSystemErrorMessage { get; set; }
    }
}