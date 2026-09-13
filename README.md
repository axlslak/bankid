# BankId

A minimal AOSharp plugin for the **full Anarchy Online game client**.

Type `/bankid`. At that moment it reads the live dynel list. If both
`Kbcentral` and `Rubi-Ka Banking Service Terminal` are present, it resolves
Apcmanager's character ID and sends one tell:

```text
bankid 1477725977
```

The number comes from the visible terminal's current `Identity.Instance`; the
number above is only an example. There is no distance check, UI, timer,
background scanning, automatic retry, or saved bank ID. The first matching
terminal in the live list is used. Missing Kbcentral or terminal produces only
a local chat message. Apcmanager handles authorization, including officer alts.

## Pull, compile, use

1. Open **BankId.sln** in Visual Studio 2022 with **.NET desktop development** installed.
2. Select **Release | x86**, then **Build > Build Solution**. NuGet restores the pinned public packages automatically.
3. Take **bin/Release/BankId.dll** and load it in AOSharp.

That is the compiled plugin. No source file or solution file goes into AOSharp.
Debug builds likewise produce `bin/Debug/BankId.dll`. Solution and project builds
use these same output folders.

For command-line builds with the .NET SDK, run from this directory:

```powershell
dotnet build BankId.sln -c Release
```

This restores and compiles in one command, producing `bin/Release/BankId.dll`.
Internet access is required for the first package restore.

Both build dependencies are exact-version public NuGet packages:

- [AOSharpSDK 1.0.105](https://www.nuget.org/packages/AOSharpSDK/1.0.105)
- [Microsoft.NETFramework.ReferenceAssemblies.net48 1.0.3](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net48/1.0.3)

`NuGet.Config` uses only nuget.org. There are no project references, local DLL
paths, private feeds, navigation packages, or dependencies on City Dwellers or
InfoHelper. The .NET Framework compilation references restore from NuGet; no
locally installed targeting pack is required.

## Load and use

Use an existing working AOSharp installation in the full game client, compatible
with AOSharpSDK 1.0.105. This DLL is a plugin, not an AOSharp loader installer.
The Windows runtime needs .NET Framework 4.8 or newer.

Copy **BankId.dll only** to your AOSharp plugin location and load it with your
usual AOSharp loader. The host supplies AOSharp.Core and AOSharp.Common; do not
overwrite the host's assemblies with SDK reference DLLs. No accompanying XML,
configuration file, or UI assets are needed. Restart the game client when
replacing an already loaded build.

Enter the room with Kbcentral and the bank terminal, then type `/bankid`.
Apcmanager's reply confirms whether it accepted the new ID. Nothing is sent just
because the plugin is loaded or you enter the room.

Source/API and dependency metadata reviewed; build and live testing are left to
the operator.
