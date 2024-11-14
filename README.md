# TheServerTeam

**__Project details__**

**Project:** *TheServerTeam*

**dev language:** *c# oxide*

**Plugin language:** *en*

**Author:** [@RustFlash](https://github.com/Flash-Ticker)

[![RustFlash - Your Favourite Trio Server](https://github.com/Flash-Ticker/TheServerTeam/blob/main/TheServerTeam_Thumb.jpg)](https://youtu.be/xJzMHkWhYpw?si=Xg3FFy5DJ8DGYJIP)

## Description
**TheServerTeam** is a plugin that provides a user interface to display the server team directly in the game. With a modern, clear team overview, players can quickly see which team members are active (online) and which are offline. The team overview includes the roles (e.g. owner, admin, moderators) and clearly displays information about the members. The colors and layout options are easily customizable to adapt the design to the needs of the server.

## Features:
- **Modern user interface:** Shows the server team in a clean, appealing interface.
- **Automatic status display:** Shows the online status of each team member (green for online, red for offline).
- **Customizable colors:** Color scheme and background colors can be customized in the configuration file.
- **Role-based display:** Groups team members by roles such as owners, admins and moderators.
- **Close button:** A button to easily close the team display is integrated into the user interface.
- **Close button:** If you do not use the close button, the UI closes automatically after 30 seconds

## Commands:

`/admin` - Opens the server team overview and displays all team members and their online status.



## Permissions:
This plugin does not require any special authorizations to be used. However, it can be restricted in the server configuration by making the chat command `/admin` only accessible to certain groups.


## Config:

```
{
  "TeamRoles": {
    "Owner": [
      {
        "SteamID": 7656143438253131280,
        "Name": "Oli"
      }
    ],
    "Admin": [
      {
        "SteamID": 765611433423785119,
        "Name": "Ubby"
      }
    ],
    "Mod": [
      {
        "SteamID": 76561199123785119,
        "Name": "Slolaone"
      },
      {
        "SteamID": 76561199123723320,
        "Name": "Slolatwo"
      },
      {
        "SteamID": 7656119912378421,
        "Name": "Slolathree"
      }
  	],
  },
  "Colors": {
    "Background": "#0C0E0FFC",
    "CardBackground": "#293133",
    "HeaderText": "#f3f4f6",
    "PrimaryText": "#ffed00",
    "SecondaryText": "#9ca3af",
    "Online": "#34d399",
    "Offline": "#ef4444",
    "CloseButton": "#ef4444",
    "CloseButtonHover": "#ff0000"
  }
}

```

---

**load, run, enjoy** 💝
