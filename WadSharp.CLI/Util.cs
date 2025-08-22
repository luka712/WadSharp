namespace WadSharp.CLI
{
    /// <summary>
    /// Utility class for working with comsole.
    /// </summary>
    internal class Util
    {
        /// <summary>
        /// Checks if an argument exists.
        /// </summary>
        /// <example>
        /// bool hasFlag = HasArgument(args, "--help", "-h");
        /// </example>
        /// <param name="arguments">The console arguments.</param>
        /// <param name="argumentNames">
        /// The name of arguments. Gets value of first existing one.
        /// </param>
        /// <returns>
        /// <c>true</c> if any of the arguments exist; otherwise, <c>false</c>.
        /// </returns>
        internal static bool HasArgument(string[] arguments, params string[] argumentNames)
            => arguments.Any(arg => argumentNames.Contains(arg));

        /// <summary>
        /// Gets the value of an argument.
        /// </summary>
        /// <example>
        /// string? value = GetArgumentValue(args, "--i", "--input");
        /// </example>
        /// <param name="arguments">The console arguments.</param>
        /// <param name="argumentNames">
        /// The name of arguments. Gets value of first existing one.
        /// </param>
        /// <returns>
        /// The exiting value or <c>null</c>.
        /// </returns>
        internal static string? GetArgumentValue(string[] arguments, params string[] argumentNames)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                string arg = arguments[i];
                foreach (string argName in argumentNames)
                {
                    if (arg == argName && i + 1 < arguments.Length)
                    {
                        return arguments[i + 1];
                    }
                }
            }

            return null;
        }
    }
}
