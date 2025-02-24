TimerTool
=========

A modified version of original TimerTool by Tebjan Halm, et al. (https://github.com/tebjan/TimerTool)
It uses .NET Framework 2.0, so that it can be run even on Windows 2000.

I guess I should limit possible timer resolutions to factors of 10'000'000, so that they always represent an integer value in hertz.
In addition, a resolution of 6.25 ms should already be enough.
(On Windows XP, requesting the resolution of 4 to 7 ms will set the hardware timer to 256 Hz (3.90625 ms), which is a good value too.)
Therefore, I replaced a numeric up/down control with a slider.

System timer resolution values lower than 1.25 ms (800 Hz) should be ridiculed for their considerable negative impact on performance and power efficiency.