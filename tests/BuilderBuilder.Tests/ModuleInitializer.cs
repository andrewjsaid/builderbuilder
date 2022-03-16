using System.Runtime.CompilerServices;
using VerifyTests;

namespace BuilderBuilder.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
