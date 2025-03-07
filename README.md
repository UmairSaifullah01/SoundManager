# SoundManager

The **SoundManager** is a Unity package designed to simplify audio management in your Unity projects. It provides a flexible and easy-to-use system for playing, stopping, and managing sounds, including support for different sound types (SFX, UI, Music), volume control, and more.

---

## **Key Features**

- **Easy Sound Management**: Play, stop, and manage sounds with simple API calls.
- **Sound Types**: Supports different sound types (SFX, UI, Music) with individual volume controls.
- **Random Sound Playback**: Play random clips from a list of audio clips.
- **Mute Functionality**: Easily mute all sounds with a single setting.
- **Customizable Settings**: Configure sound settings such as volume, pitch, and loop behavior.
- **Object Pooling**: Efficient audio source management with automatic pooling.
- **3D Audio Support**: Built-in spatial audio support with configurable distance settings.
- **Volume Control**: Advanced volume control including fade in/out effects.
- **Performance Optimized**: Efficient resource usage with object pooling and proper cleanup.

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
   - Create a SoundSettings asset in your Resources folder
   - Configure your audio mixer groups
   - Follow the [Configuration Guide](#configuration) for detailed setup.

---

## **Usage**

The SoundManager provides multiple ways to manage sounds in your Unity project:

### **Basic Sound Management**

```csharp
// Get or create an instance of SoundService
SoundService soundService = // ...

// Play a sound
soundService.Play("Explosion");

// Stop a sound
soundService.Stop("Explosion");

// Play at position
soundService.Play("Explosion", new Vector3(10, 0, 0));

// Mute all sounds
soundService.SetMute(true);

// Mute specific type
soundService.SetMute(SoundType.Music, true);
```

### **Advanced Sound Control**

```csharp
// Play with volume and pitch
audioClip.Play(transform, volume: 0.8f);

// Play with type, volume, pitch and loop
audioClip.Play(SoundType.Music, transform,
    volume: 0.8f,
    pitch: 1.2f,
    loop: true);

// Fade effects
AudioSource source = soundService.Play("BackgroundMusic");
source.FadeIn(duration: 2f, targetVolume: 0.8f);
source.FadeOut(duration: 1f, destroyWhenDone: true);
```

### **3D Sound System**

```csharp
// Play spatialized sound
audioClip.PlaySpatialized(
    position: transform.position,
    minDistance: 2f,
    maxDistance: 30f,
    volume: 0.7f
);

// Play one-shot sound
audioClip.PlayOneShot(transform, volume: 0.7f);
```

### **Volume Control**

```csharp
// Adjust individual sound volume
soundService.SetVolume("BackgroundMusic", 0.5f);

// Adjust sound pitch
soundService.SetPitch("PlayerFootsteps", 1.2f);

// Stop all sounds
soundService.StopAllSounds();
```

## **Configuration**

### **Sound Settings Asset**

Create a SoundSettings asset in your Resources folder with the following settings:

- **Master Volume**: Controls overall volume (0-1)
- **Sound Type Volumes**:
  - **SFX Volume**: For sound effects
  - **UI Volume**: For interface sounds
  - **Music Volume**: For background music
- **Mixer Groups**: Assign your audio mixer groups for each sound type

### **Sound Types**

The system supports three types of sounds:

- **SFX**: For general sound effects
- **UI**: For user interface sounds
- **Music**: For background music and ambient sounds

### **Performance Settings**

- **Max Concurrent Sounds**: Configure maximum simultaneous sounds (default: 30)
- **Initial Pool Size**: Set initial audio source pool size (default: 1)
- **Auto-Cleanup**: Automatic cleanup of finished sounds
- **DontDestroyOnLoad Support**: Persistent audio between scenes

## Author Information

- **Name**: Umair Saifullah
- **Email**: contact@umairsaifullah.com
- **Website**: [umairsaifullah.com](https://www.umairsaifullah.com)

## License

This project is licensed under the MIT License. For more details, refer to the LICENSE file.
