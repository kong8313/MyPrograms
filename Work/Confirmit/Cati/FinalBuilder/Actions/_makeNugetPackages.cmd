call "_runVsCmd.cmd"

msbuild kit.proj /t:CreateBackendNugetPackage
