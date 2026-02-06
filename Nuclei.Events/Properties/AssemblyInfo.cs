using System.Runtime.CompilerServices;

// Allow the patches assembly to invoke internal event raisers.
[assembly: InternalsVisibleTo("MaxWasUnavailable.Nuclei.Patches")]
// Allow the core assembly to invoke internal event raisers.
[assembly: InternalsVisibleTo("MaxWasUnavailable.Nuclei.Core")]
