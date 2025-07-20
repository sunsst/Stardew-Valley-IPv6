# Stardew Valley IPv6

[简体中文](https://github.com/sunsst/Stardew-Valley-IPv6/blob/IPv6/README.md)

[English](https://github.com/sunsst/Stardew-Valley-IPv6/blob/IPv6/README_EN.md)

Enable Stardew Valley to use IPv6 addresses for multiplayer.

Referenced projects:
- [Lidgren.Network][lnet]
- [SpaceWizards.Lidgren.Network][slnet]
- [HarmonyLib][har]
- [SMAPI][smapi]

## Installation
If you want to enable IPv6 support for the vanilla version, choose the *dll* patch. If you have [SMAPI][smapi] installed, either version will work (though installing both is not recommended).

### MOD Version
After unzipping the *IPv6.zip* archive, drag the extracted folder into the `Stardew Valley/Mods` directory.

### DLL Version
Copy and replace the modified *Lidgren.Network.dll* file into `Stardew Valley/Lidgren.Network.dll`.

**It is recommended to back up the original *Lidgren.Network.dll* file.** If unexpected errors occur, you can restore the original file.

## Features
Three server IP modes are available:
1. Server only supports IPv4 client connections
2. Server only supports IPv6 client connections
3. Server supports both IPv4 and IPv6 client connections

When the host enters a save file, a banner will appear in the bottom-left corner indicating the current server IP mode.

By default, the server selects the IP mode based on the device's supported protocols. If both protocols are supported, it enables dual-stack mode.

The host can specify the IP mode by holding down the *4* or *6* keys (or both simultaneously) during save loading.

Clients automatically select the appropriate IP mode based on the entered server address, requiring no manual adjustment.

Clients can enter either IP addresses or domain names, as the vanilla game supports domain resolution.

## Implementation
Both patches implement the same process differently:
1. The original [Lidgren.Network][lnet] is outdated and lacks IPv6 support, requiring replacement.
2. Enable IPv6 functionality in the initialization options of `Class StardewValley.Network.LidgrenClient` and `StardewValley.Network.LidgrenServer`.
3. Fix `Method StardewValley.Network.LidgrenClient.attemptConnection()` to properly resolve IPv6 addresses.

The replacement [Lidgren.Network][lnet] version is a fork: [SpaceWizards.Lidgren.Network][slnet]. This fork supports newer .NET frameworks but contains a bug preventing direct use.

### Impact
This patch primarily affects mods that modify the game's multiplayer networking layer, with minimal impact on other mods.

#### MOD Version
Since the MOD version cannot replace the original [Lidgren.Network][lnet], the following classes/methods are rewritten:
- `Class StardewValley.Network.LidgrenClient`
- `Class StardewValley.Network.LidgrenServer`
- `Class StardewValley.Network.NetBufferReadStream`
- `Class StardewValley.Network.NetBufferWriteStream`
- `Class StardewValley.Network.LidgrenMessageUtils`
- `Method StardewValley.Network.GameServer.UpdateLocalOnlyFlag()`

Uses [HarmonyLib][har] to modify references to original types in:
- `Method StardewValley.Game1.UpdateTitleScreen()`
- `Method StardewValley.Menus.CoopMenu.enterIPPressed()`
- `Method StardewValley.Multiplayer.LogDisconnect()`
- `Constructor StardewValley.Network.GameServer()`

#### DLL Version
The DLL version directly replaces the original [Lidgren.Network][lnet]. However, since the vanilla address resolution cannot handle IPv6, [HarmonyLib][har] is used to modify `Method StardewValley.Network.LidgrenClient.attemptConnection()`.

To ensure vanilla compatibility, a full [HarmonyLib][har] is bundled, increasing the file size by ***over 2MB***.

Configuration changes are implemented by overriding `Method Lidgren.Network.NetPeer.Start()` in `Class Lidgren.Network.NetServer` and `Class Lidgren.Network.NetClient`.

#### Android Compatibility
As noted in [Mod: Installing SMAPI on Android][wiki]:
The Android version of Stardew Valley has significant architectural differences. While an unofficial SMAPI port exists (compatible with most frameworks like Content Patcher and Json Assets), some mods may not work.

With assistance from [@nkanf-dev][nkanf], Android compatibility was achieved. Differences in the Android implementation:
- Modifications to `Method StardewValley.Menus.CoopMenu.enterIPPressed()` are replaced by changes to `Method StardewValley.Menus.CoopGameMenu.enterIPPressed()`.
- Modify `Field StardewValley.Menus.TitleTextInputMenu.textBox.textLimit` to allow longer input.

[lnet]: https://github.com/lidgren/lidgren-network-gen3
[slnet]: https://github.com/space-wizards/SpaceWizards.Lidgren.Network
[har]: https://github.com/pardeike/Harmony
[smapi]: https://github.com/Pathoschild/SMAPI
[wiki]: https://zh.stardewvalleywiki.com/%E6%A8%A1%E7%BB%84:%E5%9C%A8Android%E4%B8%8A%E5%AE%89%E8%A3%85SMAPI
[nkanf]: https://github.com/nkanf-dev