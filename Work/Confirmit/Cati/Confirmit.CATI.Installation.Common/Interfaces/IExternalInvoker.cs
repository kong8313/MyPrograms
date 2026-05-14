namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface IExternalInvoker
    {
        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        string Invoke(string scriptNameOrPath, string args);

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="delay">Delay for waiting</param>
        string Invoke(string scriptNameOrPath, string args, int delay);

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="isNeedToWait">Is need to wait until execution finish </param>
        string Invoke(string scriptNameOrPath, string args, bool isNeedToWait);

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="tempFolderPath">Temp folder path</param>
        string Invoke(string scriptNameOrPath, string args, string tempFolderPath);

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="tempFolderPath">Temp folder path</param>
        /// <param name="delay">Delay for waiting</param>
        /// <param name="isNeedToWait">Is need to wait until execution finish</param>
        /// <param name="doNotVerifyExitCode">Do not verify exit code</param>
        string Invoke(string scriptNameOrPath, string args, string tempFolderPath, int delay, bool isNeedToWait, bool doNotVerifyExitCode);
    }
}