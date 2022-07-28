# UnityProfilerLiteKun

<img width="800" alt="a532971f88c85a4653daae4dab0280b0" src="https://user-images.githubusercontent.com/29646672/137266796-2e436fbd-14f7-48ce-82af-32369759327b.gif">

## Summary

Simplified Unity Profiler。

## What can you do with this Editor extension 

- Displays simplified profiling information in Unity Editor.
- Obtain profiling information as memory allows.
- Save the obtained profiling information in CSV format.
- Static-like window with simplified profiling information could be displayed on the Player side.
  <img width="800" alt="Stats" src="https://user-images.githubusercontent.com/29646672/137267690-ed73cf86-15fd-46da-b66f-65cc6221e071.png">

## Operating Environment 

### Unity Version

- Unity2019.4.5f1
- Unity2020.2.2f1

### Platform

#### Android

- Pixel4XL

## How to use

1. Place Scenes/UnityProfilerLiteKun 
2. Build App (Development Build: ON (required), Autoconnect Profiler: ON (recommended))
3. Open dedicated window from Window->UnityProfilerLiteKun
4. Run the App
5. Press the Record button 

## API

If you wish to measure at a specific time and period, execute the following API from the script.

- UnityProfilerLiteKun.instance.StartRecording()
- UnityProfilerLiteKun.instance.EndRecording()

## Other

In order to have effective performance tuning, constantly record the profile first since you can perform Profiling without limit on the number of Frames being used(as long as the memory allows).
Whenever you detect screen with poor performance, use Profling in UnityProfiler 
This Editor extention is inteded to be used in combination with [UnityChoseKun](https://github.com/katsumasa/UnityChoseKun)
When Stats is enabled, the number of frames will be displayed on the screen, so if you record the screen with UnityChoseKun, it will be easier to find out which screen has poor performance.

If you have any requests or problems, please contact from Issues.
