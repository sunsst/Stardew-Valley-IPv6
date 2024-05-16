# Stardew Valley IPv6
让星露谷物语也能使用 IPv6 地址进行联机。

引用的项目：
- [Lidgren.Network][lnet]
- [SpaceWizards.Lidgren.Network][slnet]
- [HarmonyLib][har]
- [SMAPI][smapi]

## 安装方法
如果你希望让原版也能支持 IPv6 可以选择 *dll* 版本的补丁。如果你有安装 [SMAPI][smapi] 选择哪个版本都无所谓，两个补丁全安装上也没啥问题（但我不建议这么做）。

### mode版本：
解压 *IPv6.zip* 压缩包后，将解压的文件夹拖入 `/Stardew Valley/mode/` 文件夹中。

### dll版本
将修改过的 *Lidgren.Network.dll* 文件复制粘贴并**替换** `/Stardew Valley/Lidgren.Network.dll` 文件。

**建议备份原有的 *Lidgren.Network.dll* 文件**，当发生不可预知的错误时可以替换回原有文件，恢复运行。


## 实现过程
两种补丁都是同一过程的不同实现：
1. 原有的 [Lidgren.Network][lnet] 是旧版本，不支持 IPv6，需要更换为其他版本。
2. 让 `Class StardewValley.Network.LidgrenClient` 与 `StardewValley.Network.LidgrenServer` 中的初始化选项，启用 IPv6 功能。
3. 修复 `Method StardewValley.Network.LidgrenClient.attemptConnection()` 让该方法可以正常解析 IPv6 地址。

更换的 [Lidgren.Network][lnet] 版本是其的一个分支 —— [SpaceWizards.Lidgren.Network][slnet]。

### 影响
大概或许可能应该，该补丁只会影响对游戏联机底层进行修改的模组，而其他模组可以说毫无影响。

#### mode版本
补丁的 mode 版本由于不能替换原有的 [Lidgren.Network][lnet]，我选择重写与之关联的类与方法：
- `Class StardewValley.Network.LidgrenClient`
- `Class StardewValley.Network.LidgrenServer`
- `Class StardewValley.Network.NetBufferReadStream`
- `Class StardewValley.Network.NetBufferWriteStream`
- `Class StardewValley.Network.LidgrenMessageUtils`
- `Method StardewValley.Network.GameServer.UpdateLocalOnlyFlag()`

使用 [HarmonyLib][har] 修改对原类型的引用的方法：
- `Method StardewValley.Game1.UpdateTitleScreen()`
- `Method StardewValley.Menus.CoopMenu.enterIPPressed()`
- `Method StardewValley.Multiplayer.LogDisconnect()`
- `Constructor StardewValley.Network.GameServer()`

#### dll版本
补丁的 dll 版本可以直接替换原有的 [Lidgren.Network][lnet]，但由于原有的客户端地址解析过程无法解析 IPv6 地址，所以不得不使用 harmonylib 修改方法 `Method StardewValley.Network.LidgrenClient.attemptConnection()`

并且为了让原版也能正常运行，又不得不在其中打包了完整的 [HarmonyLib][har] 使体积上涨了 ***2MB*** 有余。

配置选项的更改是通过 `Class Lidgren.Network.NetServer` 与 `Class Lidgren.Network.NetClient` 覆写 `Method Lidgren.Network.NetPeer.Start()` 方法实现的。



[lnet]: https://github.com/lidgren/lidgren-network-gen3
[slnet]: https://github.com/space-wizards/SpaceWizards.Lidgren.Network
[har]: https://github.com/pardeike/Harmony
[smapi]: https://github.com/Pathoschild/SMAPI

