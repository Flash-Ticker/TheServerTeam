using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Oxide.Plugins
{
    [Info("TheServerTeam", "RustFlash", "1.0.0")]
    public class TheServerTeam : RustPlugin
    {
        private ConfigData configData;

        private class ConfigData
        {
            public Dictionary<string, List<TeamMember>> TeamRoles = new Dictionary<string, List<TeamMember>>();
            public UIColors Colors = new UIColors();
        }

        private class TeamMember
        {
            public ulong SteamID;
            public string Name;
        }

        private class UIColors
        {
            public string Background = "#0C0E0FFC";         // Dunkelgrau
            public string CardBackground = "#293133";     // Helleres Grau
            public string HeaderText = "#f3f4f6";         // Fast-Weiß
            public string PrimaryText = "#ffed00";        // Hellgrau
            public string SecondaryText = "#9ca3af";      // Mittleres Grau
            public string Online = "#34d399";             // Grün
            public string Offline = "#ef4444";            // Rot
            public string CloseButton = "#ef4444";        // Dunkelgrau
            public string CloseButtonHover = "#ff0000";   // Helleres Grau
        }

        // Konvertiert HTML Hex zu Unity Color String
        private string HexToUnityColor(string hex)
        {
            try
            {
                hex = hex.Replace("#", "");
                if (hex.Length == 6) hex += "FF"; // Füge Alpha hinzu wenn nicht vorhanden
                
                var r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
                var g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
                var b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
                var a = Convert.ToInt32(hex.Substring(6, 2), 16) / 255f;

                return $"{r} {g} {b} {a}";
            }
            catch
            {
                PrintError($"Ungültiger Farbcode: {hex}");
                return "1 1 1 1"; // Fallback zu Weiß
            }
        }

        protected override void LoadDefaultConfig()
        {
            configData = new ConfigData
            {
                TeamRoles = new Dictionary<string, List<TeamMember>>
                {
                    ["Owner"] = new List<TeamMember>
                    {
                        new TeamMember { SteamID = 76561198253131280, Name = "[RF]Oli" }
                    },
                    ["Admin"] = new List<TeamMember>
                    {
                        new TeamMember { SteamID = 76561198000000001, Name = "Peter" }
                    },
                    ["Moderatoren"] = new List<TeamMember>
                    {
                        new TeamMember { SteamID = 76561198000000002, Name = "Lisa" }
                    }
                },
                Colors = new UIColors()
            };
            SaveConfig();
        }

        private void Init()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (Config.Exists())
            {
                try
                {
                    configData = Config.ReadObject<ConfigData>();
                    if (configData.Colors == null) configData.Colors = new UIColors();
                }
                catch (Exception ex)
                {
                    PrintError($"Fehler beim Laden der Konfigurationsdatei: {ex.Message}. Erstelle neue Konfiguration.");
                    LoadDefaultConfig();
                }
            }
            else
            {
                // Keine Konfiguration gefunden, also Standardwerte laden und speichern
                PrintError("Konfigurationsdatei nicht gefunden. Erstelle neue Konfiguration.");
                LoadDefaultConfig();
            }
        }

        private void SaveConfig()
        {
            Config.WriteObject(configData, true);
        }

        private void ShowGUI(BasePlayer player, List<string> teamMembers)
        {
            var elements = new CuiElementContainer();

            var panel = elements.Add(new CuiPanel
            {
                Image = { Color = HexToUnityColor(configData.Colors.Background) },
                RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1" },
                CursorEnabled = true
            }, "Overlay", "ServerTeamPanel");

            elements.Add(new CuiPanel
            {
                Image = { Color = HexToUnityColor(configData.Colors.CardBackground) },
                RectTransform = { AnchorMin = "0 0.9", AnchorMax = "1 1" }
            }, panel);

            elements.Add(new CuiLabel
            {
                Text = { Text = "SERVER TEAM", FontSize = 24, Align = TextAnchor.MiddleCenter, Color = HexToUnityColor(configData.Colors.HeaderText) },
                RectTransform = { AnchorMin = "0 0.9", AnchorMax = "1 1" }
            }, panel);

            elements.Add(new CuiButton
            {
                Button = { Color = HexToUnityColor(configData.Colors.CloseButton), Command = "tst.close" },
                RectTransform = { AnchorMin = "0.93 0.93", AnchorMax = "0.97 0.97" },
                Text = { Text = "×", FontSize = 20, Align = TextAnchor.MiddleCenter, Color = HexToUnityColor(configData.Colors.HeaderText) }
            }, panel);

            AddModernTeamMembers(elements, panel, teamMembers);

            CuiHelper.AddUi(player, elements);
        }

        private void AddModernTeamMembers(CuiElementContainer elements, string parent, List<string> teamMembers)
        {
            int columns = 3;
            float cardSpacing = 0.02f;
            float contentStartY = 0.85f;

            // Gesamtbreite der Karten berechnen
            float cardWidth = (0.5f - (cardSpacing * (columns + 1))) / columns;
            float totalWidth = cardWidth * columns + cardSpacing * (columns + 1);

            // Wenn weniger Karten als die Anzahl der Spalten vorhanden sind, müssen wir den Offset anpassen
            float offsetX = (1f - totalWidth) / 2f;

            // Mitglieder nach Kategorien (Rollen) gruppieren
            var groupedMembers = teamMembers.GroupBy(m => m.Split('\n')[0]).ToList();

            float rowStartY = contentStartY; // Start Y-Position für die Zeilen

            // Durch die Kategorien gehen
            foreach (var group in groupedMembers)
            {
                int row = 0;
                float rowWidth = cardWidth * group.Count() + cardSpacing * (group.Count() + 1);
                float rowOffsetX = (1f - rowWidth) / 2f; // Berechnung des Versatzes für die Zeile

                // Mitglieder innerhalb der Kategorie in Karten umwandeln
                foreach (var memberInfo in group)
                {
                    string[] memberData = memberInfo.Split('\n');
                    string role = memberData[0];
                    string name = memberData[1];
                    string status = memberData[2];

                    // Berechne die Position der Karte basierend auf der Spalte
                    float cardX = rowOffsetX + (row * (cardWidth + cardSpacing));
                    float cardY = rowStartY;

                    var cardPanel = elements.Add(new CuiPanel
                    {
                        Image = { Color = HexToUnityColor(configData.Colors.CardBackground) },
                        RectTransform = { 
                            AnchorMin = $"{cardX} {cardY - 0.2f}", 
                            AnchorMax = $"{cardX + cardWidth} {cardY}"
                        }
                    }, parent);

                    // Name in PrimaryText-Farbe
                    elements.Add(new CuiLabel
                    {
                        Text = { 
                            Text = name, 
                            FontSize = 16, 
                            Align = TextAnchor.MiddleCenter,
                            Color = HexToUnityColor(configData.Colors.PrimaryText)
                        },
                        RectTransform = { AnchorMin = "0 0.6", AnchorMax = "1 0.9" }
                    }, cardPanel);

                    // Kategorie (Role) in SecondaryText-Farbe
                    elements.Add(new CuiLabel
                    {
                        Text = { 
                            Text = role.ToUpper(), 
                            FontSize = 14, 
                            Align = TextAnchor.MiddleCenter,
                            Color = HexToUnityColor(configData.Colors.SecondaryText)
                        },
                        RectTransform = { AnchorMin = "0 0.4", AnchorMax = "1 0.8" }
                    }, cardPanel);

                    // Status (Online/Offline)
                    string statusColor = status == "Online" ? 
                        HexToUnityColor(configData.Colors.Online) : 
                        HexToUnityColor(configData.Colors.Offline);

                    elements.Add(new CuiPanel
                    {
                        Image = { Color = statusColor },
                        RectTransform = { 
                            AnchorMin = "0.43 0.2", 
                            AnchorMax = "0.47 0.25" 
                        }
                    }, cardPanel);

                    elements.Add(new CuiLabel
                    {
                        Text = { 
                            Text = status, 
                            FontSize = 12, 
                            Align = TextAnchor.MiddleLeft,
                            Color = statusColor
                        },
                        RectTransform = { AnchorMin = "0.48 0.15", AnchorMax = "0.8 0.3" }
                    }, cardPanel);

                    row++;
                }

                // Verschiebung der Y-Position für die nächste Reihe
                rowStartY -= 0.25f; // Höhe der Karten + Abstand
            }
        }

        private void DestroyGUI(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "ServerTeamPanel");
        }

        [ChatCommand("admin")]
        private void CommandTheServerTeam(BasePlayer player, string command, string[] args)
        {
            if (configData?.TeamRoles == null)
            {
                PrintError("Konfiguration ist nicht geladen. Lade Konfiguration neu...");
                LoadConfig();
                if (configData?.TeamRoles == null)
                {
                    PrintError("Konnte Konfiguration nicht laden.");
                    return;
                }
            }

            List<string> teamMembers = new List<string>();

            foreach (var role in configData.TeamRoles)
            {
                foreach (var member in role.Value)
                {
                    var onlinePlayer = BasePlayer.activePlayerList.FirstOrDefault(p => p.userID == member.SteamID);
                    string status = onlinePlayer != null ? "Online" : "Offline";
                    string memberInfo = $"{role.Key}\n{member.Name}\n{status}";
                    teamMembers.Add(memberInfo);
                }
            }

            ShowGUI(player, teamMembers);
            timer.Once(60f, () => DestroyGUI(player));
        }

        [ConsoleCommand("tst.close")]
        private void CloseUICommand(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (player != null)
            {
                DestroyGUI(player);
            }
        }
    }
}