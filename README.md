# 语言选择 / Language
- [简体中文](#简体中文)
- [English](#English)

---

## 简体中文
# BangbooEncryptor

把任何文本加密成《绝区零》中邦布特有的语言。<br>
本项目采用 MIT 许可协议，详见 LICENSE 文件。


## 这是什么？

这是一个超级无厘头的文本隐写（steganography）小工具。<br>
它会把你的原文：

1. 用 Deflate 压缩<br>
2. 用密码做 XOR 加密<br>
3. 转成二进制<br>
4. 每 2 bit 映射成「嗯呢」「嗯呐」「哇哒」「嗯呐哒」<br>
5. 再加上标点和分句，让它看起来像邦布在聊天

最后输出一串类似邦布语的“密文”。

## 用法

```bash
# 基本带密码加密
BangbooEncryptor encrypt "消灾解厄" -p mypassword

# 从管道输入
echo "六豆链接大脑，平A代替思考" | BangbooEncryptor encrypt -p pass123

# 指定种子（可复现）
echo "你怎么知道我今天十连双黄" | BangbooEncryptor encrypt -p key -s 114514

# 解密
BangbooEncryptor decrypt "嗯呐哒！哇哒，嗯呢嗯呢，哇哒嗯呐哒！哇哒！嗯呢，嗯呢嗯呢，嗯呐哒！嗯呐！" -p "secret"

# 交互模式
BangbooEncryptor interactive
```

更多选项请运行：
```bash
BangbooEncryptor --help
```

## 构建

需要 .NET 8 或更高版本。

```bash
# Clone the repo
git clone https://github.com/Marksonthegamer/BangbooEncryptor.git
cd BangbooEncryptor

# Run directly
dotnet run encrypt "test" -p secret

# Publish a single executable file (recommended)
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

## 谁适合用？

- 绝区零玩家（尤其是喜欢邦布的）
- 想在群里/评论区藏东西又不想被一眼看穿的人
- 喜欢整活、发癫、做社区暗号的快乐犯病患者
- 需要低带宽、强语境伪装的短消息传输场景

**注意**：这不是严肃的加密工具！安全性仅供娱乐，千万不要用来传真正敏感信息哦～

## 致谢

灵感来源：
- 《绝区零》里的可爱邦布们
- 经典的语言隐写（linguistic steganography）玩法
- 米游社 / B站玩家们的梗文化

祝你加密愉快，邦布语流利～

## English
# BangbooEncryptor

Encrypt any text into adorable Bangboo speech from Zenless Zone Zero.<br>
Licensed under the MIT License. See the LICENSE file for details.

##  What is this?

This is a silly yet functional text steganography tool that disguises your messages as Bangboo language from Zenless Zone Zero.

Features:
- Compression (Deflate) + password-protected XOR encryption
- 2-bit → phrase mapping (嗯呢 / 嗯呐 / 哇哒 / 嗯呐哒)
- Cute punctuation & sentence breaking for natural-looking output
- Reproducible output with seed option
- Supports stdin / file input / interactive mode

## Usage

```bash
# Basic password encryption
BangbooEncryptor encrypt "DAMIDAMI" -p mypassword

# Input from pipe
echo "PEAK" | BangbooEncryptor encrypt -p pass123

# Specify a seed (does not affect decryption)
echo "My services are expensive." | BangbooEncryptor encrypt -p key -s 114514

#  Decryption
BangbooEncryptor decrypt "嗯呐哒！哇哒，嗯呢嗯呢，哇哒嗯呐哒！哇哒！嗯呢，嗯呢嗯呢，嗯呐哒！嗯呐！" -p "secret"

# Interactive mode
BangbooEncryptor interactive
```

For more options run：
```bash
BangbooEncryptor --help
```

## Build

Require .NET 8 or later.

```bash
# Clone the repo
git clone https://github.com/Marksonthegamer/BangbooEncryptor.git
cd BangbooEncryptor

# Run directly
dotnet run encrypt "test" -p secret

# Publish a single executable file (recommended)
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

##  Who is this for?

- Players of Zenless Zone Zero (especially those who like Bangboo)
- People who want to hide content in group chats or comment sections without it being immediately obvious
- Enthusiastic meme lovers who enjoy creating inside jokes and community codes
- Scenarios requiring low-bandwidth, context-heavy covert communication

**Warning**: This is literally vibe-coding stuff and **not** a serious cryptographic tool. It's for fun, memes, and light community play. Do **not** use it for sensitive data!

Inspired by cute bangboos in ZZZ and linguistic steganography.

Enjoy your Bangboo encryption adventure!
