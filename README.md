This is a service to control the keyboard backlight behavior on Lenovo P and X series laptops (possibly other models).

It will ensure the keyboard backlight is on after boot and also turn it back on after the system comes out of a standby.

For now, there is no installer but you can manually install using the following elevated command:
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe "path\to\LBCService.exe"

NOTE: This was only tested using the LOCAL SYSTEM account for the service log on account.


This service will create a file called LCBServiceConfig.xml in the same folder as the executable. The path to
Keyboard_Core.dll can be specified/changed here, as well as the preferred brightness level.

TODOs:  Add support for a timeout.
		Add support for enabling again after timeout via KB or mouse movement.
		Both of these will require a "helper" app that will need to run in the user space.

This program was written using the work of liguis- here: https://github.com/ligius-/lenovo-backlight-control/
and Tam Bui's code to hook in to the system power changes here: 
https://social.msdn.microsoft.com/Forums/vstudio/en-US/a195860c-6a24-4526-8e96-06ea56690c12/windows-service-wont-stop-when-registerpowersettingnotification-is-used-to-detect-connected-standby?forum=netfxbcl