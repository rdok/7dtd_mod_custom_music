# 7 Days to Die Mod - No Combat Music
[![nexus-mods-collection-immersive-hud](https://img.shields.io/badge/Nexus%20Mods%20Collection-Immersive%20HUD%20-orange?style=flat-square&logo=spinrilla)](https://next.nexusmods.com/7daystodie/collections/epfqzi) [![nexus-mods-page](https://img.shields.io/badge/Nexus%20Mod-Custom%20Music%20-orange?style=flat-square&logo=spinrilla)](https://www.nexusmods.com/7daystodie/mods/6035) [![github-repository](https://img.shields.io/badge/GitHub-Repository-green?style=flat-square&logo=github)](https://github.com/rdok/7dtd_mod_custom_music)

> **Custom Music:** Replace the dynamic music with your own music.
> **EAC:** This mod uses custom code that is not compatible with Easy Anti-Cheat (EAC).

[![Showcase](https://github.com/rdok/7dtd_mod_custom_music/blob/main/documentation/showcase.jpg?raw=true)](https://www.nexusmods.com/7daystodie/mods/6035)

## Features
> The following features are subject change primarily to own my gaming experience, and secondary to other players' feedback. In cases where these conflict I will provide a Gears mod settings to allow customizations.
- Overrides:
    - Dynamic music: Exploration, Suspense, Combat, Bloodmoon, HomeDay, HomeNight, TraderBob, TraderHugh, TraderJen, TraderJoel, TraderRekt.
    - when pausing the game 
- Supports changing volume
- Ambient music such as crickets sound are NOT overriden.
- Includes two free tracks (Creative Commons). See credits section.
- Supports mp3 & wav; as well as aiff & flac, although I haven't tested the two later. 
  - The two included tracks are in mp3 & wav formats
- Plays tracks in random, and ensures no two tracks are played in the same order.

## User Resources
- To add custom tracks to your game, you can use [youtube-dl](https://github.com/ytdl-org/youtube-dl), a command-line program that allows you to download videos from YouTube.com and other video sites in mp3 format. This tool is legal to use for non-infringing purposes, such as downloading & converting public domain music or tracks you have purchased the rights to.
  - For more information on the Digital Millennium Copyright Act (DMCA) and GitHub's response, see: [Standing up for developers: youtube-dl is back](https://github.blog/news-insights/policy-news-and-insights/standing-up-for-developers-youtube-dl-is-back/).
- I recommend adding music that isn't intense; although for the average player this is not an issue, for players like me who play multiple hours per day, playing consistent intense music quickly becomes tiring/annoying. I have found that ambient music similar to the style of *Red Dead Redemption 2*'s soundtrack provides a more relaxing experience.
  - You might want to explore tracks from the [RDR2 ambient music playlist](https://www.youtube.com/playlist?list=PLeZn0JLtzRu2hHLf8EWbWkaZFWeCp3wrf). These tracks provide lots of breathing room and can be listened to for hundreds of hours without causing listener fatigue.
  
## Backlog
- Lower a bit the music level; even on 13% it's sounds a bit high.
- Apply fade in/out effects when changing tracks.
- Daily Time Allotted; currently this mod plays music constantly. Not a big issue when having lots of music tracks. Regardless, I still want this feature to replicate the RDR2 behaviour/default game behaviour; having that quite in between music tracks adds to the atmosphere, gives a bit of breathing room for the next music track.
- Long tracks; if the track to be played is longer than 7 minutes have the music player play from a random location instead from the start.
- Although I'm against combat music, when listening to legendary Doom music while in combat I can't deny it's worth it. Note to myself: try to make it non-immersion breaking by changing when it stops; e.g. delay stopping the music after killing the last enemy by a few random seconds; in order to not give a queue to the player that no remaining enemies are left.
- Break this to two mods. The music player, and another mod which just loads the tracks to play; this should mitigate any issues from having the custom tracks removed after an update.

## Required Music Attribution

### "Cast Aside" by Hayden Folke
> Track description:  Wonderful cinematic western music by Hayden Folker. This royalty free music is available under Creative Commons License — CC BY 3.0  and you can use it for your videos (even monetized) for free if you credit Hayden Folker  in your video description. MP3 (320 kbps) and WAV (44.1 kHz 16 bit, stereo) audio formats are available for free download. The track has a duration of 2 minutes and 37 seconds, with a tempo of 95 BPM.
- Source: Free Stock Music - free-stock-music.com/hayden-folker-the-red-desert.html
- Artist's Website: soundcloud.com/hayden-folker

### "The Red Desert" by Hayden Folke
> Track description:  Superb suspenseful cinematic music by Hayden Folker. “The Red Desert” is available under Creative Commons License — CC BY 3.0  and you can use it for your videos (even monetized) for free if you credit Hayden Folker  in your video description. MP3 and WAV audio formats are available for free download. The track’s length is 01:20.
- Source: Free Stock Music - free-stock-music.com/hayden-folker-the-red-desert.html
- Artist's Website: soundcloud.com/hayden-folker

## Software Attribution
- To play the audio tracks this mod uses [naudio/NAudio](https://github.com/naudio/NAudio), an Audio and MIDI library for .NET. As of the version [2.2.1](https://github.com/naudio/NAudio/releases/tag/v2.2.1) this library can be used under MIT License. The MIT License is a permissive open-source license that allows software to be freely used, modified, distributed, and used for commercial purposes.

## Changelog  
#### v1.0.0
- Overrides: Dynamic music: Exploration, Suspense, Combat, Bloodmoon, HomeDay, HomeNight, TraderBob, TraderHugh, TraderJen, TraderJoel, TraderRekt.
- Overrides when pausing the game. 
- Supports changing volume.
- Ambient music such as crickets sound are NOT overriden.
- Add two free tracks (Creative Commons).
- Supports mp3 & wav; as well as aiff & flac
- Plays tracks in random, and ensures no two tracks are played in the same order.
