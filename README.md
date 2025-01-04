# SoundManager

The **SoundManager** is a Unity package designed to simplify audio management in your Unity projects. It provides a flexible and easy-to-use system for playing, stopping, and managing sounds, including support for different sound types (SFX, UI, Music), volume control, and more.

---

## **Key Features**

- **Easy Sound Management**: Play, stop, and manage sounds with simple API calls.
- **Sound Types**: Supports different sound types (SFX, UI, Music) with individual volume controls.
- **Random Sound Playback**: Play random clips from a list of audio clips.
- **Mute Functionality**: Easily mute all sounds with a single setting.
- **Customizable Settings**: Configure sound settings such as volume, pitch, and loop behavior.

---

## **Installation**

To install the SoundManager package in your Unity project, follow these steps:

1. **Download the Package**:
   - Clone the repository or download the package from the [GitHub repository](https://github.com/UmairSaifullah01/SoundManager).

2. **Import into Unity**:
   - Open your Unity project.
   - Navigate to `Assets > Import Package > Custom Package`.
   - Select the downloaded package and click `Import`.

3. **Verify Installation**:
   - After importing, you should see the `SoundManager` folder under `Assets`.

4. **Setup**:
   - Follow the [Configuration Guide](#configuration) to set up the SoundManager in your project.

---

## **Usage**

The SoundManager provides a simple API to manage sounds in your Unity project. Below are some common usage examples:

### **Playing a Sound**
```csharp
SoundService soundService = // Get or create an instance of SoundService
soundService.Play("Explosion");
```
### **Stopping a Sound**
```csharp
soundService.Stop("Explosion");
```
### **Muting All Sounds**
```csharp
soundService.SetMute(true);
```
### **Muting All Sounds**
```csharp
soundService.Play("Explosion", new Vector3(10, 0, 0));
```

## **Configuration**
The SoundManager can be configured to suit your project's needs. Below are the key configuration options:

## **Sound Settings**
- **Master Volume**: The volume of all sounds in the project.
- **Sound Volume**: Controls the volume of sound effects.
- **UI Volume**: Controls the volume of UI sounds.
- **Music Volume**: Controls the volume of background music.
- **Mute**: Mutes all sounds.

## Author Information
- **Name**: Umair Saifullah
- **Email**: contact@umairsaifullah.com
- **Website**: [umairsaifullah.com](https://www.umairsaifullah.com)

## License
This project is licensed under the MIT License. For more details, refer to the LICENSE file.