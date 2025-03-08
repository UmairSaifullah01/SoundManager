# Changelog

All notable changes to this project will be documented in this file.

## [1.1.0] - 2025-03-08

### Added

- Object pooling system for AudioSources to improve performance
- New extension methods for advanced audio control:
  - `PlaySpatialized` for 3D audio with distance settings
  - `FadeIn` for smooth volume increase
  - `FadeOut` for smooth volume decrease
  - `PlayOneShot` for one-time sound effects
- Integration with CoroutineHandler package for better coroutine management
- Volume and pitch control methods in SoundService
- Support for DontDestroyOnLoad for persistent audio

### Changed

- Improved error handling with descriptive messages
- Better resource management for audio sources
- Optimized dictionary-based sound lookup
- Enhanced initialization process with validation
- Improved handling of looping sounds

### Fixed

- Memory leaks from uncleaned audio sources
- Issues with audio source lifecycle management
- Proper cleanup of finished sounds

## [1.0.0] - 2024-02-15

### Initial Release

- Basic sound management system
- Support for different sound types (SFX, UI, Music)
- Volume control for different sound categories
- Random clip selection feature
- Basic 3D audio support
