## Changelog

#### v1.1.1 - 08-Sept-24
- fix: Brief game freeze upon analyzing track to find max decibels first time.
- chore: CI to run unit tests.
- chore: Remove NAudio library; game already provides it.

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
