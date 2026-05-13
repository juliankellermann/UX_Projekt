# Serious Game: Touchless Warehouse Training

## Projektbeschreibung
Dieses Unity-Projekt ist ein "Serious Game", das als Prototyp für die Einarbeitung und das Training von Lagerarbeitern entwickelt wurde. Im Fokus steht die Lösung logistischer Probleme (wie z.B. heruntergefallene Pakete oder festgefahrene Roboter) durch eine intuitive, berührungslose Steuerung.

Als zentrales Interaktionsgerät kommt ein **Skywriter-Gesten-Sensor** zum Einsatz, der die Fernwartung und Steuerung in logistischen Umgebungen simuliert.

## Features & Interaktionskonzept
* **Berührungslose Steuerung:** Nutzung des Skywriter-Sensors für Gestenerkennung.
* **Komplexe 3D-Metapher:** Steuerung von Roboterarmen im virtuellen Raum. Die horizontale Y-Achse des Sensors ist hierbei doppelt belegt, um unter anderem den Greifmechanismus präzise zu bedienen.
* **2D-Cursor-Selektion:** Präzise Auswahl von Menüs und Objekten im Raum.
* **Echtzeit-Kommunikation:** Sensordaten werden von einem angeschlossenen Raspberry Pi erfasst und per UDP-Protokoll in Echtzeit an die Unity-Anwendung gesendet.

## Verwendete Technologien
* **Game Engine:** Unity (C#)
* **Hardware:** Raspberry Pi, Skywriter Gesture Sensor
* **Schnittstellen:** UDP-Netzwerkprotokoll, Python (für das Raspberry Pi Ausleseskript)

## Kontext
Dieses Projekt entstand im Rahmen eines UX-Projekts an der TH Deggendorf zur Erforschung moderner Mensch-Maschine-Interaktion in der Industrie.

---
*Entwickelt von Julian Kellermann*
