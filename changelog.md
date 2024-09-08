## Changelog

#### v1.1.1 - 09-Sept-24
- fix: Improved performance by analyzing tracks asynchronously to calculate max decibels, preventing potential game freezes during audio processing.  
- chore: Created CI pipeline to execute unit tests automatically, ensuring consistent test coverage across builds.
- chore: Removed the redundant NAudio library dependency, utilizing the native audio functionality already provided by the game.

#### v1.1.0 - 07-Sept-24

- feat: Dynamically adjust each track volume based on their differences with the dynamic music volume in decibels.
 
#### v1.0.2 - 01-Sept-24

- fix: Dynamic volume was not setting the right volume, using stale variable.

#### v1.0.1 - 01-Sept-24

- fix: Dynamic volume was not listening to the master volume upon track change.

#### v1.0.0 - 01-Sept-24

- feat: Overrides: Dynamic music: Exploration, Suspense, Combat, Bloodmoon, HomeDay, HomeNight, TraderBob, TraderHugh,
  TraderJen, TraderJoel, TraderRekt.
- feat: Overrides when pausing the game.
- feat: Supports changing volume.
- feat: Ambient music such as crickets sound are NOT overriden.
- feat: Add two free tracks (Creative Commons).
- feat: Supports mp3 & wav; flac and aiff.
- feat: Plays tracks in random, and ensures no two tracks are played in the same order.
