# PowerPlanNotifyicon

It lets you easily control all of your existing power schemes.

If you want to build it yourself and you need UTF-8:


Replace in "PPProcess.cs"
* 'proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850);'

with

* 'proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(65001);'
